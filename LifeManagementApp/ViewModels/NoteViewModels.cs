using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace LifeManagementApp.ViewModels
{
    // --- MODEL (Tietomalli) ---
    public class Note : INotifyPropertyChanged
    {
        private string _text;
        private DateTime _date;
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- VIEWMODEL: Yksittäinen muistiinpano ---
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

    // --- VIEWMODEL: Kaikki muistiinpanot ---
    public class AllNotesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();
        public ICommand AddCommand { get; }
        public ICommand SelectCommand { get; }

        // Tallennuspolku puhelimen muistiin
        private string _filePath => Path.Combine(FileSystem.AppDataDirectory, "notes.json");

        public AllNotesViewModel()
        {
            AddCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(Views.NotePage)));
            SelectCommand = new Command<Note>(async (n) => await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?id={n.Id}"));

            MessagingCenter.Subscribe<NoteViewModel, Note>(this, "Save", (s, n) => SaveNote(n));
            MessagingCenter.Subscribe<NoteViewModel, Note>(this, "Delete", (s, n) => DeleteNote(n));

            LoadNotes();
        }

        private void SaveNote(Note note)
        {
            var existing = Notes.FirstOrDefault(x => x.Id == note.Id);
            if (existing != null) Notes.Remove(existing);
            Notes.Insert(0, note);
            SaveToFile();
        }

        private void DeleteNote(Note note)
        {
            var existing = Notes.FirstOrDefault(x => x.Id == note.Id);
            if (existing != null) Notes.Remove(existing);
            SaveToFile();
        }

        private void SaveToFile() => File.WriteAllText(_filePath, JsonSerializer.Serialize(Notes));

        private void LoadNotes()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<Note>>(json);
                if (loaded != null)
                {
                    Notes.Clear();
                    foreach (var n in loaded.OrderByDescending(x => x.Date)) Notes.Add(n);
                }
            }
            catch { /* Virheiden sivuutus demoa varten */ }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}