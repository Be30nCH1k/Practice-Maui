using UP.ViewModels;

namespace UP.Views;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage(MainMenuViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}