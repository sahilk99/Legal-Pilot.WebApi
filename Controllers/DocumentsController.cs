using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Compliance_Hub.api.DTOs;
using Compliance_Hub.api.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Compliance_Hub.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IAuditService _auditService;

        public DocumentsController(IDocumentService documentService, IAuditService auditService)
        {
            _documentService = documentService;
            _auditService = auditService;
        }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);

        [HttpGet]
        [Authorize(Roles = "Admin,Auditor,Viewer")]
        public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? status, [FromQuery] int? departmentId)
        {
            var userId = GetUserId();
            var documents = await _documentService.GetAllAsync(category, status, departmentId);
            
            await _auditService.LogAsync(userId, "Viewed", "Document", 0, "Viewed filtered list of documents");
            
            return Ok(documents);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Auditor,Viewer")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var document = await _documentService.GetByIdAsync(id);
            
            if (document == null)
            {
                return NotFound();
            }

            await _auditService.LogAsync(userId, "Viewed", "Document", id, $"Viewed document: {document.Title}");
            
            return Ok(document);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDocumentDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Title))
            {
                return BadRequest("Title is required.");
            }

            if (dto.ExpiryDate.HasValue && dto.ExpiryDate.Value <= DateTime.UtcNow)
            {
                return BadRequest("ExpiryDate must be a future date.");
            }

            var userId = GetUserId();
            var document = await _documentService.CreateAsync(dto, userId);

            await _auditService.LogAsync(userId, "Created", "Document", document.Id, $"Created document: {document.Title}");
            
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentDTO dto)
        {
            var userId = GetUserId();
            var document = await _documentService.UpdateAsync(id, dto);
            
            if (document == null)
            {
                return NotFound();
            }

            await _auditService.LogAsync(userId, "Updated", "Document", id, $"Updated document: {document.Title}");
            
            return Ok(document);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var success = await _documentService.DeleteAsync(id);
            
            if (!success)
            {
                return NotFound();
            }

            await _auditService.LogAsync(userId, "Deleted", "Document", id, $"Deleted document ID: {id}");
            
            return NoContent();
        }
    }
}
