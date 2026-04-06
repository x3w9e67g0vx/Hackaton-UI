using System.Collections.Generic;
using HackatonUi.Models;
namespace HackatonUi.Models;

public class Building
{
    public int Id { get; set; }
    public string? Address { get; set; } = null!;
    public int CadastralNumber { get; set; }
    public int Floors { get; set; }
    public double Area { get; set; }
    public int BuildingTypeId { get; set; }
    
    public List<BuildingAttribute> Attributes { get; set; } = new List<BuildingAttribute>();
    public List<BuildingDocument> Documents { get; set; } = new List<BuildingDocument>();


}