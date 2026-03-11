using Demo.Application.Features.Authentication.Commands;
using Demo.Dto.Authentication;

namespace Demo.Api.Controllers.Authentication;

public class AuthenticationController(IMediator mediator) : ApiControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<LoginResponse>))]
    public async Task<IActionResult> Login(LoginRequest login)
    {
        var command = new Login.Command(login.Email, login.Password);
        var result = await mediator.Send(command);

        return ObjectResult(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<RefreshTokenResponse>))]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refresh)
    {
        var command = new RefreshToken.Command(refresh.Token, refresh.RefreshToken);
        var result = await mediator.Send(command);

        return ObjectResult(result);
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Empty>))]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPassword)
    {
        var command = new UserForgotPassword.Command(forgotPassword.Email);
        var result = await mediator.Send(command);

        return ObjectResult(result);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Empty>))]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
    {
        var command = new UserResetPassword.Command(resetPassword.Email, resetPassword.Password, resetPassword.ConfirmPassword, resetPassword.Code);
        var result = await mediator.Send(command);

        return ObjectResult(result);
    }
}
