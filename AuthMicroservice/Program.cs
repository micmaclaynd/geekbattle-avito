using AuthMicroservice.Consumers;
using AuthMicroservice.Services;
using MassTransit;
using RabbitMQ.Client;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFilesFromDirectory(builder.Configuration.GetOrThrow("ConfigsDirectory"));

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors();
builder.Services.AddControllers();
builder.WebHost.UseUrls(builder.Configuration.GetOrThrow("Auth:Url"));
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(massTransitConfig => {
    massTransitConfig.AddConsumer<LoginConsumer>();
    massTransitConfig.AddConsumer<RegisterConsumer>();
    massTransitConfig.AddConsumer<VerifyConsumer>();

    massTransitConfig.AddRequestClient<IGetUserByUsernameRequest>(new Uri(builder.Configuration.GetOrThrow("Users:Endpoints:GetUserById:Uri")));
    massTransitConfig.AddRequestClient<ICreateUserRequest>(new Uri(builder.Configuration.GetOrThrow("Users:Endpoints:CreateUser:Uri")));

    massTransitConfig.UsingRabbitMq((context, rabbitMqConfig) => {
        rabbitMqConfig.Host(builder.Configuration.GetOrThrow("MessageBroker:Host"), hostConfig => {
            hostConfig.Username(builder.Configuration.GetOrThrow("MessageBroker:Username"));
            hostConfig.Password(builder.Configuration.GetOrThrow("MessageBroker:Password"));
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Auth:Endpoints:Login:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Auth:Endpoints:Login:Exchange"), exchangeConfig => {
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<LoginConsumer>(context);
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Auth:Endpoints:Register:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Auth:Endpoints:Register:Exchange"), exchangeConfig => {
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<RegisterConsumer>(context);
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Auth:Endpoints:Verify:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Auth:Endpoints:Verify:Exchange"), exchangeConfig => {
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<VerifyConsumer>(context);
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


app.UseSwagger(swaggerConfig => {
    swaggerConfig.RouteTemplate = app.Configuration.GetOrThrow("Auth:Swagger:RouteTemplate");
});
app.UseSwaggerUI(swaggerUiConfig => {
    swaggerUiConfig.RoutePrefix = app.Configuration.GetOrThrow("Auth:Swagger:RoutePrefix");
});

app.MapControllers();

app.Run();
