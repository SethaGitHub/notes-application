namespace NoteAPI.Services; 

using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using NoteAPI.Models;

public class NotesService
{
    private readonly IDbConnection _connection;

    public NotesService(IDbConnection connection) => _connection = connection;

    public async Task<IEnumerable<Note>> GetAllNotes() =>
        await _connection.QueryAsync<Note>("SELECT * FROM Notes");

    public async Task<IEnumerable<Note>> GetAllNotesNoneUser() =>
        await _connection.QueryAsync<Note>("SELECT * FROM Notes WHERE userId is NULL");

    public async Task<IEnumerable<Note>> GetNotesByUser(int id) =>
        await _connection.QueryAsync<Note>("SELECT * FROM Notes WHERE userId = @userId", new { userId = id });

    public async Task<Note?> GetNoteById(int id) =>
        await _connection.QueryFirstOrDefaultAsync<Note>("SELECT * FROM Notes WHERE Id = @Id", new { Id = id });

    public async Task<bool> AddNote(Note note)
    {
        note.CreatedAt = DateTime.Now;
        note.UpdatedAt = DateTime.Now;
        var result = await _connection.ExecuteAsync(
            "INSERT INTO Notes (Title, Content, userId, CreatedAt, UpdatedAt) VALUES (@Title, @Content,@userId ,@CreatedAt, @UpdatedAt)", note);
        return result > 0;
    }

    public async Task<bool> UpdateNote(int id, Note note)
    {
        note.UpdatedAt = DateTime.Now;
        var result = await _connection.ExecuteAsync(
            "UPDATE Notes SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt WHERE Id = @Id",
            new { note.Title, note.Content, note.UpdatedAt, Id = id });
        return result > 0;
    }

    public async Task<bool> DeleteNote(int id)
    {
        var result = await _connection.ExecuteAsync("DELETE FROM Notes WHERE Id = @Id", new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<Note>> GetNotesByUserId(int userId) =>
    await _connection.QueryAsync<Note>("SELECT * FROM Notes WHERE UserId = @UserId", new { UserId = userId });

    public async Task<IEnumerable<Note>> GetNotesWithoutUser() =>
        await _connection.QueryAsync<Note>("SELECT * FROM Notes WHERE UserId IS NULL");

}
