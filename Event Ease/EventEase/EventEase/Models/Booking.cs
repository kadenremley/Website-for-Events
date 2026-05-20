// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Represents a booking linking a venue and an event with a booking date.
//
// References:
// Microsoft (2025) Data annotations in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
// (Accessed: 10 April 2026)
//
// Microsoft (2025) Relationships in EF Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships
// (Accessed: 05 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on data models.
// The Independent Institute of Education.

//PART 2:
//
// Microsoft (2026) Validation attributes in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
// (Accessed: 04 May 2026)
//
// Microsoft (2026) Date and time handling in .NET.
// Available at: https://learn.microsoft.com/en-us/dotnet/standard/datetime/
// (Accessed: 06 May 2026)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    // Represents a booking made for an event at a venue
    public partial class Booking
    {
        // Primary key (unique ID for each booking)
        public int BookingId { get; set; }

        // Foreign key for Venue (must be selected)
        // Nullable so validation works properly with dropdowns
        [Required]
        public int? VenueId { get; set; }

        // Foreign key for Event (must be selected)
        [Required]
        public int? EventId { get; set; }

        // Date when the booking is made
        [Required]
        public DateTime BookingDate { get; set; }

        // Navigation property (links booking to Event)
        public virtual Event? Event { get; set; }

        // Navigation property (links booking to Venue)
        public virtual Venue? Venue { get; set; }
    }
}