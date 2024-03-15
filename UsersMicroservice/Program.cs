using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Shared.Configuration.Extensions;
using UsersMicroservice.Consumers;
using UsersMicroservice.Contexts;
using UsersMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFilesFromDirectory(builder.Configuration.GetOrThrow("ConfigsDirectory"));

builder.Services.AddDbContext<ApplicationContext>(contextConfig => {
    contextConfig.UseMySQL(builder.Configuration.GetOrThrow("Users:Database:ConnectionString"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.WebHost.UseUrls(builder.Configuration.GetOrThrow("Users:Url"));

builder.Services.AddMassTransit(massTransitConfig => {
    massTransitConfig.AddConsumer<GetUserByIdConsumer>();
    massTransitConfig.AddConsumer<GetUserByUsernameConsumer>();
    massTransitConfig.AddConsumer<CreateUserConsumer>();

    massTransitConfig.UsingRabbitMq((context, rabbitMqConfig) => {
        rabbitMqConfig.Host(builder.Configuration.GetOrThrow("MessageBroker:Host"), hostConfig => {
            hostConfig.Username(builder.Configuration.GetOrThrow("MessageBroker:Username"));
            hostConfig.Password(builder.Configuration.GetOrThrow("MessageBroker:Password"));
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Users:Endpoints:GetUserById:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Users:Endpoints:GetUserById:Exchange"), exchangeConfig => {
                exchangeConfig.RoutingKey = builder.Configuration.GetOrThrow("Users:Endpoints:GetUserById:RoutingKey");
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<GetUserByIdConsumer>(context);
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Users:Endpoints:GetUserByUsername:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Users:Endpoints:GetUserByUsername:Exchange"), exchangeConfig => {
                exchangeConfig.RoutingKey = builder.Configuration.GetOrThrow("Users:Endpoints:GetUserByUsername:RoutingKey");
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<GetUserByUsernameConsumer>(context);
        });

        rabbitMqConfig.ReceiveEndpoint(builder.Configuration.GetOrThrow("Users:Endpoints:CreateUser:Queue"), endpointConfig => {
            endpointConfig.Bind(builder.Configuration.GetOrThrow("Users:Endpoints:CreateUser:Exchange"), exchangeConfig => {
                exchangeConfig.ExchangeType = ExchangeType.Direct;

                exchangeConfig.AutoDelete = true;
                exchangeConfig.Durable = false;
            });

            endpointConfig.AutoDelete = true;
            endpointConfig.Durable = false;

            endpointConfig.ConfigureConsumer<CreateUserConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
