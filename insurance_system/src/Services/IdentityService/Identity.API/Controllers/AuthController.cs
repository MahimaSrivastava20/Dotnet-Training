using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Identity.API.Application.Commands;
using Identity.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
        {
            var result = await _mediator.Send(new LoginCommand(dto));
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult<ApiResponse<string>>> SendOtp([FromBody] SendOtpRequestDto dto)
        {
            var result = await _mediator.Send(new SendOtpCommand(dto.Email));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
        {
            var result = await _mediator.Send(new VerifyOtpCommand(dto.Email, dto.OtpCode));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
