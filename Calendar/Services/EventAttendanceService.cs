using Calendar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Services
{
    public class EventAttendanceService
    {
        private readonly DatabaseContext _context;

        // Constructor that injects the database context (DatabaseContext)
        public EventAttendanceService(DatabaseContext context)
        {
            _context = context;
        }

        // Method for a user to attend an event
        public async Task<string> AttendEvent(int userId, int eventId)
        {
            // Fetch the event by its ID from the database
            var eventToAttend = await _context.Event.FirstOrDefaultAsync(e => e.EventId == eventId);

            // If the event is not found, return an error message
            if (eventToAttend == null)
            {
                return "Event not found.";
            }

            // Check if the event has already started
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if (eventToAttend.EventDate < currentDate || eventToAttend.EventDate == currentDate && eventToAttend.StartTime < DateTime.UtcNow.TimeOfDay)
            {
                return "Event has already started.";
            }

            // Check if the event still has availability (assuming MaxAttendees property is added)
            int currentAttendees = await _context.Event_Attendance.CountAsync(ea => ea.Event.EventId == eventId);
            if (eventToAttend.Event_Attendances.Count >= eventToAttend.MaxAttendees)
            {
                return "Event is full.";
            }

            // Register the attendance
            var attendance = new Event_Attendance
            {
                User = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId),
                Event = eventToAttend,
                Feedback = "", // Default to empty, assuming feedback is added post-event
                Rating = 0
            };

            _context.Event_Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return "Attendance confirmed.";
        }

        // Method to get all attendees of a specific event
        public async Task<List<Event_Attendance>> GetEventAttendees(int eventId)
        {
            // Fetch the event attendees, including the user information
            return await _context.Event_Attendance
                .Include(ea => ea.User)
                .Where(ea => ea.Event.EventId == eventId)
                .ToListAsync();
        }

        // Method to cancel a user's attendance at an event
        public async Task<string> CancelAttendance(int userId, int eventId)
        {
            // Find the attendance record in the database
            var attendance = await _context.Event_Attendance
                .FirstOrDefaultAsync(ea => ea.User.UserId == userId && ea.Event.EventId == eventId);

            // If no attendance record is found, return an error message
            if (attendance == null)
            {
                return "Attendance not found.";
            }

            // Remove the attendance from the database and save changes
            _context.Event_Attendance.Remove(attendance);
            await _context.SaveChangesAsync();

            return "Attendance canceled.";
        }
    }
}
