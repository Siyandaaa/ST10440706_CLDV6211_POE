using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventEase.Data;
using EventEase.Models;
using System;
using System.Linq;

namespace EventEase.Models
{
    public class SeedData
    {
    public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EventEaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<EventEaseContext>>()))
            {
                if (context.EventType.Any())
                {
                    return;   // DB has been seeded
                }
                context.EventType.AddRange(
                new EventType { EventTypes = "Wedding" },
                new EventType { EventTypes = "Conference" },
                new EventType { EventTypes = "Concert" },
                new EventType { EventTypes = "Birthday Party" },
                new EventType { EventTypes = "Corporate Event" }
                );
                context.SaveChanges();
            }
        }
    }
}
