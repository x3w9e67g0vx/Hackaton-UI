using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.ViewModels;

namespace HackatonUi.Views
{
    public partial class ObjectsView : UserControl
    {
        public ObjectsView()
        {
            InitializeComponent();
        }
      
        private void InitializeComponent()
        {
            Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        }
        
        private void OpenAddBuilding_Click(object sender, RoutedEventArgs e)
        {
            var addBuildingWindow = new AddBuildingWindow();
            addBuildingWindow.Show();
        }

        private void OpenEditBuilding_Click(object sender, RoutedEventArgs e)
        {
            var editBuildingWindow = new EditBuildingWindow();
            editBuildingWindow.Show();
        }

        private void OpenAddAttribute_Click(object sender, RoutedEventArgs e)
        {
            var addAttrWindow = new AddAttributeWindow();
            addAttrWindow.Show();
        }
    }
}