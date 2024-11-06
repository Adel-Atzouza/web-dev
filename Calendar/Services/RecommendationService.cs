using Calendar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Services
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
                .Where(ea => ea.User.UserId == userId)  // Find all attendances where the user matches
                .Select(ea => ea.Event.EventId)          // Only grab the EventId for each attendance
                .ToListAsync();                         

            // Step 2: Find the unique categories of events the user attended
            var attendedCategories = await _context.Event
                .Where(e => attendedEventIds.Contains(e.EventId) && e.Category != null) // Only events with categories
                .Select(e => e.Category)           // Grab the category of each attended event
                .Distinct()                        // Remove duplicate categories
                .ToListAsync();                   

            // Step 3: Find new events in the same categories that the user hasn’t attended yet
            var recommendations = await _context.Event
                .Where(e => !attendedEventIds.Contains(e.EventId) // Exclude events they already attended
                            && attendedCategories.Contains(e.Category) // Match categories they’ve shown interest in
                            && e.Category != null) // Only include events with a defined category
                .ToListAsync();

            // Step 4: Randomly pick up to three events from the recommendations list
            var randomRecommendations = recommendations
                .OrderBy(_ => Guid.NewGuid()) // Shuffle the order randomly
                .Take(3)                      // Pick the first 3 from the shuffled list
                .ToList();                    

            // Return the randomly selected recommendations
            return randomRecommendations;
        }
    }
}
