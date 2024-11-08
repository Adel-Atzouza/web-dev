using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Calendar.Services;
using Calendar.Models;
using Microsoft.AspNetCore.Authorization;

namespace Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventAttendanceController : Controller
    {
        private readonly EventAttendanceService _attendanceService;

        public EventAttendanceController(EventAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // POST: api/EventAttendance/Attend
        // This endpoint lets a user attend an event, validating the event's availability first
        [HttpPost("Attend")]
        public async Task<IActionResult> AttendEvent([FromBody] EventAttendanceRequest request)
        {
            var result = await _attendanceService.AttendEvent(request.UserId, request.EventId);

            // Respond based on different conditions
            return result switch
            {
                "Event not found." => NotFound(result), // Couldn’t find the event in the database
                "Event has already started." => BadRequest(result), // The event has already started
                "Event is full." => BadRequest(result), // No more available
                _ => Ok(result) // Success! The user is now attending the event
            };
        }

        // POST: api/EventAttendance/SubmitReview
        // Allows users to submit a review after attending an event
        [HttpPost("SubmitReview")]
        public async Task<IActionResult> SubmitReview([FromBody] EventReviewRequest request)
        {
            var result = await _attendanceService.SubmitReview(request.UserId, request.EventId, request.Rating, request.Feedback);

            // If the user didn't attend, return a 404
            if (result == "Attendance not found. User did not attend this event.")
                return NotFound(result);

            // Otherwise, confirm the review was submitted
            return Ok(result); // "Review submitted successfully."
        }

        // GET: api/EventAttendance/Attendees/{eventId}
        // Fetches a list of attendees for a given event
        [HttpGet("Attendees/{eventId}")]
        public async Task<IActionResult> GetEventAttendees(int eventId)
        {
            var attendees = await _attendanceService.GetEventAttendees(eventId);

            // Check if there are any attendees
            if (attendees == null || attendees.Count == 0)
                return NotFound("No attendees found for this event."); // No attendees found

            return Ok(attendees); // Success! Returning the list of attendees
        }

        // DELETE: api/EventAttendance/Cancel
        // Cancels a user's attendance at an event
        [HttpDelete("Cancel")]
        public async Task<IActionResult> CancelAttendance([FromBody] EventAttendanceRequest request)
        {
            var result = await _attendanceService.CancelAttendance(request.UserId, request.EventId);

            // Return a 404 if attendance was not found, otherwise confirm the cancellation
            return result == "Attendance not found." ? NotFound(result) : Ok(result); // "Attendance canceled."
        }
    }
}
