using System.Threading.Tasks;
using Blog.ApiModels.Auth;
using Blog.ApiModels.Verfication;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly VerificationService _verificationService;
    private readonly EmailSender _emailSender;
    private readonly IJwtServices _jwtServices;
    public AuthController(VerificationService verificationService, EmailSender emailSender,IJwtServices jwtServices)
    {
        _verificationService = verificationService;
        _emailSender = emailSender;
        _jwtServices = jwtServices;
    }
    [HttpPost]
    public async Task<ActionResult> Register([FromForm]RegisterApiModel registerApiModel)
    {
        var result =await _jwtServices.Register(registerApiModel);
        if (!result.IsAuthenticated)
        {
            return BadRequest(new { error = result.Message });
        }
        return Ok(result);
    }
    [HttpPost("RefreshToken")]
    public async Task<ActionResult> RefreshToken(string refreshToken)
    {
        var result = await _jwtServices.RefreshToken(refreshToken);
        if (!result.IsAuthenticated)
        {
            return BadRequest(new { error = result.Message });
        }
        return Ok(result);
    }
    [HttpPost("LogIn")]
    public async Task<ActionResult> LogIn(LoginApiModel loginApiModel)
    {
        var result = await _jwtServices.GetToken(loginApiModel);
        if (!result.IsAuthenticated)
        {
            return BadRequest(new { error = result.Message });
        }
        return Ok(result);
    }

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] EmailDto emailDto)
    {
        var code = _verificationService.GenerateCode(emailDto.Email);
        await _emailSender.SendVerificationEmail(emailDto.Email, $"Your code is: {code}");
        return Ok(new { Message = "Verification code sent" });
    }
    [HttpPost("RevokToken")]
    public async Task<ActionResult> RevokeToken([FromBody]RevokeToken token)
    {
        var result =await _jwtServices.RevokeToken(token.Token);
        if (!result)
        {
            return BadRequest(new { error = "Can't log out now" });
        }
        return Ok(new {message="logout succesfully"});
    } 
    

    [HttpPost("verify-code")]
    public IActionResult VerifyCode([FromBody] VerifyCodeDto verifyDto)
    {
        var isValid = _verificationService.VerifyCode(verifyDto.Email, verifyDto.Code);
        if (isValid)
            return Ok(new { Message = "Code is valid" });
        else
            return BadRequest(new { Message = "Invalid or expired code" });
    }
}