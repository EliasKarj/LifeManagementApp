using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class NotePage : ContentPage
{
    public NotePage(NoteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}