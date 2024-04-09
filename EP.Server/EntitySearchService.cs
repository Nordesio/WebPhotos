using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using EP.Ner;
using System.Xml;
using DbData.Models;
using System.Xml.Linq;
using EP.Server;
using Microsoft.Extensions.DependencyInjection;
using DbData.StorageInterfaces;

namespace EP.Server
{
    public interface IEntitySearchService
    {
        void SearchEntities(Request req);
    }

    public class EntitySearchService : IEntitySearchService
    {
        private readonly IEntityStorage _entityStorage;
        public EntitySearchService(IEntityStorage entityStorage)
        {
            _entityStorage = entityStorage;
        }
        public void SearchEntities(Request req)
        {
            Sdk.Initialize();


            HashSet<string> filterAttributes = new HashSet<string>()
            { "PERSON", "GEO", "STREET", "NAMEDENTITY", "ORGANIZATION", "ADRRESS", "MONEY", "DATE"};



            // Инициализируем список для хранения объектов Entity
            Entity entity = new Entity();
            entity.RequestId = req.Id;
            using (MemoryStream output = new MemoryStream())
            {
                XmlWriter xmlWriter = ProcessStreamHelper.ProcessXml(req.Text, output);
                // Переводим поток в строку, чтобы можно было дальше работать с XML
                // Получаем XML-данные из потока
                byte[] xmlBytes = output.ToArray();
                string xmlOut = Encoding.UTF8.GetString(xmlBytes);
                xmlOut = xmlOut.Remove(0, xmlOut.IndexOf('\n'));
                // Выводим XML-данные
                Console.WriteLine(xmlOut);

                // Разбираем XML-данные и записываем в entities
                var doc = new XmlDocument();
                doc.LoadXml(xmlOut);
                var xmlEntities = doc.GetElementsByTagName("entity");
                foreach (XmlNode xmlEntity in xmlEntities)
                {
                    Console.WriteLine("\n");

                    Console.WriteLine($"Entity type: {xmlEntity.Attributes["type"].Value}");
                    // Выведем все атрибуты для текущего узла
                    var attributes = xmlEntity.SelectNodes("attr");
                    foreach (XmlNode attributeNode in attributes)
                    {
                        Console.WriteLine($"Attribute name: {attributeNode.Attributes["name"].Value}");
                    }
                    // Создаем новый объект Entity

                    switch (xmlEntity.Attributes["type"].Value)
                    {
                        case "GEO":
                            entity.Geo = GetAttributeValue(xmlEntity, "GEO");
                            break;
                        case "DATE":
                            entity.Date = GetAttributeValue(xmlEntity, "DATE");
                            break;
                        case "ORGANIZATION":
                            entity.Organization = GetAttributeValue(xmlEntity, "ORGANIZATION");
                            break;
                        case "NAMEDENTITY":
                            entity.Namedentity = GetAttributeValue(xmlEntity, "NAMEDENTITY");
                            break;
                        case "PERSON":
                            entity.Person = GetAttributeValue(xmlEntity, "PERSON");
                            break;
                        case "STREET":
                            entity.Street = GetAttributeValue(xmlEntity, "STREET");
                            break;
                        case "MONEY":
                            entity.Money = GetAttributeValue(xmlEntity, "MONEY");
                            break;
                        case "ADDRESS":
                            entity.Address = GetAttributeValue(xmlEntity, "ADDRESS");
                            break;
                        default:
                            // В случае несовпадения типа сущности, можно выполнить какие-то действия
                            break;
                    }
                }
                _entityStorage.Insert(entity);
            }
        }

        private string? GetAttributeValue(XmlNode xmlNode, string attributeName)
        {
            if (xmlNode.Attributes["type"].Value.Equals(attributeName))
            {


                var attributes = xmlNode.SelectNodes("attr");
                string text = "";
                foreach (XmlNode attributeNode in attributes)
                {
                    text += attributeNode.Attributes["name"].Value + "; ";
                }
                return text;
            }
            else { return null; }
        }
    }
}
