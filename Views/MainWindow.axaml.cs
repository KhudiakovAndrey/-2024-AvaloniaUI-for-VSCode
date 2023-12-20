using Avalonia.Controls;
using Avalonia.Interactivity;
using RequestServices_Ivanov.ViewModels;

namespace RequestServices_Ivanov.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    public void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel mainVM = (MainWindowViewModel)DataContext;
        mainVM.CheckLogin(this);
    }
}