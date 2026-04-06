using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.ViewModels;
using Avalonia.Markup.Xaml;

namespace HackatonUi.Views
{
    public partial class EditTaskAndDecisionWindow : Window
    {
        private readonly ProtocolsViewModel _viewModel;

        public EditTaskAndDecisionWindow(ProtocolsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveChangesCommand.Execute(null);
            Close();
        }
    
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}