using System;
using SSBHLib;
using SSBHLib.Formats;
using System.Xml;
using System.IO;
using System.Reflection;

namespace HBSSTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ISSBH_File File;
            if(SSBH.TryParseSSBHFile("", out File))
            {
                ExportFileAsXML("Test.xml", File);
            }
            //Console.ReadLine();
        }

        public static void ExportFileAsXML(string Filename, object Ob)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            XmlWriter o = XmlWriter.Create(new FileStream(Filename, FileMode.Create), settings);
            o.WriteStartDocument();

            o.WriteStartElement("m", "material", "urn:material");

            WriteObjectToXML(o, Ob);

            o.WriteEndElement();
            o.Close();
        }

        private static void WriteObjectToXML(XmlWriter Writer, object Object)
        {
            PropertyInfo[] Fields = Object.GetType().GetProperties();

            foreach (PropertyInfo Field in Fields)
            {
                if (Field == null) continue;
                if (Field.PropertyType.IsArray)
                {
                    Writer.WriteStartElement("Array", "");
                    Writer.WriteAttributeString("Name", Field.Name);
                    if (Field.PropertyType.GetElementType().IsPrimitive)
                    {
                        if(Field.PropertyType.GetElementType() == typeof(float))
                        foreach (object v in (float[])Field.GetValue(Object))
                        {
                                Writer.WriteString(v.ToString() + " ");
                            }
                        if (Field.PropertyType.GetElementType() == typeof(byte))
                            foreach (object v in (byte[])Field.GetValue(Object))
                            {
                                Writer.WriteString(v.ToString() + " ");
                            }
                    } else
                    foreach(object v in (object[])Field.GetValue(Object))
                        {
                            Writer.WriteStartElement("Element", "");
                            Writer.WriteAttributeString("Index", "");
                            WriteObjectToXML(Writer, v);
                            Writer.WriteEndElement();
                        }
                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement(Field.PropertyType.ToString(), "");
                    Writer.WriteAttributeString("Name", Field.Name);
                    Writer.WriteString(Field.GetValue(Object).ToString());
                    Writer.WriteEndElement();
                }
            }
        }
    }
}
