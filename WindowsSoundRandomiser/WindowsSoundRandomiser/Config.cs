using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace WindowsSoundRandomiser
{
    class Config
    {
        private const string registryBasePath = @"AppEvents\Schemes\Apps\.Default\";
        private const string filename = "config.xml";

        private Config() { }

        public static void CreateConfigFile()
        {
            XmlDocument config = new XmlDocument();

            try
            {
                config.Load(filename);
            }
            catch
            {
                XmlElement root = config.CreateElement("Config");

                XmlAttribute path = config.CreateAttribute("BasePath");
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                path.Value = basePath;

                root.Attributes.Append(path);

                XmlAttribute regPath = config.CreateAttribute("RegistryBasePath");
                regPath.Value = registryBasePath;

                root.Attributes.Append(regPath);

                config.AppendChild(root);

                config.Save(filename);
            }
        }

        public static void SetBasePath(string newPath)
        {
            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(filename);

                config.DocumentElement.SetAttribute("BasePath", newPath);
            }
            catch (Exception e)
            {
                throw new Exception("File could not be loaded! " + e.Message);
            }
        }

        public static string GetBasePath()
        {
            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(filename);

                return config.DocumentElement.GetAttribute("BasePath");
            }
            catch (Exception e)
            {
                throw new Exception("File could not be loaded! " + e.Message);
            }
        }

        public static void SetRegistryBasePath(string newPath)
        {
            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(filename);

                config.DocumentElement.SetAttribute("RegistryBasePath", newPath);
            }
            catch (Exception e)
            {
                throw new Exception("File could not be loaded! " + e.Message);
            }
        }

        public static string GetRegistryBasePath()
        {
            try
            {
                XmlDocument config = new XmlDocument();
                config.Load(filename);

                return config.DocumentElement.GetAttribute("RegistryBasePath");
            }
            catch (Exception e)
            {
                throw new Exception("File could not be loaded! " + e.Message);
            }
        }
    }
}
