using System.ComponentModel;

namespace LifeManagementApp.Models
{
    public class Note : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private DateTime _date;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}