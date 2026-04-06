using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.ViewModels;
using Avalonia.Markup.Xaml;

namespace HackatonUi.Views
{
    public partial class DocumentsView : UserControl
    {
        public DocumentsView()
        {
            InitializeComponent();

            var mainVm = new MainWindowViewModel();
            DataContext = new DocumentsViewModel(mainVm);
        }
      
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void OpenAddDocument_Click(object sender, RoutedEventArgs e)
        {
            var addDocWindow = new AddDocumentWindow();
            addDocWindow.Closed += (s, args) =>
            {
                if (DataContext is DocumentsViewModel vm)
                {
                    vm.LoadBuildings();
                }
            };
            addDocWindow.Show();
        }
    }
}