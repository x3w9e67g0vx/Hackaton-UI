using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.ViewModels;

namespace HackatonUi.Views
{
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserViewModel vm)
            {
                vm.Assign();
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DecisionAssignmentInfo assignment)
            {
                if (this.DataContext is UserViewModel vm)
                {
                    vm.DeleteAssignment(assignment);
                }
            }
        }
    }
}