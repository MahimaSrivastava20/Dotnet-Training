using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Policy.API.Application.Commands;
using Policy.API.Application.DTOs;
using Policy.API.Application.Queries;
using Policy.API.Infrastructure.Data;
using Policy.API.Infrastructure.Services;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Policy.API.Controllers
{
    [ApiController]
    [Route("api/v1/policies")]
    public class PoliciesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly PolicyDbContext _db;
        private readonly IPolicyDocumentGenerator _docGenerator;

        public PoliciesController(IMediator mediator, PolicyDbContext db, IPolicyDocumentGenerator docGenerator)
        {
            _mediator = mediator;
            _db = db;
            _docGenerator = docGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PolicyCatalogDto>>>> GetPolicies([FromQuery] PolicyType? type, [FromQuery] decimal? maxPrice)
        {
            var result = await _mediator.Send(new GetPoliciesQuery(type, maxPrice));
            return Ok(result);
        }

        [HttpPost("calculate-premium")]
        public async Task<ActionResult<ApiResponse<PremiumCalculationResultDto>>> CalculatePremium([FromBody] CalculatePremiumRequestDto dto)
        {
            var result = await _mediator.Send(new CalculatePremiumCommand(dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("purchase/initiate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<RazorpayOrderResponseDto>>> InitiatePurchase([FromBody] InitiatePurchaseDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new InitiatePurchaseCommand(userId, dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("purchase/verify-payment")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserPolicyDto>>> VerifyPayment([FromBody] VerifyRazorpayPaymentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new VerifyPaymentAndIssuePolicyCommand(userId, dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-policies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<UserPolicyDto>>>> GetMyPolicies()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new GetMyPoliciesQuery(userId));
            return Ok(result);
        }

        [HttpGet("my-policies/{id}/download")]
        [Authorize]
        public async Task<IActionResult> DownloadPolicyDocument(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var policy = await _db.UserPolicies.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (policy == null) return NotFound("Policy certificate not found.");

            var documentText = _docGenerator.GeneratePolicyDocumentText(policy);
            var bytes = Encoding.UTF8.GetBytes(documentText);

            return File(bytes, "text/plain", $"PolicyCertificate_{policy.PolicyNumber}.txt");
        }

        [HttpPost("renew/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserPolicyDto>>> RenewPolicy(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new RenewPolicyCommand(id, userId));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
