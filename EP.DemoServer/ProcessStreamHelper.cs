using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EP.Ner;

namespace EP.DemoServer
{
    static class ProcessStreamHelper
    {
        public static void ProcessXml(Stream input, Stream output)
        {
            // извлекаем текст из потока
            string txt = null;
            byte[] buf = new byte[100000];
            using (MemoryStream mem = new MemoryStream())
            {
                while (true)
                {
                    int i = input.Read(buf, 0, buf.Length);
                    if (i <= 0) break;
                    mem.Write(buf, 0, i);
                }
                mem.Position = 0;
                if (mem.Length > 0)
                    txt = Encoding.UTF8.GetString(mem.ToArray());
            }

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
