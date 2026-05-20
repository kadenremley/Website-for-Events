// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Handles booking creation, editing, deletion, and search functionality.
//
// References:
// Microsoft (2025) Controllers in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
// (Accessed: 13 April 2026)
//
// Microsoft (2025) Working with data in EF Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/saving/
// (Accessed: 13 April 2026)
//
// Microsoft (2025) Loading related data.
// Available at: https://learn.microsoft.com/en-us/ef/core/querying/related-data/
// (Accessed: 12 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on CRUD operations.
// The Independent Institute of Education.

//PART 2:
//
// Microsoft (2026) Querying data with Entity Framework Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/querying/
// (Accessed: 04 May 2026)
//
// Microsoft (2026) Preventing duplicate records in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud
// (Accessed: 04 May 2026)
//
// Microsoft (2026) Validation and error handling in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
// (Accessed: 04 May 2026)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    // Controller that handles all Booking-related actions
    public class BookingsController : Controller
    {
        // Database context (used to access database tables)
        private readonly EventEaseDbContext _context;

        // Constructor to inject the database context
        public BookingsController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: Bookings (with search functionality)
        public async Task<IActionResult> Index(string searchString)
        {
            // Load bookings including related Event and Venue data
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // If user enters a search term, filter results
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.Venue.Name.Contains(searchString) ||
                    b.Event.Name.Contains(searchString));
            }

            // Return list of bookings to the view
            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Create (shows form)
        public IActionResult Create()
        {
            // Populate dropdown list for Venues
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");

            // Populate dropdown list for Events
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name");

            return View();
        }

        // POST: Bookings/Create (handles form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Check if booking date was entered
            if (booking.BookingDate == DateTime.MinValue)
            {
                ModelState.AddModelError("BookingDate", "Booking date is required.");
            }

            // Only check duplicates if date is valid
            if (ModelState.IsValid)
            {
                bool bookingExists = await _context.Bookings.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate == booking.BookingDate);

                // If booking already exists
                if (bookingExists)
                {
                    ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
                }
            }

            // Save booking if everything is valid
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if validation fails
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);

            return View(booking);
        }

        // GET: Bookings/Edit (load booking data into form)
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if ID is provided
            if (id == null)
                return NotFound();

            // Find booking by ID
            var booking = await _context.Bookings.FindAsync(id);

            // If not found, return error
            if (booking == null)
                return NotFound();

            // Populate dropdowns with selected values
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);

            return View(booking);
        }

        // POST: Bookings/Edit (save changes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            // Check if URL ID matches booking ID
            if (id != booking.BookingId)
                return NotFound();

            // Check if venue is already booked for same date/time
            bool bookingExists = await _context.Bookings.AnyAsync(b =>
                b.BookingId != booking.BookingId &&
                b.VenueId == booking.VenueId &&
                b.BookingDate == booking.BookingDate);

            // Show validation error if duplicate booking exists
            if (bookingExists)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
            }

            // If validation fails, reload form with dropdowns
            if (!ModelState.IsValid)
            {
                ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);

                return View(booking);
            }

            try
            {
                // Update booking in database
                _context.Update(booking);

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Return error if booking no longer exists
                if (!_context.Bookings.Any(e => e.BookingId == booking.BookingId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Delete (confirm delete page)
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if ID is provided
            if (id == null)
                return NotFound();

            // Load booking with related data
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            // If not found, return error
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete (delete confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find booking by ID
            var booking = await _context.Bookings.FindAsync(id);

            // If found, remove it
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            // Redirect back to list
            return RedirectToAction(nameof(Index));
        }
    }
}