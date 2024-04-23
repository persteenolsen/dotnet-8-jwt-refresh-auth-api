namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model, ipAddress());
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _userService.RefreshToken(refreshToken, ipAddress());
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeTokenRequest model)
    {
        // accept refresh token in request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        _userService.RevokeToken(token, ipAddress());
        return Ok(new { message = "Token revoked" });
    }
    
    // Note: Used by the Angular Client to show all Users !
    // TEST - Public Route for testing
    // http://localhost:4000/users
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    } 
    
     // TEST
    // Select all columns from the Account and the RefreshToken Table
    // http://localhost:4000/users/users-refresh-tokens-x
    [AllowAnonymous]
    [HttpGet("users-refresh-tokens-x")]
     public IActionResult GetAllX()
     {
       // Not Mapping the Entity to Model
        var users = _userService.GetAllX();
        return Ok(users);
    }

    // TEST 
    // Select only specific columns from the Account Table and all columns from the Refresh Table
    // http://localhost:4000/users/users-refresh-tokens-y
    [AllowAnonymous]
    [HttpGet("users-refresh-tokens-y")]
    public IActionResult GetAllY()
    {
        var users = _userService.GetAllY();
        return Ok(users);
    }
    
    // TEST 
    // Select only specific columns from the both Account and the Refresh Table
    // http://localhost:4000/users/users-refresh-tokens-x
    [AllowAnonymous]
    [HttpGet("users-refresh-tokens-z")]
    public IActionResult GetAllZ()
    {
        var users = _userService.GetAllZ();
        return Ok(users);
    }

    // TEST - Public Route for testing
    // http://localhost:4000/users/1
    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }
    
    // TEST - Public Route for testing
    // http://localhost:4000/users/1/refresh-tokens
    [AllowAnonymous]
    [HttpGet("{id}/refresh-tokens")]
    public IActionResult GetRefreshTokens(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user.RefreshTokens);
    }

    // helper methods

    private void setTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private string ipAddress()
    {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}