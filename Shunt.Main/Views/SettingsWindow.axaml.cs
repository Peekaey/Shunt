using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Shunt.Main.ViewModels;

namespace Shunt.Main.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<SettingsWindowViewModel>();
    }
}

