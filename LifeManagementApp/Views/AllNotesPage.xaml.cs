using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views
{
    public partial class AllNotesPage : ContentPage
    {
        public AllNotesPage(AllNotesViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}