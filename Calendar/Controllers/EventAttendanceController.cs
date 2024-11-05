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
        [HttpPost("Attend")]
        public async Task<IActionResult> AttendEvent([FromBody] EventAttendanceRequest request)
        {
            var result = await _attendanceService.AttendEvent(request.UserId, request.EventId);

            return result switch
            {
                "Event not found." => NotFound(result),
                "Event has already started." => BadRequest(result),
                "Event is full." => BadRequest(result),
                _ => Ok(result) // "Attendance confirmed."
            };
        }

        [HttpPost("SubmitReview")]
        public async Task<IActionResult> SubmitReview([FromBody] EventReviewRequest request)
        {
            var result = await _attendanceService.SubmitReview(request.UserId, request.EventId, request.Rating, request.Feedback);

            if (result == "Attendance not found. User did not attend this event.")
                return NotFound(result);

            return Ok(result); // "Review submitted successfully."
        }



        // GET: api/EventAttendance/Attendees/{eventId}
        [HttpGet("Attendees/{eventId}")]
        // Only allow authorized users to access
        public async Task<IActionResult> GetEventAttendees(int eventId)
        {
            var attendees = await _attendanceService.GetEventAttendees(eventId);

            if (attendees == null || attendees.Count == 0)
                return NotFound("No attendees found for this event.");

            return Ok(attendees);
        }

        // DELETE: api/EventAttendance/Cancel
        [HttpDelete("Cancel")]
        public async Task<IActionResult> CancelAttendance([FromBody] EventAttendanceRequest request)
        {
            var result = await _attendanceService.CancelAttendance(request.UserId, request.EventId);

            return result == "Attendance not found." ? NotFound(result) : Ok(result); // "Attendance canceled."
        }
    }
}
