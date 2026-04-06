using System;
using System.Collections.Generic;

namespace HackatonUi.Models
{
    public class InspectionTask
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Коллекция решений, привязанных к задаче.
        public List<TaskDecision> Decisions { get; set; } = new List<TaskDecision>();
        
        // Для удобства удаления решения (например, если пользователь выбирает решение)
        public TaskDecision? SelectedDecision { get; set; }
    }
}