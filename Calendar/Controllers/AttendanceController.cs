using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Calendar.Models;
using System.Threading.Tasks;

namespace Calendar.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AttendanceController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AttendancePost([FromBody] Attendance attendance)
        {
            // Ensure User is included in the query to check for attendance
            bool attendanceExists = await _context.Attendance
                .Include(a => a.User)  // Include User to access its properties
                .AnyAsync(a => a.User.UserId == attendance.User.UserId && a.AttendanceDate == attendance.AttendanceDate);

            if (attendanceExists)
            {
                return BadRequest("");
            }

            // Add and save attendance
            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            // Response with CreatedAtAction pointing to a new GetAttendance action
            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.AttendanceId }, attendance);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendance(int id)
        {
            var attendance = await _context.Attendance
                .Include(a => a.User) // Include User in the result for completeness
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
            {
                return NotFound("");
            }

            return Ok(attendance);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> AttendanceDelete(int id)
        {
            var attendance = await _context.Attendance.FindAsync(id);

            if (attendance == null)
            {
                return NotFound(new { Message = "Attendance record does not exist" });
            }

            _context.Attendance.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
