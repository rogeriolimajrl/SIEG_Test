using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Mongo2Go;
using MongoDB.Driver;
using SIEG_Test.DTOs;
using SIEG_Test.Messaging;
using SIEG_Test.Models;

namespace SIEG_Test.Testes
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly HttpClient Client;
        private readonly MongoDbRunner _mongoRunner;

        public IntegrationTestBase()
        {
            // Inicializa Mongo in-memory
            _mongoRunner = MongoDbRunner.Start();

            Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove configuração real de MongoClient
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IMongoClient));
                    if (descriptor != null) services.Remove(descriptor);

                    // Configura Mongo2Go
                    var mongoClient = new MongoClient(_mongoRunner.ConnectionString);
                    //var db = mongoClient.GetDatabase("siegDb");
                    
                    services.AddSingleton<IMongoClient>(_ => mongoClient);

                    // MassTransit in-memory
                    services.AddMassTransitTestHarness(x =>
                    {
                        x.AddConsumer<DummyConsumer>();
                    });
                });
            });

            Client = Factory.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
            _mongoRunner.Dispose();
        }

        // Dummy consumer para testes (não faz nada)
        public class DummyConsumer : IConsumer<DocumentProcessedEvent>
        {
            public Task Consume(ConsumeContext<DocumentProcessedEvent> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
