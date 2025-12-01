#pragma warning disable CS0618
using LifeManagementApp.Interfaces;
using LifeManagementApp.Models;
using System.Windows.Input;

namespace LifeManagementApp.ViewModels
{
    public class NoteViewModel : IQueryAttributable, System.ComponentModel.INotifyPropertyChanged
    {
        private Note _currentNote = new Note();
        private readonly INoteService _noteService;

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

        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            SaveCommand = new Command(async () => await SaveAsync());
            DeleteCommand = new Command(async () => await DeleteAsync());
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("id"))
            {
                string? noteId = query["id"]?.ToString();
                if (!string.IsNullOrEmpty(noteId))
                {
                    Task.Run(async () =>
                    {
                        var note = await _noteService.GetNoteAsync(noteId);
                        if (note != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                CurrentNote = note;
                            });
                        }
                    });
                }
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentNote.Text)) return;

            CurrentNote.Date = DateTime.Now;

            await _noteService.SaveNoteAsync(CurrentNote);

            MessagingCenter.Send(this, "Refresh", "Saved");

            await Shell.Current.GoToAsync("..");
        }

        private async Task DeleteAsync()
        {
            // Poistetaan tietokannasta
            await _noteService.DeleteNoteAsync(CurrentNote);

            MessagingCenter.Send(this, "Refresh", "Deleted");

            await Shell.Current.GoToAsync("..");
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}