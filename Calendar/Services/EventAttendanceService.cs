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
            Console.WriteLine($"Searching for Event with ID: {eventId}");
            // Step 1: Fetch the event by its ID from the database
            var eventToAttend = await _context.Event.FirstOrDefaultAsync(e => e.EventId == eventId);

            // Step 2: Check if the event exists
            // If the event is not found, return an error message
            if (eventToAttend == null)
            {
                return "Event not found.";
            }

            // Step 3: Check if the event has already started
            // Convert current date and time
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if (eventToAttend.EventDate < currentDate || 
               (eventToAttend.EventDate == currentDate && eventToAttend.StartTime < DateTime.UtcNow.TimeOfDay))
            {
                return "Event has already started.";
            }

            // Step 4: Check event availability
            // Query the number of attendees for this event
            int currentAttendees = await _context.Event_Attendance.CountAsync(ea => ea.Event.EventId == eventId);
            if (eventToAttend.Event_Attendances.Count >= eventToAttend.MaxAttendees)
            {
                return "Event is full.";
            }

            // Step 5: Check if the user is already attending the event
            // Prevents duplicate attendance records for the same event and user
            var existingAttendance = await _context.Event_Attendance
                .FirstOrDefaultAsync(ea => ea.Event.EventId == eventId && ea.User.UserId == userId);
            if (existingAttendance != null)
            {
                return "You are already attending this event.";
            }

            // Step 6: Register the attendance
            // Create a new Event_Attendance record to log the user's attendance at the event
            var attendance = new Event_Attendance
            {
                User = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId),
                Event = eventToAttend,
                Feedback = "", // Initialize feedback as an empty string, as feedback might be added after the event
                Rating = 0     // Initialize rating to 0; rating may be updated after the event
            };

            
            // Add the new attendance record to the database
            _context.Event_Attendance.Add(attendance);
            await _context.SaveChangesAsync(); // Save changes to make attendance official

            return "Attendance confirmed.";
        }

        // Method to get all attendees of a specific event
        public async Task<List<Event_Attendance>> GetEventAttendees(int eventId)
        {
            // Step 1: Fetch all attendance records for the specified event
            // Includes user information in the results to provide details about attendees
            return await _context.Event_Attendance
                .Include(ea => ea.User) // Load associated User entity for each attendance record
                .Where(ea => ea.Event.EventId == eventId)
                .ToListAsync();
        }

        // Method to cancel a user's attendance at an event
        public async Task<string> CancelAttendance(int userId, int eventId)
        {
            // Step 1: Locate the attendance record in the database
            // Find the specific record that matches both the user and event
            var attendance = await _context.Event_Attendance
                .FirstOrDefaultAsync(ea => ea.User.UserId == userId && ea.Event.EventId == eventId);

            // Step 2: Check if the attendance record exists
            // If the record is not found, return an error message
            if (attendance == null)
            {
                return "Attendance not found.";
            }

            // Step 3: Remove the attendance record from the database
            // Deletes the record and commits the change to cancel attendance
            _context.Event_Attendance.Remove(attendance);
            await _context.SaveChangesAsync(); // Save changes to finalize cancellation

            return "Attendance canceled.";
        }
    }
}
