using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        IEnumerable<Reservation> query = AppData.Reservations;

        if (date.HasValue)
            query = query.Where(r => r.Date == date.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));

        if (roomId.HasValue)
            query = query.Where(r => r.RoomId == roomId.Value);

        return Ok(query);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reservation> GetById([FromRoute] int id)
    {
        var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation is null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpPost]
    public ActionResult<Reservation> Create([FromBody] Reservation reservation)
    {
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room is null)
        {
            return BadRequest(new
            {
                message = "Nie można dodać rezerwacji dla nieistniejącej sali."
            });
        }

        if (!room.IsActive)
        {
            return BadRequest(new
            {
                message = "Nie można dodać rezerwacji dla nieaktywnej sali."
            });
        }

        if (HasConflict(reservation))
        {
            return Conflict(new
            {
                message = "Rezerwacja koliduje czasowo z inną rezerwacją tej samej sali."
            });
        }

        var newId = AppData.Reservations.Count == 0 ? 1 : AppData.Reservations.Max(r => r.Id) + 1;
        reservation.Id = newId;
        AppData.Reservations.Add(reservation);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Reservation> Update([FromRoute] int id, [FromBody] Reservation updatedReservation)
    {
        var existing = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (existing is null)
            return NotFound();

        var room = AppData.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room is null)
        {
            return BadRequest(new
            {
                message = "Nie można przypisać rezerwacji do nieistniejącej sali."
            });
        }

        if (!room.IsActive)
        {
            return BadRequest(new
            {
                message = "Nie można przypisać rezerwacji do nieaktywnej sali."
            });
        }

        updatedReservation.Id = id;

        if (HasConflict(updatedReservation, ignoreReservationId: id))
        {
            return Conflict(new
            {
                message = "Zaktualizowana rezerwacja koliduje czasowo z inną rezerwacją tej samej sali."
            });
        }

        existing.RoomId = updatedReservation.RoomId;
        existing.OrganizerName = updatedReservation.OrganizerName;
        existing.Topic = updatedReservation.Topic;
        existing.Date = updatedReservation.Date;
        existing.StartTime = updatedReservation.StartTime;
        existing.EndTime = updatedReservation.EndTime;
        existing.Status = updatedReservation.Status;

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var reservation = AppData.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation is null)
            return NotFound();

        AppData.Reservations.Remove(reservation);
        return NoContent();
    }

    private static bool HasConflict(Reservation candidate, int? ignoreReservationId = null)
    {
        return AppData.Reservations.Any(existing =>
            existing.RoomId == candidate.RoomId &&
            existing.Date == candidate.Date &&
            existing.Id != ignoreReservationId &&
            candidate.StartTime < existing.EndTime &&
            candidate.EndTime > existing.StartTime);
    }
}