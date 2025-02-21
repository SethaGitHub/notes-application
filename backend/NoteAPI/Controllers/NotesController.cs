using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using NoteAPI.Models;
using NoteAPI.Services;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly NotesService _notesService;

    public NotesController(NotesService notesService)
    {
        _notesService = notesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        var notes = await _notesService.GetAllNotes();
        return Ok(notes);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotesNoneUser()
    {
        var notes = await _notesService.GetAllNotesNoneUser();
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var note = await _notesService.GetNoteById(id);
        return note is not null ? Ok(note) : NotFound();
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetNotesByUser(int id)
    {
        var note = await _notesService.GetNotesByUser(id);
        return note is not null ? Ok(note) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] Note note)
    {
        var success = await _notesService.AddNote(note);
        return success ? CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note) : BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
    {
        var updated = await _notesService.UpdateNote(id, note);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var deleted = await _notesService.DeleteNote(id);
        return deleted ? NoContent() : NotFound();
    }
}
