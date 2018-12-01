using System;
using SSBHLib;
using SSBHLib.Formats;

namespace HBSSTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ISSBH_File File;
            if(SSBH.TryParseSSBHFile("", out File))
            {
                Console.WriteLine(File.GetType());
                MESH Model = (MESH)File;
                Console.WriteLine(Model.HeaderFloats.Length);

                foreach (var e in Model.VertexBuffers)
                {
                    Console.WriteLine(e.Buffer.Length);
                }
            }
            Console.ReadLine();
        }
    }
}
