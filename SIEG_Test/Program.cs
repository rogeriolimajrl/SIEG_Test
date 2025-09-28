using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SIEG_Test.Controllers;
using SIEG_Test.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings");
    return new MongoClient(settings["ConnectionString"]);
});

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbit = builder.Configuration.GetSection("RabbitMqSettings");
        cfg.Host(rabbit["Host"], "/", h =>
        {
            h.Username(rabbit["Username"]);
            h.Password(rabbit["Password"]);
        });
    });
});

// Serviços
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

// Para testes de integração funcionar
public partial class Program { }
