#pragma warning disable CS0618 // Piilottaa MessagingCenter-varoituksen
using System.Windows.Input;
using LifeManagementApp.Models;

namespace LifeManagementApp.ViewModels
{
    public class NoteViewModel
    {
        public Note CurrentNote { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public NoteViewModel() : this(new Note()) { }

        public NoteViewModel(Note note)
        {
            CurrentNote = note;
            SaveCommand = new Command(async () => await SaveAsync());
            DeleteCommand = new Command(async () => await DeleteAsync());
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
    }
}