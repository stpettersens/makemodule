using System;
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace makemodule
{
    class MakeModule
    {
        private static ArrayList names;
        private static ArrayList enabled;

        static void Main(string[] args)
        {
            names = new ArrayList();
            enabled = new ArrayList();

            if (args.Length == 0)
            {
                Console.WriteLine("\nmakemodule utility");
                Console.WriteLine("Copyright (c) 2015 Sam Saint-Pettersen.");
                Console.WriteLine("Released under the MIT/X11 License.");
                Console.WriteLine("\nmakemodule [module..module]");
                Environment.Exit(1);
            }

            foreach(string arg in args)
            {
                names.Add(arg);

                Process p = new Process();
                p.StartInfo.FileName = arg;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;

                try
                {
                    p.Start();

                    p.WaitForExit();
                    if (p.ExitCode != 0) enabled.Add(true);
                    else enabled.Add(false);
                }
                catch(Win32Exception)
                {
                    enabled.Add(false);
                }
            }

            if(names.Count > 0) writeModuleXML();
        }

        static void writeModuleXML()
        {
            XmlDocument modulesDoc = new XmlDocument();
            XmlNode root = modulesDoc.CreateElement("configuration");
            modulesDoc.AppendChild(root);

            for(int i = 0; i < names.Count; i++)
            {
                XmlNode moduleNode = modulesDoc.CreateElement("module");
                XmlNode nameNode = modulesDoc.CreateElement("name");
                nameNode.InnerText = names[i].ToString();
                XmlNode enabledNode = modulesDoc.CreateElement("enabled");
                enabledNode.InnerText = enabled[i].ToString();
                moduleNode.AppendChild(nameNode);
                moduleNode.AppendChild(enabledNode);
                root.AppendChild(moduleNode);
            }

            Console.WriteLine("Writing modules.xml...");
            modulesDoc.Save("modules.xml");

            StreamReader sr = new StreamReader("modules.xml");
            ArrayList lines = new ArrayList();
            while(!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }
            sr.Close();
            StreamWriter sw = new StreamWriter("modules.xml", false, Encoding.UTF8);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            foreach(string line in lines)
            {
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}