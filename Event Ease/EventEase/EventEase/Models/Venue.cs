// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Represents a venue with name, location, capacity, and optional image.
//
// References:
// Microsoft (2025) Entity Framework Core modeling.
// Available at: https://learn.microsoft.com/en-us/ef/core/modeling/
// (Accessed: 05 April 2026)
//
// Microsoft (2025) Nullable reference types in C#.
// Available at: https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references
// (Accessed: 05 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on data structures.
// The Independent Institute of Education.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    // Represents a venue where events can take place
    public partial class Venue
    {
        // Primary key (unique ID for each venue)
        public int VenueId { get; set; }

        // Name of the venue
        public string Name { get; set; } = null!;

        // Location of the venue
        public string Location { get; set; } = null!;

        // Maximum number of people the venue can hold
        public int Capacity { get; set; }

        // URL for the venue image (optional)
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation property (one venue can have many bookings)
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}