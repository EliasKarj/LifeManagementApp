using LifeManagementApp.Models;

namespace LifeManagementApp.Interfaces
{
    public interface INoteService
    {
        Task<List<Note>> GetNotesAsync();
        Task<Note> GetNoteAsync(string id);
        Task SaveNoteAsync(Note note);
        Task DeleteNoteAsync(Note note);
    }
}