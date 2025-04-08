using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;

namespace Presentaion.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILoginService _loginService;

    public HomeController(ILogger<HomeController> logger, ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("api/validate")]
    public async Task<IActionResult> Validate([FromBody] LoginViewModel loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Email))
        {
            return BadRequest("Email cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(loginModel.Password))
        {
            return BadRequest("Password cannot be null or empty.");
        }

        return await _loginService.ValidateAsync(loginModel.Email, loginModel.Password);
    }
}
