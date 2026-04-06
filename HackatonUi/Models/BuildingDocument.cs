using System;
using System.Xml.Serialization;

namespace HackatonUi.Models;
[XmlRoot("Buildings")]
public class BuildingDocument
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    [XmlAttribute("path")]
    public string FilePath { get; set; } = null!;
    
    [XmlAttribute("date")]
    
    public DateTimeOffset UploadedAt { get; set; }
    [XmlAttribute("uploader")]
    public string UploadedBy { get; set; } = null!;
}