using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using LifeManagementApp.Interfaces;
using LifeManagementApp.Models;
using LifeManagementApp.Views;

namespace LifeManagementApp.ViewModels
{
    public class AllNotesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();
        public ICommand AddCommand { get; }
        public ICommand SelectCommand { get; }

        private readonly IJokeService? _jokeService;
        private readonly INoteService _noteService;

        private string _dailyJoke = "Ladataan...";
        public string DailyJoke
        {
            get => _dailyJoke;
            set { _dailyJoke = value; OnPropertyChanged(nameof(DailyJoke)); }
        }

        public AllNotesViewModel(IJokeService jokeService, INoteService noteService)
        {
            _jokeService = jokeService;
            _noteService = noteService;

            AddCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NotePage)));
            SelectCommand = new Command<Note>(async (n) => await Shell.Current.GoToAsync($"{nameof(NotePage)}?id={n.Id}"));

            MessagingCenter.Subscribe<NoteViewModel, string>(this, "Refresh", async (s, arg) => await LoadNotes());

            Task.Run(async () =>
            {
                await LoadDailyJoke();
                await LoadNotes();
            });
        }


        public async Task LoadNotes()
        {
            var notesList = await _noteService.GetNotesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notes.Clear();
                foreach (var note in notesList)
                {
                    Notes.Add(note);
                }
            });
        }

        private async Task LoadDailyJoke()
        {
            if (_jokeService == null) return;
            var jokes = await _jokeService.GetJokesAsync();
            DailyJoke = (jokes != null && jokes.Count > 0) ? jokes[0].ToString() : "Ei vitsiä.";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}