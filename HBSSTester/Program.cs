using System;
using SSBHLib;
using SSBHLib.Formats;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ANIMCMD;
using ANIMCMD.Simulator;

namespace HBSSTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ACMD_Runner_Pseudo runner = new ACMD_Runner_Pseudo();
            runner.SetScript(@"app::sv_animcmd::frame(2);
if(app::sv_animcmd::is_excute())
{
	app::FighterWorkModuleImpl::method_23(0x2100000C);
}
app::sv_animcmd::frame(7);
if(app::sv_animcmd::is_excute())
{
	app::sv_animcmd::ATTACK(0, 0, 0, 11.0, 80, 100, 0, 20, 4.2, 4.4, -1.0, 0.0, 2.4, -1.0, -3.5, 1.1, 1.0, 1, 0, 0, 0, 0.0, 0, 0, 0, 0, 0, 1, 3u, 63, 31, 0, null, 2, 2, 1);
}
app::sv_animcmd::frame(8);
if(app::sv_animcmd::is_excute())
{
	app::sv_animcmd::ATTACK(1, 0, 0, 11.0, 80, 100, 0, 20, 3.2, -0.4, -1.0, -7.0, -4.4, -1.0, -5.0, 1.1, 1.0, 1, 0, 0, 0, 0.0, 0, 0, 0, 0, 0, 1, 3u, 63, 31, 0, null, 2, 2, 1);
}
app::sv_animcmd::frame(11);
if(app::sv_animcmd::is_excute())
{
    app::FighterAttackModuleImpl::method_A();
}
app::sv_animcmd::FT_MOTION_RATE(0.85);
app::sv_animcmd::frame(40);
if(app::sv_animcmd::is_excute())
{
	app::FighterWorkModuleImpl::method_25(0x2100000C);
}");
            runner.SetFrame(8);
            Console.WriteLine(runner.GetHitbox(0).Size);
            Console.ReadLine();

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

            /* ISSBH_File File;
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
