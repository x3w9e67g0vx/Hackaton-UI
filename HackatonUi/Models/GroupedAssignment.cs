using System.Collections.ObjectModel;

namespace HackatonUi.Models
{
    public class GroupedAssignment
    {
        public string DecisionDescription { get; set; } = "";
        public ObservableCollection<DecisionAssignmentInfo> Assignments { get; init; } = new ObservableCollection<DecisionAssignmentInfo>();
    }
}