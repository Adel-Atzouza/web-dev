using Calendar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Services
{
    public class EventAttendanceService
    {
        private readonly DatabaseContext _context;

        // Constructor to initialize the database context
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
                return "Event not found."; // Event doesn't exist in the database
            }

            // Fetch the user by ID
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);

            // Check if the user exists
            if (user == null)
            {
                return "User not found."; // User doesn't exist in the database
            }

            // Check if the event has already started
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if (eventToAttend.EventDate < currentDate || 
                (eventToAttend.EventDate == currentDate && eventToAttend.StartTime < DateTime.UtcNow.TimeOfDay))
            {
                return "Event has already started."; // Too late to attend the event
            }

            // Check event availability (based on max attendees)
            int currentAttendees = await _context.Event_Attendance.CountAsync(ea => ea.Event.EventId == eventId);//count attendance records 
            if (currentAttendees >= eventToAttend.MaxAttendees)
            {
                return "Event is full."; // No spots left
            }

            // Check if the user is already attending
            var existingAttendance = await _context.Event_Attendance
                .FirstOrDefaultAsync(ea => ea.Event.EventId == eventId && ea.User.UserId == userId);
            if (existingAttendance != null)
            {
                return "You are already attending this event."; // Prevent duplicate attendance
            }

            // Register the attendance
            var attendance = new Event_Attendance
            {
                User = user,
                Event = eventToAttend,
                Feedback = "", // Empty feedback initially
                Rating = 0     // No rating initially
            };

            _context.Event_Attendance.Add(attendance); // Add attendance to the database
            await _context.SaveChangesAsync(); // Save the changes

            return "Attendance confirmed."; // Successful attendance
        }

        // Method for submitting a review for an event the user attended
        [HttpPost("SubmitReview")]
        public async Task<string> SubmitReview(int userId, int eventId, int rating, string feedback)
        {
            // Check if the user attended the event
            var attendance = await _context.Event_Attendance.FirstOrDefaultAsync(ea => ea.User.UserId == userId && ea.Event.EventId == eventId);

            if (attendance == null)
            {
                return "Attendance not found. User did not attend this event."; // Can't review without attending
            }

            // Update rating and feedback
            attendance.Rating = rating;
            attendance.Feedback = feedback;

            await _context.SaveChangesAsync(); // Save the review
            return "Review has been submitted"; // Success message
        }


        // Method to get a list of attendees for a specific event
        public async Task<List<AttendeeDtogetattendees>> GetEventAttendees(int eventId)
        {
            // Query for event attendance records matching the given event ID
            return await _context.Event_Attendance
                .Where(ea => ea.Event.EventId == eventId) // Filter by event ID
                .Select(ea => new AttendeeDtogetattendees
                {
                    FirstName = ea.User.FirstName, // Select the first name of the user
                    LastName = ea.User.LastName // Select the last name of the user
                })
                .ToListAsync();
        }

        // Method to cancel a user's attendance at an event
        public async Task<string> CancelAttendance(int userId, int eventId)
        {
            // Locate the attendance record based on user and event IDs
            var attendance = await _context.Event_Attendance
                .FirstOrDefaultAsync(ea => ea.User.UserId == userId && ea.Event.EventId == eventId);

            // Check if the attendance record exists
            if (attendance == null)
            {
                return "Attendance not found."; // No attendance found to cancel
            }

            // Remove the attendance record
            _context.Event_Attendance.Remove(attendance);
            await _context.SaveChangesAsync(); // Save changes

            return "Attendance canceled."; // Confirmation message
        }
    }
}
