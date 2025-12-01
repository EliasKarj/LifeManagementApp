#pragma warning disable CS0618
using LifeManagementApp.Models;
using System.Text.Json;
using System.Windows.Input;

namespace LifeManagementApp.ViewModels
{
    public class NoteViewModel : IQueryAttributable, System.ComponentModel.INotifyPropertyChanged
    {
        private Note _currentNote = new Note();

        public Note CurrentNote
        {
            get => _currentNote;
            set
            {
                _currentNote = value;
                OnPropertyChanged(nameof(CurrentNote));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public NoteViewModel()
        {
            SaveCommand = new Command(async () => await SaveAsync());
            DeleteCommand = new Command(async () => await DeleteAsync());
        }

        public NoteViewModel(Note note) : this()
        {
            CurrentNote = note;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("id"))
            {
                string? noteId = query["id"]?.ToString();
                if (!string.IsNullOrEmpty(noteId))
                {
                    LoadNoteById(noteId);
                }
            }
        }

        private void LoadNoteById(string noteId)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "notes.json");
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var notes = JsonSerializer.Deserialize<List<Note>>(json);
                        var foundNote = notes?.FirstOrDefault(n => n.Id == noteId);
                        if (foundNote != null)
                        {
                            CurrentNote = foundNote;
                        }
                    }
                }
                catch { }
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentNote.Text)) return;
            CurrentNote.Date = DateTime.Now;
            MessagingCenter.Send(this, "Save", CurrentNote);
            await Shell.Current.GoToAsync("..");
        }

        private async Task DeleteAsync()
        {
            MessagingCenter.Send(this, "Delete", CurrentNote);
            await Shell.Current.GoToAsync("..");
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}