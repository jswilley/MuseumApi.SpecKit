using MuseumApi.Models;

namespace MuseumApi.Data;

public static class DbInitializer
{
    public static void Initialize(MuseumContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if data already exists
        bool hasMuseumHours = context.MuseumDailyHours.Any();
        bool hasSpecialEvents = context.SpecialEvents.Any();

        var today = DateOnly.FromDateTime(DateTime.Today);

        // Seed museum hours - Next 30 days
        if (!hasMuseumHours)
        {
            var museumHours = new List<MuseumDailyHours>();
            
            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(i);
                var dayOfWeek = date.DayOfWeek;
                
                // Museum closed on Mondays
                if (dayOfWeek == DayOfWeek.Monday)
                    continue;
                    
                // Weekend hours (Saturday & Sunday): 10 AM - 4 PM
                if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                {
                    museumHours.Add(new MuseumDailyHours
                    {
                        Date = date,
                        TimeOpen = new TimeOnly(10, 0),
                        TimeClosed = new TimeOnly(16, 0)
                    });
                }
                // Weekday hours: 9 AM - 5 PM
                else
                {
                    museumHours.Add(new MuseumDailyHours
                    {
                        Date = date,
                        TimeOpen = new TimeOnly(9, 0),
                        TimeClosed = new TimeOnly(17, 0)
                    });
                }
            }
            
            context.MuseumDailyHours.AddRange(museumHours);
            context.SaveChanges();
        }

        // Seed special events
        if (!hasSpecialEvents)
        {
            var events = new List<SpecialEvent>
            {
                new SpecialEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = "Ancient Egypt: Treasures of the Pharaohs",
                    EventDescription = "Explore artifacts from ancient Egyptian civilization, including mummies, hieroglyphics, and golden treasures.",
                    Price = 15.00m
                },
                new SpecialEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = "Dinosaurs: Giants of the Past",
                    EventDescription = "Life-sized dinosaur replicas and fossils from the Mesozoic Era. Perfect for families and dinosaur enthusiasts.",
                    Price = 12.50m
                },
                new SpecialEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = "Renaissance Masters",
                    EventDescription = "A curated collection of paintings and sculptures from the Italian Renaissance, featuring works inspired by Leonardo da Vinci and Michelangelo.",
                    Price = 18.00m
                },
                new SpecialEvent
                {
                    EventId = Guid.NewGuid(),
                    EventName = "Space Exploration: Journey to the Stars",
                    EventDescription = "Interactive exhibits on space missions, astronauts, and the future of space travel.",
                    Price = 10.00m
                }
            };

            context.SpecialEvents.AddRange(events);
            context.SaveChanges();

            // Add event dates for each event
            var eventDates = new List<SpecialEventDate>();

            // Ancient Egypt - Running for 3 weeks on weekends
            for (int i = 0; i < 21; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    eventDates.Add(new SpecialEventDate
                    {
                        EventId = events[0].EventId,
                        Date = date
                    });
                }
            }

            // Dinosaurs - Running for 2 weeks daily (except Mondays)
            for (int i = 0; i < 14; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek != DayOfWeek.Monday)
                {
                    eventDates.Add(new SpecialEventDate
                    {
                        EventId = events[1].EventId,
                        Date = date
                    });
                }
            }

            // Renaissance Masters - Running for 4 weeks on Friday, Saturday, Sunday
            for (int i = 0; i < 28; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Friday || 
                    date.DayOfWeek == DayOfWeek.Saturday || 
                    date.DayOfWeek == DayOfWeek.Sunday)
                {
                    eventDates.Add(new SpecialEventDate
                    {
                        EventId = events[2].EventId,
                        Date = date
                    });
                }
            }

            // Space Exploration - Running for 10 days starting a week from now
            for (int i = 7; i < 17; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek != DayOfWeek.Monday)
                {
                    eventDates.Add(new SpecialEventDate
                    {
                        EventId = events[3].EventId,
                        Date = date
                    });
                }
            }

            context.SpecialEventDates.AddRange(eventDates);
            context.SaveChanges();
        }
    }
}
