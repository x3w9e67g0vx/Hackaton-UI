using System.Xml.Serialization;

namespace HackatonUi.Models;

public class BuildingAttribute
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    [XmlAttribute("section")]
    public string Section { get; set; } = "";
    [XmlAttribute("key")]
    public string Key { get; set; } = "";
    [XmlAttribute("value")]
    public string Value { get; set; } = "";
}