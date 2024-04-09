﻿using System;
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
namespace EP.DemoServer
{
        class Program
    {
        static void Main(string[] args)
        {


            Sdk.Initialize();
           

            HashSet<string> filterAttributes = new HashSet<string>()
            { "PERSON", "GEO", "STREET", "NAMEDENTITY", "ORGANIZATION", "ADRRESS", "MONEY", "DATE"};

                // Инициализируем список для хранения объектов Entity
                List<Entity> entities = new List<Entity>();
            using (MemoryStream output = new MemoryStream())
            {

                
                string text = "Правительство России задумало вернуть налог на движимое имущество, который отменили с 1 января 2019 года, еще при предыдущем составе кабмина. Об этом заявил замминистра финансов Алексей Лавров, пишут «Ведомости». Спикер Совета Федерации Валентина Матвиенко напомнила, что ранее предупреждали о негативных последствиях налогового послабления: «Освободить от налога стоило только новое оборудование, приобретаемое компаниями, но нас не послушали, все освободили». В 2019 году Минфин оценил выпадающие доходы в 183 миллиарда рублей. Компенсировать их предполагалось за счет роста цен на крепкий алкоголь, но добиться этого не удалось.";
                //XmlWriter xmlWriter = ProcessStreamHelper.ProcessXml("Б-р Новосондецкий 11, кв-4. Сергей Радонежский. 1000 рублей.", output);
                XmlWriter xmlWriter = ProcessStreamHelper.ProcessXml(text, output);
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
                Entity entity = new Entity();
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
                entities.Add(entity);
                    
            }
                

        }


        private static string? GetAttributeValue(XmlNode xmlNode, string attributeName)
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
