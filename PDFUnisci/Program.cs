using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using LogManager;

namespace PDFUnisci
{
	class Program
	{
        static void StampaContenuto(List<string> Files, string Cover) {
            if(Cover != null)
                LogHelper.Log($"Cover: {Cover}");

            foreach (var file in Files)
            {
                LogHelper.Log($"File: {file}");
            }
        }

		static void Main(string[] args)
        {
            List<string> Files = new List<string>();
            string Cover = null;

            //leggo il file di configurazione
			Config.LeggiXML();

            foreach (var file in args)
            {
                if (System.IO.File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf") 
                    Files.Add(file);
                else 
                    LogHelper.Log($"Il file selezionato non esiste o non è un pdf, quindi verrà escluso. {file}",LogType.Error);
            }

            Files.Sort();

            if(Files.Count() == 2)
            {
                Cover = Files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains("cover"));

                if (Cover != null) 
                    Files.Remove(Cover);
            }

            string OutFile = $"{Path.GetDirectoryName(Files.FirstOrDefault())}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(Files.FirstOrDefault())}";

            //basandomi sulla quantità di file nella lista scelgo un azione da compiere
            switch (Files.Count)
            {
                case 0:
                    Menu.Create();
                    break;
                case 1:
                    if(Cover == null) PDFInterface.SplitPDF(Files.FirstOrDefault(), OutFile + "_split");
                    else PDFInterface.ReplaceCoverPDF(Files.FirstOrDefault(), Cover, OutFile + "_merged.pdf");
                    break;
                default:
                    PDFInterface.MergePDF(Files, OutFile + "_merged.pdf");
                    break;
            }

			if (Config.ExitConfirmation == 1)
			{
				Console.WriteLine("\nPremere invio per chiudere");
				Console.Read();
			}
        }
    }
}