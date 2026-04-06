using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HackatonUi.Models;

    [XmlRoot("Buildings")]
    public class BuildingXmlList
    {
        [XmlElement("Building")]
        public List<BuildingXmlDto> Items { get; set; } = new();
    }

    public class BuildingXmlDto
    {
        public string Address { get; set; } = null!;
        public int CadastralNumber { get; set; }
        public int Floors { get; set; }
        public double Area { get; set; }
        public int BuildingTypeId { get; set; }

            [XmlArray("Attributes")]
            [XmlArrayItem("Attribute")]
        public List<BuildingAttributeXml> Attributes { get; set; } = new();

        [XmlArray("Documents")]
        [XmlArrayItem("Document")]
        public List<BuildingDocumentXml> Documents { get; set; } = new();

        [XmlArray("ControlDates")]
        [XmlArrayItem("ControlDate")]
        public List<ControlDateXml> ControlDates { get; set; } = new();
    }

    public class BuildingAttributeXml
    {
        [XmlAttribute("section")]
        public string Section { get; set; } = "";

        [XmlAttribute("key")]
        public string Key { get; set; } = "";

        [XmlAttribute("value")]
        public string Value { get; set; } = "";
    }

    public class BuildingDocumentXml
    {
        [XmlAttribute("path")]
        public string Path { get; set; } = null!;

        [XmlAttribute("uploader")]
        public string Uploader { get; set; } = null!;

        [XmlAttribute("date")]
        public DateTime Date { get; set; }
    }

    public class ControlDateXml
    {
        [XmlAttribute("title")]
        public string Title { get; set; } = null!;

        [XmlAttribute("dueDate")]
        public DateTime DueDate { get; set; }
    }