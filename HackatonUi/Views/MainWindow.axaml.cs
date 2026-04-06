using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HackatonUi.ViewModels;

namespace HackatonUi.Views;

public partial class MainWindow : Window
{
    private const double ExpandedWidth = 170;
    private const double CollapsedWidth = 80;

    public double SidebarWidth
    {
        get => _sidebarWidth;
        set
        {
            _sidebarWidth = value;
            this.FindControl<Grid>("MainGrid").ColumnDefinitions[0].Width = new GridLength(value);
        }
    }
    private double _sidebarWidth = CollapsedWidth;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        SidebarWidth = CollapsedWidth; // начальная ширина
    }

    private void Sidebar_PointerEntered(object? sender, PointerEventArgs e)
    {
        AnimateSidebarWidth(ExpandedWidth);
    }

    private void Sidebar_PointerExited(object? sender, PointerEventArgs e)
    {
        AnimateSidebarWidth(CollapsedWidth);
    }

    private async void AnimateSidebarWidth(double targetWidth)
    {
        const int duration = 50; 
        double start = SidebarWidth;
        double step = (targetWidth - start) / duration;

        for (int i = 0; i < duration; i++)
        {
            SidebarWidth += step;
            await Task.Delay(1);
        }

        SidebarWidth = targetWidth; 
    }

    private void Navigate(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag && DataContext is MainWindowViewModel vm)
        {
            vm.Navigate(tag);
        }
    }
    
}