using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;

class Program
{
    // Ajuste para a URL onde sua API está rodando
    private static readonly string BaseUrl = "http://localhost:5000";

    static async Task<int> Main(string[] args)
    {
        // HttpClient reutilizável
        var handler = new HttpClientHandler();
        // Se for https + cert dev e quiser ignorar (apenas dev):
        // handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

        // STEP: Ingestão (POST /api/documents/upload)
        var postStep = Step.Create("post_documents", async context =>
        {
            var payload = new
            {
                Xml = "<NFe>Exemplo de XML de teste</NFe>",
                Type = "NFe" // ou 0 dependendo de como sua API desserializa enums
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync("/api/documents/upload", content);
            }
            catch (Exception ex)
            {
                return Response.Fail(statusCode: 0, error: ex.Message);
            }

            var body = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? Response.Ok(sizeBytes: body?.Length ?? 0)
                : Response.Fail(statusCode: (int)response.StatusCode, error: body);
        });

        // STEP: Consulta (GET /api/documents?page=1&pageSize=10)
        var getStep = Step.Create("get_documents", async context =>
        {
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync("/api/documents?page=1&pageSize=10");
            }
            catch (Exception ex)
            {
                return Response.Fail(statusCode: 0, error: ex.Message);
            }

            var body = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode
                ? Response.Ok(sizeBytes: body?.Length ?? 0)
                : Response.Fail(statusCode: (int)response.StatusCode, error: body);
        });

        // Cenários: ingestão e consulta
        var postScenario = Scenario.Create("ingest_documents", postStep)
            .WithLoadSimulations(Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromSeconds(30))); // 20 req/s por 30s

        var getScenario = Scenario.Create("query_documents", getStep)
            .WithLoadSimulations(Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromSeconds(30))); // 50 req/s por 30s

        // Registrar e rodar
        var runner = NBomberRunner.RegisterScenarios(postScenario, getScenario);
        var result = runner.Run();

        Console.WriteLine("Finished. NBomber reports saved to .nbomber/reports (ver console).");
        return 0;
    }
}
