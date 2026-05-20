// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Manages event records including creation, editing, searching, and deletion.
//
// References:
// Microsoft (2024) Overview of ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-10.0
// (Accessed: 12 April 2026)
//
// Microsoft (2025) Model binding in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding
// (Accessed: 12 April 2026)
//
// Microsoft (2025) Model validation.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
// (Accessed: 11 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on MVC architecture.
// The Independent Institute of Education.

//PART 2:
//
// Microsoft (2026) File uploads in ASP.NET Core MVC.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
// (Accessed: 03 May 2026)
//
// Microsoft (2026) Azure Blob Storage for .NET applications.
// Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
// (Accessed: 04 May 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on cloud-based file management.
// The Independent Institute of Education.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;

namespace EventEase.Controllers
{
    // Controller that handles all Event-related actions
    public class EventsController : Controller
    {
        // Database context to access data
        private readonly EventEaseDbContext _context;

        // Blob service for image uploads
        private readonly BlobService _blobService;

        // Constructor to inject database context
        public EventsController(EventEaseDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Events (with search functionality)
        public async Task<IActionResult> Index(string searchString)
        {
            // Get all events from database
            var events = _context.Events.AsQueryable();

            // Filter events if user enters search text
            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Name.Contains(searchString));
            }

            // Send filtered list to view
            return View(await events.ToListAsync());
        }

        // GET: Events/Details (show single event)
        public async Task<IActionResult> Details(int? id)
        {
            // Check if ID is provided
            if (id == null) return NotFound();

            // Find event by ID
            var evt = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);

            // If not found, return error
            if (evt == null) return NotFound();

            return View(evt);
        }

        // GET: Events/Create (show form)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create (handle form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            // Check if form data is valid
            if (ModelState.IsValid)
            {
                // Upload image to Azurite blob storage
                if (evt.ImageFile != null)
                {
                    evt.ImageUrl = await _blobService.UploadFileAsync(evt.ImageFile);
                }

                // Add event to database
                _context.Add(evt);

                // Save changes
                await _context.SaveChangesAsync();

                // Redirect to event list
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload form
            return View(evt);
        }

        // GET: Events/Edit (load event into form)
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if ID is provided
            if (id == null) return NotFound();

            // Find event by ID
            var evt = await _context.Events.FindAsync(id);

            // If not found, return error
            if (evt == null) return NotFound();

            return View(evt);
        }

        // POST: Events/Edit (save changes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            // Check if URL ID matches event ID
            if (id != evt.EventId) return NotFound();

            // Check if form data is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Upload new image if selected
                    if (evt.ImageFile != null)
                    {
                        evt.ImageUrl = await _blobService.UploadFileAsync(evt.ImageFile);
                    }

                    // Update event in database
                    _context.Update(evt);

                    // Save changes
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if event still exists
                    if (!_context.Events.Any(e => e.EventId == evt.EventId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload form
            return View(evt);
        }

        // GET: Events/Delete (confirm delete)
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if ID is provided
            if (id == null) return NotFound();

            // Find event by ID
            var evt = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);

            // If not found, return error
            if (evt == null) return NotFound();

            return View(evt);
        }

        // POST: Events/Delete (delete confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find event by ID
            var evt = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);

            // Check if event exists
            if (evt == null)
            {
                return NotFound();
            }

            // Prevent deletion if event has active bookings
            if (evt.Bookings.Any())
            {
                TempData["ErrorMessage"] = "This event cannot be deleted because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            // Remove event
            _context.Events.Remove(evt);

            // Save changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}