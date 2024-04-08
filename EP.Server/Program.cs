using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using EP.Ner;
using System.Xml;

namespace EP.DemoServer
{


    public class TextEntity
    {
       
    }
    /// <summary>
    /// Пример обработчика, которых работает под MONO в других операционных системах
    /// (Linux, Mac, Android)
    /// Работает как в пакетном режиме, так и в режиме обработки TCP-сервера.
    /// На входе получает текст UTF-8, на выходе формирует XML.
    /// Проект можно использовать как пример для создания своих обработчиков.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            Sdk.Initialize();
            //Console.WriteLine(" OK ");
            


            ProcessStreamHelper.ProcessXml("Б-р Новосондецкий 11, кв-4", null);
            

            
            //while (true)
            //{
            //    ConsoleKeyInfo kki = Console.ReadKey(false);
            //    //if (kki.KeyChar == 0x1B)
            //    if (kki.KeyChar != 0x0)
            //        break;
            //    System.Threading.Thread.Sleep(1000);
            //}

            //Console.Write("\r\nStop server ");
            //try
            //{
            //    if (m_Thread != null)
            //        m_Thread.Abort();
            //}
            //catch { }
            //try
            //{
                
            //    Console.WriteLine("OK");
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(" <- ERROR: {0} ", ex.Message);
            //}
            //Console.WriteLine(" Good bye!");
            //бд
            //TextMetadata md = new TextMetadata();

            HashSet<string> filterAttributes = new HashSet<string>()
            { "PERSON", "GEO", "STREET", "NAMEDENTITY", "ORGANIZATION", "ADRRESS", "MONEY", "DATE"};


            //string xmlOut = Encoding.UTF8.GetString(res);
            //// doc.LoadXml(xmlOut) ругается на первую строчку в XML, просто удаляем ее
            //xmlOut = xmlOut.Remove(0, xmlOut.IndexOf('\n'));
            //var doc = new System.Xml.XmlDocument();
            //Console.WriteLine(xmlOut);
            //doc.LoadXml(xmlOut);
            //// TODO разобрать xmlOut, вытащить из него список сущностей и записать в md.entities
            //var entities = doc.GetElementsByTagName("entity");

            //foreach (XmlNode entity in entities)
            //{
            //    if (filterAttributes.Contains(entity.Attributes["type"].Value))
            //    {
            //        //класс из бд
            //        //TextEntity te = new TextEntity();

            //        foreach (XmlNode child in entity.ChildNodes)
            //        {
            //            //Text entity это моя таблица в бд
            //            te.name = entity.Attributes["type"].Value;
            //            te.values.Add(child.Attributes["name"].Value);
            //        }
            //        //запись в бд
            //        md.entities.Add(te);

            //    }

            //    //md.entities.Add(new TextEntity()
            //    //{
            //    //    name = entity.Attributes["type"].Value,
            //    //    values = values
            //    //});
            //}

        }

        
        static Thread m_Thread;

        /// <summary>
        /// Обработчик TCP-запросов
        /// </summary>
       
        static bool m_Break;
        static void _keyPress()
        {
            m_Break = false;
            while (!m_Break)
            {
                ConsoleKeyInfo kki = Console.ReadKey(false);
                if (kki.KeyChar == 0x1B) m_Break = true;
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
