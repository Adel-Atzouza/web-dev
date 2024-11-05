using Calendar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CalendarProject.Services
{
    public class RecommendationService
    {
        private readonly DatabaseContext _context;

        public RecommendationService(DatabaseContext context)
        {
            _context = context;
        }

        // Method to recommend new events to a user based on their previously attended events
        public async Task<List<Event>> GetRecommendations(int userId)
        {
            // Step 1: Fetch all event IDs that the user has attended
            var attendedEventIds = await _context.Event_Attendance
                .Where(ea => ea.User.UserId == userId)
                .Select(ea => ea.Event.EventId)
                .ToListAsync();

            // Step 2: Fetch unique categories of attended events
            var attendedCategories = await _context.Event
                .Where(e => attendedEventIds.Contains(e.EventId) && e.Category != null) // Only include events with a category
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();

            // Step 3: Recommend new events in the same categories that the user has NOT attended yet
            var recommendations = await _context.Event
                .Where(e => !attendedEventIds.Contains(e.EventId) 
                            && attendedCategories.Contains(e.Category) 
                            && e.Category != null) // Ensure event has a category
                .ToListAsync();

            return recommendations;
        }
    }
}
