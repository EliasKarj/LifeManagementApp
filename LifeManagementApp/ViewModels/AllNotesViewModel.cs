using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;
using LifeManagementApp.Interfaces;
using LifeManagementApp.Models;
using LifeManagementApp.Views; // Tarvitaan navigaatioon

namespace LifeManagementApp.ViewModels
{
    public class AllNotesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();
        public ICommand AddCommand { get; }
        public ICommand SelectCommand { get; }

        private string _filePath => Path.Combine(FileSystem.AppDataDirectory, "notes.json");

        private readonly IJokeService? _jokeService;
        private string _dailyJoke = "Ladataan...";

        public string DailyJoke
        {
            get => _dailyJoke;
            set
            {
                _dailyJoke = value;
                OnPropertyChanged(nameof(DailyJoke));
            }
        }

        // Oletuskonstruktori XAML:ää varten (estää joskus XAML-virheitä)
        public AllNotesViewModel()
        {
            AddCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NotePage)));
            SelectCommand = new Command<Note>(async (n) => await Shell.Current.GoToAsync($"{nameof(NotePage)}?id={n.Id}"));
        }

        // Dependency Injection -konstruktori
        public AllNotesViewModel(IJokeService jokeService) : this()
        {
            _jokeService = jokeService;

            // Kuunnellaan viestejä NoteViewModelilta
            MessagingCenter.Subscribe<NoteViewModel, Note>(this, "Save", (s, n) => SaveNote(n));
            MessagingCenter.Subscribe<NoteViewModel, Note>(this, "Delete", (s, n) => DeleteNote(n));

            LoadNotes();

            // Ladataan vitsi taustalla
            Task.Run(async () => await LoadDailyJoke());
        }

        private async Task LoadDailyJoke()
        {
            if (_jokeService == null) return;

            // Pieni viive UI:n latautumiselle
            await Task.Delay(500);
            var jokes = await _jokeService.GetJokesAsync();

            if (jokes != null && jokes.Count > 0)
            {
                DailyJoke = jokes[0].ToString();
            }
            else
            {
                DailyJoke = "Ei vitsiä saatavilla.";
            }
        }

        private void SaveNote(Note note)
        {
            var existing = Notes.FirstOrDefault(x => x.Id == note.Id);
            if (existing != null) Notes.Remove(existing);
            Notes.Insert(0, note); // Lisätään listan alkuun
            SaveToFile();
        }

        private void DeleteNote(Note note)
        {
            var existing = Notes.FirstOrDefault(x => x.Id == note.Id);
            if (existing != null) Notes.Remove(existing);
            SaveToFile();
        }

        private void SaveToFile()
        {
            try
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(Notes));
            }
            catch { }
        }

        private void LoadNotes()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    var loaded = JsonSerializer.Deserialize<List<Note>>(json);
                    if (loaded != null)
                    {
                        Notes.Clear();
                        // Järjestetään uusimmat ensin
                        foreach (var n in loaded.OrderByDescending(x => x.Date))
                            Notes.Add(n);
                    }
                }
            }
            catch { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}