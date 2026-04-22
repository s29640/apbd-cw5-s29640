using System;
using System.Collections.Generic;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Data;

public static class AppData
{
    public static List<Room> Rooms { get; } = new();
    public static List<Reservation> Reservations { get; } = new();

    private static bool _initialized = false;

    public static void Seed()
    {
        if (_initialized) return;

        Rooms.AddRange(new[]
        {
            new Room
            {
                Id = 1,
                Name = "Sala A101",
                BuildingCode = "A",
                Floor = 1,
                Capacity = 20,
                HasProjector = true,
                IsActive = true
            },
            new Room
            {
                Id = 2,
                Name = "Sala A202",
                BuildingCode = "A",
                Floor = 2,
                Capacity = 30,
                HasProjector = true,
                IsActive = true
            },
            new Room
            {
                Id = 3,
                Name = "Sala B015",
                BuildingCode = "B",
                Floor = 0,
                Capacity = 12,
                HasProjector = false,
                IsActive = true
            },
            new Room
            {
                Id = 4,
                Name = "Sala C310",
                BuildingCode = "C",
                Floor = 3,
                Capacity = 40,
                HasProjector = true,
                IsActive = false
            },
            new Room
            {
                Id = 5,
                Name = "Lab 204",
                BuildingCode = "B",
                Floor = 2,
                Capacity = 24,
                HasProjector = true,
                IsActive = true
            }
        });

        Reservations.AddRange(new[]
        {
            new Reservation
            {
                Id = 1,
                RoomId = 1,
                OrganizerName = "Anna Kowalska",
                Topic = "Warsztaty HTTP",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                Status = "confirmed"
            },
            new Reservation
            {
                Id = 2,
                RoomId = 2,
                OrganizerName = "Jan Nowak",
                Topic = "Konsultacje REST",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 30),
                Status = "planned"
            },
            new Reservation
            {
                Id = 3,
                RoomId = 3,
                OrganizerName = "Maria Zielińska",
                Topic = "Szkolenie z Postmana",
                Date = new DateOnly(2026, 5, 11),
                StartTime = new TimeOnly(8, 30),
                EndTime = new TimeOnly(11, 0),
                Status = "confirmed"
            },
            new Reservation
            {
                Id = 4,
                RoomId = 5,
                OrganizerName = "Piotr Wiśniewski",
                Topic = "ASP.NET Core API",
                Date = new DateOnly(2026, 5, 12),
                StartTime = new TimeOnly(13, 0),
                EndTime = new TimeOnly(15, 0),
                Status = "planned"
            }
        });

        _initialized = true;
    }
}