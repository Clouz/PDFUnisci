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
                System.Console.WriteLine("Il file config.xml è stato letto correttamente");
            }
            catch (System.IO.FileNotFoundException)
            {
                config = new XDocument(
                    new XComment("PDFUnisci config"),
                    new XElement("config",
                        new XComment("Indica il numero di digit quando si divide un PDF"),
                        new XElement($"{nameof(PDFInterface.DefaultDigit)}", PDFInterface.DefaultDigit),
                        new XComment("Il programma rimane aperto fino alla pressione di un tasto"),
                        new XElement($"{nameof(ExitConfirmation)}", ExitConfirmation),
                        new XComment("Se impostato a zero disattiva la funziona per sostituire la cover"),
                        new XElement($"{nameof(PDFInterface.CoverFunction)}", PDFInterface.CoverFunction)));
                config.Declaration = new XDeclaration("1.0", "utf-8", "true");
                config.Save(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.xml");

                System.Console.WriteLine("Il file config.xml non è stato trovato, verrà creato");
            }

            
            //Leggo il file xml
            PDFInterface.DefaultDigit = ReadConfig(PDFInterface.DefaultDigit, nameof(PDFInterface.DefaultDigit));
            ExitConfirmation = ReadConfig(ExitConfirmation, nameof(ExitConfirmation));
            PDFInterface.CoverFunction = ReadConfig(PDFInterface.CoverFunction, nameof(PDFInterface.CoverFunction));
        }

        static int ReadConfig(int option, string nameof)
        {
            int i;
            if (int.TryParse(config.Elements("config").Select(o => o.Element($"{nameof}").Value).First(), out i)) return i;
            else return option;
        }
    }
}
