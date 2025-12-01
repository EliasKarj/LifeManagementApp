using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

[QueryProperty(nameof(NoteId), "id")]
public partial class NotePage : ContentPage
{
    public NotePage()
    {
        InitializeComponent();
        BindingContext = new NoteViewModel();
    }

    public string NoteId
    {
        set
        {
            var vm = Shell.Current.CurrentPage is AllNotesPage anp ? anp.BindingContext as AllNotesViewModel : null;
            if (vm != null)
            {
                var note = vm.Notes.FirstOrDefault(n => n.Id == value);
                if (note != null)
                {
                    BindingContext = new NoteViewModel(note);
                }
            }
        }
    }
}