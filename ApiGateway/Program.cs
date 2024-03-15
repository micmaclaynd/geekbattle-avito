using ApiGateway.Services;
using MassTransit;
using Microsoft.AspNetCore.Localization;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Auth;
using Shared.Results;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFilesFromDirectory(builder.Configuration.GetOrThrow("ConfigsDirectory"));

builder.Services.AddCors();
builder.WebHost.UseUrls(builder.Configuration.GetOrThrow("ApiGateway:Url"));
builder.Services.AddScoped<IRouterService, RouterService>();

builder.Services.AddMassTransit(massTransitConfig => {
    massTransitConfig.AddRequestClient<IVerifyRequest>(new Uri(builder.Configuration.GetOrThrow("Auth:Endpoints:Verify:Uri")));

    massTransitConfig.UsingRabbitMq((context, rabbitMqConfig) => {
        rabbitMqConfig.Host(builder.Configuration.GetOrThrow("MessageBroker:Host"), hostConfig => {
            hostConfig.Username(builder.Configuration.GetOrThrow("MessageBroker:Username"));
            hostConfig.Password(builder.Configuration.GetOrThrow("MessageBroker:Password"));
        });
    });
});

var app = builder.Build();

app.UseCors(corsConfig => {
    corsConfig.AllowAnyMethod();
    corsConfig.AllowAnyHeader();
    corsConfig.AllowCredentials();
    corsConfig.WithOrigins("http://127.0.0.1:3000", "https://127.0.0.1:3000", "https://avito.micmaclaynd.ru");
});

app.Run(async (context) => {
    var configuration = context.RequestServices.GetService<IConfiguration>();
    var routerService = context.RequestServices.GetService<IRouterService>();
    var route = routerService.GetRouteByPath(context.Request.Path);
    //Console.WriteLine(context.Request.Path);

    if (route == null || !route.Methods.Contains(context.Request.Method.ToUpper())) {
        context.Response.StatusCode = StatusCodes.Status502BadGateway;
        return;
    }

    var nestedUri = routerService.GetNestedUri(route, context.Request.Path);
    var nestedQueryUri = routerService.AddQueryString(nestedUri, context.Request.QueryString.ToString());
    //Console.WriteLine($"{context.Request.Method} {nestedQueryUri}");

    var proxyRequest = new HttpRequestMessage() {
        Method = new HttpMethod(context.Request.Method),
        RequestUri = nestedQueryUri
    };

    if (route.IsRequiredAuth) {
        var verifyRequestClient = context.RequestServices.GetService<IRequestClient<IVerifyRequest>>();
        var verifyToken = context.Request.Cookies[configuration.GetOrThrow("ApiGateway:Http:Cookies:AuthToken")];
        if (verifyToken == null) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        } else {
            var verifyResponse = await verifyRequestClient.GetResponse<IVerifyResponse, IServiceError>(new() {
                Token = verifyToken
            });
            if (verifyResponse.Is(out Response<IVerifyResponse> token)) {
                proxyRequest.Headers.TryAddWithoutValidation(configuration.GetOrThrow("ApiGateway:Http:Headers:UserId"), token.Message.TokenPayload.UserId.ToString());
            } else if (verifyResponse.Is(out Response<IServiceError> error)) {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
    }

    foreach (var header in context.Request.Headers) {
        proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    }

    if (!HttpMethods.IsGet(context.Request.Method)) {
        using var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();
        proxyRequest.Content = new StringContent(requestBody, Encoding.UTF8, context.Request.Headers.ContentType);
    }

    using var httpClient = new HttpClient();

    var proxyResponse = await httpClient.SendAsync(proxyRequest);
    context.Response.StatusCode = Convert.ToInt32(proxyResponse.StatusCode);
    context.Response.Headers.ContentType = context.Request.Headers.Accept.ToString().Split(",").First();

    foreach (var header in proxyResponse.Headers) {
        //Console.WriteLine(header.Key, header.Value.ToString());
        context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
    }


    if (proxyResponse.StatusCode == HttpStatusCode.NotModified) {
        if (context.Response.Headers.ContainsKey("Transfer-Encoding")) {
            context.Response.Headers.Remove("Transfer-Encoding");
        }
    } else {
        context.Response.Headers.TransferEncoding = routerService.GetDefaultTransferEncoding();
    }

    if (proxyResponse.StatusCode != HttpStatusCode.NotModified) {
        var responseContent = await proxyResponse.Content.ReadAsStringAsync();
        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(responseContent));
    }

    //Console.WriteLine(responseContent);
    //Console.WriteLine(proxyResponse.StatusCode);
});
app.Run();