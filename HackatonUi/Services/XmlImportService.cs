using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.Services
{
    public class XmlImportService
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly BuildingAttributeRepository _attributeRepo;
        private readonly BuildingDocumentRepository _docRepo;
        private readonly ControlDateRepository _controlDateRepo;

        public XmlImportService(string connectionString)
        {
            _buildingRepo = new BuildingRepository(connectionString);
            _attributeRepo = new BuildingAttributeRepository(connectionString);
            _docRepo = new BuildingDocumentRepository(connectionString);
            _controlDateRepo = new ControlDateRepository(connectionString);
        }

        public void ImportBuildingsFromXml(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(BuildingXmlList));
                using var stream = new FileStream(filePath, FileMode.Open);
                var xmlData = (BuildingXmlList)serializer.Deserialize(stream)!;

                foreach (var dto in xmlData.Items)
                {
                    // Создание здания
                    var building = new Building
                    {
                        Address = dto.Address,
                        CadastralNumber = dto.CadastralNumber,
                        Floors = dto.Floors,
                        Area = dto.Area,
                        BuildingTypeId = dto.BuildingTypeId
                    };

                    _buildingRepo.Add(building);
                    var realId = building.Id;

                    // Добавление атрибутов
                    foreach (var attr in dto.Attributes)
                    {
                        var buildingAttribute = new BuildingAttribute
                        {
                            BuildingId = realId,
                            Section = attr.Section,
                            Key = attr.Key,
                            Value = attr.Value
                        };
                        _attributeRepo.AddAttribute(buildingAttribute);
                    }

                    // Добавление документов
                    foreach (var doc in dto.Documents)
                    {
                        var buildingDocument = new BuildingDocument
                        {
                            BuildingId = realId,
                            FilePath = doc.Path,
                            UploadedBy = doc.Uploader,
                            UploadedAt = doc.Date
                        };
                        _docRepo.AddDocument(buildingDocument);
                    }

                    // Добавление контрольных дат
                    foreach (var cd in dto.ControlDates)
                    {
                        var controlDate = new ControlDate
                        {
                            BuildingId = realId,
                            Title = cd.Title,
                            DueDate = cd.DueDate,
                            IsDone = false
                        };
                        _controlDateRepo.AddControlDate(controlDate);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при импортировании XML файла: " + ex.Message);
            }
        }
    }
}
