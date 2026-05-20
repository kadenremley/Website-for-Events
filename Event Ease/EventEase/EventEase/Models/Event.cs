// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Defines event details including name, dates, and optional image.
//
// References:
// Microsoft (2025) Entity types in EF Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types
// (Accessed: 19 April 2026)
//
// Microsoft (2025) Working with date and time in .NET.
// Available at: https://learn.microsoft.com/en-us/dotnet/standard/datetime/
// (Accessed: 08 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on entity design.
// The Independent Institute of Education.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    // Represents an event in the system
    public partial class Event
    {
        // Primary key (unique ID for each event)
        public int EventId { get; set; }

        // Name of the event
        public string Name { get; set; } = null!;

        // Date and time when the event starts
        public DateTime StartDate { get; set; }

        // Date and time when the event ends
        public DateTime EndDate { get; set; }

        // URL for the event image (optional)
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation property (one event can have many bookings)
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}