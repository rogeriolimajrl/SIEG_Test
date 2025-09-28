using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SIEG_Test.DTOs;
using SIEG_Test.Models;

namespace SIEG_Test.Testes
{
    [TestFixture]
    public class DocumentsControllerTests : IntegrationTestBase
    {
        private UploadXmlDto sampleXml => new()
        {
            Xml = "<xml>teste</xml>",
            Type = DocumentType.NFe
        };

        [Test]
        public async Task Upload_ShouldReturnCreatedId()
        {
            var response = await Client.PostAsJsonAsync("/api/documents/upload", sampleXml);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Resposta da API: " + responseContent);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
            json.Should().ContainKey("id");
        }

        [Test]
        public async Task GetById_ShouldReturnDocument()
        {
            var createResponse = await Client.PostAsJsonAsync("/api/documents/upload", sampleXml);
            var id = (await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>())!["id"];

            var getResponse = await Client.GetAsync($"/api/documents/{id}");
            var doc = await getResponse.Content.ReadFromJsonAsync<Document>();

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            doc.Should().NotBeNull();
            doc!.Id.Should().Be(id);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var getResponse = await Client.GetAsync($"/api/documents/{Guid.NewGuid()}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task List_ShouldReturnDocuments()
        {
            await Client.PostAsJsonAsync("/api/documents/upload", sampleXml);

            var response = await Client.GetAsync("/api/documents?page=1&pageSize=10");
            var list = await response.Content.ReadFromJsonAsync<List<Document>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            list.Should().NotBeEmpty();
        }

        [Test]
        public async Task Update_ShouldModifyDocument()
        {
            var createResponse = await Client.PostAsJsonAsync("/api/documents/upload", sampleXml);
            var id = (await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>())!["id"];

            var updateDto = new UpdateDocumentDto
            {
                TotalValue = 123.45m,
                DestCnpj = "12345678901234"
            };

            var putResponse = await Client.PutAsJsonAsync($"/api/documents/{id}", updateDto);
            putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"/api/documents/{id}");
            var doc = await getResponse.Content.ReadFromJsonAsync<Document>();
            doc!.TotalValue.Should().Be(123.45m);
            doc.DestCnpj.Should().Be("12345678901234");
        }

        [Test]
        public async Task Update_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var updateDto = new UpdateDocumentDto
            {
                TotalValue = 999,
                DestCnpj = "00000000000000"
            };

            var putResponse = await Client.PutAsJsonAsync($"/api/documents/{Guid.NewGuid()}", updateDto);
            putResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_ShouldRemoveDocument_WhenExists()
        {
            var createResponse = await Client.PostAsJsonAsync("/api/documents/upload", sampleXml);
            var id = (await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>())!["id"];

            var deleteResponse = await Client.DeleteAsync($"/api/documents/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"/api/documents/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var deleteResponse = await Client.DeleteAsync($"/api/documents/{Guid.NewGuid()}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
