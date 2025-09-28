using Microsoft.AspNetCore.Mvc;
using SIEG_Test.DTOs;
using SIEG_Test.Repositories;
using SIEG_Test.Services;

namespace SIEG_Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService doc_service;

        public DocumentsController(IDocumentService service)
        {
            doc_service = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] UploadXmlDto dto)
        {
            var id = await doc_service.ProcessXmlDocument(dto.Xml, dto.Type);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var doc = await doc_service.GetByIdDocument(id);
            if (doc == null) return NotFound();
            return Ok(doc);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] DocumentQuery query)
        {
            var docs = await doc_service.ListDocument(query);
            return Ok(docs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentDto dto)
        {
            var updated = await doc_service.UpdateDocument(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await doc_service.DeleteDocument(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

}
