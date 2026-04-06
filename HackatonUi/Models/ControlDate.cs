using System;
using System.Xml.Serialization;

namespace HackatonUi.Models;

public class ControlDate
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    [XmlAttribute("title")]
    public string Title { get; set; } = null!;
    [XmlAttribute("dueDate")]
    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }
}