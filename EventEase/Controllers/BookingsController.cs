using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string BookingDates, string searchString, string EventTypes, DateTime? startDate, 
            DateTime? endDate, bool? IsAvailable, int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'EventEasePracticeContext.Booking' is null.");
            }

            IQueryable<DateTime> datesQuery = from n in _context.Booking
                                              orderby n.BookingDate
                                              select n.BookingDate;

            var bookings = from v in _context.Booking.Include(b => b.Event).Include(b => b.Venue)
                           select v;

            // Filter by search string for EventName or BookingId
            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(g => g.Event.EventName.ToUpper().Contains(searchString.ToUpper()) ||
                                               g.BookingId.ToString() == searchString);
            }

            // Filter by selected booking date
            if (!string.IsNullOrEmpty(BookingDates))
            {
                if (DateTime.TryParse(BookingDates, out var parsedDate))
                {
                    bookings = bookings.Where(z => z.BookingDate == parsedDate);
                }
            }

            // Filter by Event Type
            if (!string.IsNullOrEmpty(EventTypes))
            {
                bookings = bookings.Where(b => b.Event.EventType.EventTypes == EventTypes);
            }

            // Filter by date range
            if (startDate.HasValue && endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate >= startDate.Value && b.BookingDate <= endDate.Value);
            }

            // Filter by availability
            if (IsAvailable.HasValue)
            {
                bookings = bookings.Where(v => v.Venue.IsAvailable == IsAvailable.Value);
            }

            var BookingVM = new BookingViewModel
            {
                Dates = new SelectList(await _context.Booking.Select(b => b.BookingDate).Distinct().ToListAsync()),
                Bookings = await bookings.ToListAsync()
            };
            return View(BookingVM);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId");
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            // Fetch the event from the database based on booking.EventId
            var @event = await _context.Event.FindAsync(booking.EventId);
            if (@event == null)
            {
                ModelState.AddModelError("EventId", "The selected event does not exist.");
                // Populate dropdown lists and return view early
                ViewData["EventId"] = new SelectList(_context.Set<Event>(), "EventId", "EventId", booking.EventId);
                ViewData["VenueId"] = new SelectList(_context.Set<Venue>(), "VenueId", "VenueId", booking.VenueId);
                return View(booking);
            }
            if (ModelState.IsValid)
            {
                // Check for existing booking: include Event to access EventDate
                var existingBooking = await _context.Booking.Include(b => b.Event)
                    .AnyAsync(b => b.VenueId == booking.VenueId &&
                                   b.Event.EventDate.Date == @event.EventDate.Date);
                if (existingBooking)
                {
                    ModelState.AddModelError("BookingDate", "This venue is already booked for the selected date and time.");
                }
                else
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["EventId"] = new SelectList(_context.Set<Event>(), "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Set<Venue>(), "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
