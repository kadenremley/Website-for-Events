// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Handles navigation for the home page, privacy page, and error handling.
//
// References:
// Microsoft (2025) Controllers and actions in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
// (Accessed: 10 April 2026)
//
// Microsoft (2025) Error handling in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
// (Accessed: 08 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on MVC routing.
// The Independent Institute of Education.

using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EventEase.Controllers
{
    // Controller for basic pages like Home and Privacy
    public class HomeController : Controller
    {
        // GET: Home page
        public IActionResult Index()
        {
            return View();
        }

        // GET: Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Error page (shown when an error occurs)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Pass error request ID to the view
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}