// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Handles venue management including search, creation, editing, and deletion.
//
// References:
// Microsoft (2025) ASP.NET Core MVC controllers.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
// (Accessed: 07 April 2026)
//
// Microsoft (2025) LINQ queries in Entity Framework.
// Available at: https://learn.microsoft.com/en-us/ef/core/querying/
// (Accessed: 07 April 2026)
//
// Microsoft (2025) Working with forms in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms
// (Accessed: 06 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on data handling.
// The Independent Institute of Education.

//PART 2:
//
// Microsoft (2026) Upload files in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
// (Accessed: 03 May 2026)
//
// Microsoft (2026) Azure Blob Storage integration with .NET.
// Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/
// (Accessed: 02 May 2026)
//
// Microsoft (2026) Model validation in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
// (Accessed: 03 May 2026)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;

namespace EventEase.Controllers
{
    // Controller that handles all Venue-related actions
    public class VenuesController : Controller
    {
        // Database context to access data
        private readonly EventEaseDbContext _context;

        // Blob service for image uploads
        private readonly BlobService _blobService;

        // Constructor to inject database context
        public VenuesController(EventEaseDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues (with search functionality)
        public async Task<IActionResult> Index(string searchString)
        {
            // Get all venues from database
            var venues = from v in _context.Venues
                         select v;

            // Filter venues if user enters search text
            if (!string.IsNullOrEmpty(searchString))
            {
                venues = venues.Where(v =>
                    v.Name.Contains(searchString) ||
                    v.Location.Contains(searchString));
            }

            // Send filtered list to view
            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details (show single venue)
        public async Task<IActionResult> Details(int? id)
        {
            // Check if ID is provided
            if (id == null)
            {
                return NotFound();
            }

            // Find venue by ID
            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            // If not found, return error
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create (show form)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create (handle form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            // Check if form data is valid
            if (ModelState.IsValid)
            {
                // Upload image to Azurite blob storage
                if (venue.ImageFile != null)
                {
                    venue.ImageUrl = await _blobService.UploadFileAsync(venue.ImageFile);
                }

                // Add venue to database
                _context.Add(venue);

                // Save changes
                await _context.SaveChangesAsync();

                // Redirect to venue list
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload form
            return View(venue);
        }

        // GET: Venues/Edit (load venue into form)
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if ID is provided
            if (id == null)
            {
                return NotFound();
            }

            // Find venue by ID
            var venue = await _context.Venues.FindAsync(id);

            // If not found, return error
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Edit (save changes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            // Check if URL ID matches venue ID
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            // Check if form data is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Upload new image if selected
                    if (venue.ImageFile != null)
                    {
                        venue.ImageUrl = await _blobService.UploadFileAsync(venue.ImageFile);
                    }

                    // Update venue in database
                    _context.Update(venue);

                    // Save changes
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if venue still exists
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload form
            return View(venue);
        }

        // GET: Venues/Delete (confirm delete)
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if ID is provided
            if (id == null)
            {
                return NotFound();
            }

            // Find venue by ID
            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            // If not found, return error
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete (delete confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find venue by ID
            var venue = await _context.Venues
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            // Check if venue exists
            if (venue == null)
            {
                return NotFound();
            }

            // Prevent deletion if venue has active bookings
            if (venue.Bookings.Any())
            {
                TempData["ErrorMessage"] = "This venue cannot be deleted because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            // Remove venue
            _context.Venues.Remove(venue);

            // Save changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check if a venue exists in the database
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}