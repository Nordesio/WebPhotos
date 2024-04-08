using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EP.Ner;
using Pullenti.Ner;
using Processor = EP.Ner.Processor;
using ProcessorService = EP.Ner.ProcessorService;
using SourceOfAnalysis = EP.Ner.SourceOfAnalysis;
using Referent = EP.Ner.Referent;

namespace EP.DemoServer
{
    static class ProcessStreamHelper
    {
        public static void ProcessXml(string input, Stream output)
        {
            // извлекаем текст из потока
            string txt = input;

           
            

            // обрабатываем текст
            Processor proc = ProcessorService.CreateProcessor();
            var ar = proc.Process(new SourceOfAnalysis(txt));

            XmlWriterSettings ws = new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true };
            using (XmlWriter xml = XmlWriter.Create(output, ws))
            {
                xml.WriteStartDocument();
                if (string.IsNullOrEmpty(txt) || ar == null)
                    xml.WriteElementString("error", "empty text");
                else
                {
                    xml.WriteStartElement("result");
                    xml.WriteAttributeString("version", ProcessorService.Version.ToString());
                    xml.WriteAttributeString("lang", ar.BaseLanguage.ToString());
                    xml.WriteAttributeString("chars", txt.Length.ToString());

                    HashSet<string> filterAttributes = new HashSet<string>()
                     { "PERSON", "GEO", "STREET", "NAMEDENTITY", "ORGANIZATION", "ADRRESS", "MONEY", "DATE"};

                    string type_flag = ""; // для отслеживания смены типа сущностей
                    int i = 0;

                    foreach (Referent e in ar.Entities)
                    {
                        if (i == 0)
                        {
                            type_flag = e.TypeName;
                            xml.WriteStartElement("entity");
                            xml.WriteAttributeString("type", e.TypeName);
                            i = 1;
                        }
                        if (filterAttributes.Contains(e.TypeName))
                        {

                            if (type_flag != e.TypeName)

                            {
                                xml.WriteEndElement();
                                xml.WriteStartElement("entity");
                                xml.WriteAttributeString("type", e.TypeName);
                            }

                            type_flag = e.TypeName;
                            xml.WriteStartElement("attr");
                            xml.WriteAttributeString("name", e.ToString());
                            xml.WriteEndElement();

                        }
                    }

                    xml.WriteEndElement();

                    xml.WriteEndElement();
                }
                xml.WriteEndDocument();
            }
        }

        // к сожалению, приходится корректировать строку, а то разное бывает, что портит XML
        static string _corrXmlString(string txt)
        {
            if (txt == null) return "";
            foreach (var c in txt)
                if ((int)c < 0x20 && c != '\r' && c != '\n' && c != '\t')
                {
                    StringBuilder tmp = new StringBuilder(txt);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        char ch = tmp[i];
                        if ((int)ch < 0x20 && ch != '\r' && ch != '\n' && ch != '\t')
                            tmp[i] = ' ';
                    }
                    return tmp.ToString();
                }
            return txt;
        }

    }
}
