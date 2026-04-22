using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetAll(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        IEnumerable<Room> query = AppData.Rooms;

        if (minCapacity.HasValue)
            query = query.Where(r => r.Capacity >= minCapacity.Value);

        if (hasProjector.HasValue)
            query = query.Where(r => r.HasProjector == hasProjector.Value);

        if (activeOnly == true)
            query = query.Where(r => r.IsActive);

        return Ok(query);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Room> GetById([FromRoute] int id)
    {
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (room is null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetByBuilding([FromRoute] string buildingCode)
    {
        var rooms = AppData.Rooms
            .Where(r => string.Equals(r.BuildingCode, buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(rooms);
    }

    [HttpPost]
    public ActionResult<Room> Create([FromBody] Room room)
    {
        var newId = AppData.Rooms.Count == 0 ? 1 : AppData.Rooms.Max(r => r.Id) + 1;

        room.Id = newId;
        AppData.Rooms.Add(room);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Room> Update([FromRoute] int id, [FromBody] Room updatedRoom)
    {
        var existing = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (existing is null)
            return NotFound();

        existing.Name = updatedRoom.Name;
        existing.BuildingCode = updatedRoom.BuildingCode;
        existing.Floor = updatedRoom.Floor;
        existing.Capacity = updatedRoom.Capacity;
        existing.HasProjector = updatedRoom.HasProjector;
        existing.IsActive = updatedRoom.IsActive;

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var room = AppData.Rooms.FirstOrDefault(r => r.Id == id);

        if (room is null)
            return NotFound();

        var hasRelatedReservations = AppData.Reservations.Any(r => r.RoomId == id);
        if (hasRelatedReservations)
        {
            return Conflict(new
            {
                message = "Nie można usunąć sali, która ma powiązane rezerwacje."
            });
        }

        AppData.Rooms.Remove(room);
        return NoContent();
    }
}