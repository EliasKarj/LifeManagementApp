using LifeManagementApp.Data;
using LifeManagementApp.Interfaces;
using LifeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LifeManagementApp.Services
{
    public class NoteService : INoteService
    {
        private LifeManagementContext _context;

        public NoteService()
        {
            // Luodaan tietokanta, jos sitä ei ole olemassa (Code First yksinkertaistettuna)
            _context = new LifeManagementContext();
            _context.Database.EnsureCreated();
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            // Haetaan kaikki muistiinpanot tietokannasta
            return await _context.Notes.OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<Note> GetNoteAsync(string id)
        {
            return await _context.Notes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SaveNoteAsync(Note note)
        {
            // Tarkistetaan onko muistiinpano jo olemassa
            var existingNote = await _context.Notes.FirstOrDefaultAsync(x => x.Id == note.Id);

            if (existingNote == null)
            {
                // Uusi muistiinpano -> Lisätään
                await _context.Notes.AddAsync(note);
            }
            else
            {
                // Vanha muistiinpano -> Päivitetään tiedot
                // Koska note-olio tulee UI:sta, kopioimme arvot seurattuun olioon
                existingNote.Text = note.Text;
                existingNote.Date = note.Date;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(Note note)
        {
            var existingNote = await _context.Notes.FirstOrDefaultAsync(x => x.Id == note.Id);
            if (existingNote != null)
            {
                _context.Notes.Remove(existingNote);
                await _context.SaveChangesAsync();
            }
        }
    }
}