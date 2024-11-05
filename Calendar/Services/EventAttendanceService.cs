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

        // Check if the event exists
        if (eventToAttend == null)
        {
            return "Event not found.";
        }

        // Fetch the user by ID
        var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);

        // Check if the user exists
        if (user == null)
        {
            return "User not found.";
        }

        // Check if the event has already started
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        if (eventToAttend.EventDate < currentDate || 
        (eventToAttend.EventDate == currentDate && eventToAttend.StartTime < DateTime.UtcNow.TimeOfDay))
        {
            return "Event has already started.";
        }

        // Check event availability
        int currentAttendees = await _context.Event_Attendance.CountAsync(ea => ea.Event.EventId == eventId);
        if (currentAttendees >= eventToAttend.MaxAttendees)
        {
            return "Event is full.";
        }

        // Check if the user is already attending the event
        var existingAttendance = await _context.Event_Attendance
            .FirstOrDefaultAsync(ea => ea.Event.EventId == eventId && ea.User.UserId == userId);
        if (existingAttendance != null)
        {
            return "You are already attending this event.";
        }

        // Register the attendance
        var attendance = new Event_Attendance
        {
            User = user,
            Event = eventToAttend,
            Feedback = "", // Initialize feedback as empty
            Rating = 0     // Initialize rating to 0
        };

        _context.Event_Attendance.Add(attendance);
        await _context.SaveChangesAsync();

        return "Attendance confirmed.";
    }


        // Method to get all attendees of a specific event
        public async Task<List<AttendeeDto>> GetEventAttendees(int eventId)
        {
            return await _context.Event_Attendance
                .Where(ea => ea.Event.EventId == eventId)
                .Select(ea => new AttendeeDto
                {
                    FirstName = ea.User.FirstName,
                    LastName = ea.User.LastName
                })
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
