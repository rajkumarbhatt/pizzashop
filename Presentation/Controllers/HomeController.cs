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
    public IActionResult Validate([FromBody] LoginViewModel loginModel)
    {
        return _loginService.Validate(loginModel.Email, loginModel.Password);
    }
}
