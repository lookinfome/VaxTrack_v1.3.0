using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using v1Remastered.Models;

namespace v1Remastered.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // home page
    public IActionResult Index()
    {
        ViewBag.appName = "VaxTrack";
        ViewBag.appVersion = "v2";
        ViewBag.appDescription = "Launching VaxTrack v2, as an open source platform for India to operate successful vaccination with efficient monitoring with an aim of achieving win over Covid-19."; 

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
