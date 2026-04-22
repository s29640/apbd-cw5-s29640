using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.Models;

public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }

    [Required]
    public string OrganizerName { get; set; } = string.Empty;

    [Required]
    public string Topic { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "EndTime musi być późniejsze niż StartTime.",
                new[] { nameof(EndTime), nameof(StartTime) });
        }
    }
}