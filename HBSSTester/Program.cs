using System;
using SSBHLib;
using SSBHLib.Formats;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using SSBHLib.Formats.Animation;
using SSBHLib.Tools;

namespace HBSSTester
{
    class Program
    {
        static void Main(string[] args)
        {
            /*string[] files = Directory.GetFiles("", "*.numshb*", SearchOption.AllDirectories);

            List<string> ErrorReading = new List<string>();
            List<string> VertexAttributes = new List<string>();
            int Unk8 = 0;
            int Bid = 0;
            int VersionM = 0;
            int Versionm = 0;
            foreach(string s in files)
            {
                ISSBH_File File;
                try
                {
                    if (SSBH.TryParseSSBHFile(s, out File))
                    {
                        if (File is MESH)
                        {
                            VersionM = Math.Max(VersionM, ((MESH)File).VersionMajor);
                            Versionm = Math.Max(Versionm, ((MESH)File).VersionMinor);
                            foreach (MESH_Object o in ((MESH)File).Objects)
                            {
                                Unk8 = Math.Max(Unk8, o.Unk8);
                                Bid = Math.Max(Bid, o.BID);
                                foreach (MESH_Attribute a in o.Attributes)
                                {
                                    if (!VertexAttributes.Contains(a.AttributeStrings[0].Name))
                                        VertexAttributes.Add(a.AttributeStrings[0].Name);
                                }
                            }
                        }
                    }
                }
                catch(Exception)
                {
                    ErrorReading.Add(s);
                }
                
            }

            StreamWriter w = new StreamWriter(new FileStream("outmsh.txt", FileMode.Create));

            w.WriteLine("Unk8 " + Unk8.ToString("X"));
            w.WriteLine("M " + VersionM.ToString("X"));
            w.WriteLine(", " + Versionm.ToString("X"));
            w.WriteLine("BID " + Bid.ToString("X"));
            w.WriteLine("Attributes: ");
            foreach(string s in VertexAttributes)
            {
                w.WriteLine(s);
            }

            w.WriteLine("Errors: ");
            foreach (string s in ErrorReading)
            {
                w.WriteLine(s);
            }

            w.Close();*/

            SsbhFile File;
            if (Ssbh.TryParseSsbhFile(Directory.GetCurrentDirectory() + "//" + args[0], out File))
            {
                if (File is Anim anim)
                {
                    var decoder = new SsbhAnimTrackDecoder(anim);

                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace
                    };

                    string FileName = Directory.GetCurrentDirectory() + "//" + Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]) + ".xml";

                    XmlWriter o = XmlWriter.Create(new FileStream(FileName, FileMode.Create), settings);
                    o.WriteStartDocument();

                    o.WriteStartElement("NamcoAnimation");
                    
                    foreach (var an in anim.Animations)
                    {
                        o.WriteStartElement("Animation");
                        o.WriteAttributeString("Type", an.Type.ToString());
                        foreach (var node in an.Nodes)
                        {
                            o.WriteStartElement("Node");
                            o.WriteAttributeString("Name", node.Name);
                            foreach (var track in node.Tracks)
                            {
                                o.WriteStartElement("Track");
                                o.WriteAttributeString("Name", track.Name);
                                o.WriteAttributeString("FrameCount", track.FrameCount.ToString());
                                //o.WriteAttributeString("Flags", track.Flags.ToString("X"));

                                var values = decoder.ReadTrack(track);

                                if(values != null && values.Length > 0)
                                o.WriteAttributeString("Type", values[0].GetType().Name);

                                for (int i = 0; i < values.Length; i++)
                                {
                                    o.WriteStartElement("Key");
                                    o.WriteAttributeString("Frame", (i+1).ToString());
                                    o.WriteString(values[i].ToString());
                                    o.WriteEndElement();
                                }
                                o.WriteEndElement();
                            }
                            o.WriteEndElement();
                        }
                        o.WriteEndElement();
                    }

                    o.WriteEndElement();
                    o.Close();
                }
            }

            /*ISSBH_File File;
            if(SSBH.TryParseSSBHFile("", out File))
            {
                ExportFileAsXML("Test.xml", File);
            }*/
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
