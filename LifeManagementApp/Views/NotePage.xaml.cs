using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class NotePage : ContentPage
{
    public NotePage()
    {
        InitializeComponent();
        BindingContext = new NoteViewModel();
    }
}