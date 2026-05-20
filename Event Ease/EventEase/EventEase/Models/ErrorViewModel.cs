// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Stores error request information for displaying error pages.
//
// References:
// Microsoft (2025) Error handling in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
// (Accessed: 11 April 2026)
//
// Microsoft (2025) Models in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/
// (Accessed:10 April 2026)

namespace EventEase.Models
{
    // Model used to display error information on the Error page
    public class ErrorViewModel
    {
        // Stores the unique request ID for tracking errors
        public string? RequestId { get; set; }

        // Returns true if RequestId is not empty (used to decide if it should be shown)
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}