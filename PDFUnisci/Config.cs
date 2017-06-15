using System.Linq;
using System.Xml.Linq;
using LogManager;

namespace PDFUnisci
{
    static class Config
    {
        static XDocument config = null;

        static int _ExitConfirmation = 0;
        public static int ExitConfirmation
        { 
            get
            {
                if (LogHelper.ErrorLog.Count() > 0) return 1;
                return _ExitConfirmation;
            }

            set
            { 
                if(value >= 1) _ExitConfirmation = 1;
                else _ExitConfirmation = 0;
            } 
        }

        public static void LeggiXML()
        {
            try
            {
                config = XDocument.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.xml");
                System.Console.WriteLine("The configuration file has been read correctly");
            }
            catch (System.IO.FileNotFoundException)
            {
                config = new XDocument(
                    new XComment("PDFUnisci config"),
                    new XElement("config",
                        new XComment("It indicates the number of digits when you divide a PDF"),
                        new XElement($"{nameof(PDFInterface.DefaultDigit)}", PDFInterface.DefaultDigit),
                        new XComment("The program will remain open until you press any key"),
                        new XElement($"{nameof(ExitConfirmation)}", ExitConfirmation),
                        new XComment("If set to zero disables the function to replace the cover"),
                        new XElement($"{nameof(PDFInterface.CoverFunction)}", PDFInterface.CoverFunction),
                        new XComment("If set to zero disables the function to add Bookmarks when merge PDF"),
                        new XElement($"{nameof(PDFInterface.Bookmarks)}", PDFInterface.Bookmarks)));
                config.Declaration = new XDeclaration("1.0", "utf-8", "true");
                config.Save(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.xml");

                System.Console.WriteLine("The config.xml file is not found, it will create");
            }

            
            //Leggo il file xml
            PDFInterface.DefaultDigit = ReadConfig(PDFInterface.DefaultDigit, nameof(PDFInterface.DefaultDigit));
            ExitConfirmation = ReadConfig(ExitConfirmation, nameof(ExitConfirmation));
            PDFInterface.CoverFunction = ReadConfig(PDFInterface.CoverFunction, nameof(PDFInterface.CoverFunction));
            PDFInterface.Bookmarks = ReadConfig(PDFInterface.Bookmarks, nameof(PDFInterface.Bookmarks)); ;
        }

        static int ReadConfig(int option, string nameof)
        {
            int i;
            if (int.TryParse(config.Elements("config").Select(o => o.Element($"{nameof}").Value).First(), out i)) return i;
            else return option;
        }
    }
}
