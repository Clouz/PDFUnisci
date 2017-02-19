using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using LogManager;

namespace PDFUnisci
{
	class Program
	{
		static void Main(string[] args)
        {
            List<string> Files = new List<string>();
            string Cover = null;

            //leggo il file di configurazione
			Config.LeggiXML();

            foreach (var file in args)
            {
                if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf") 
                    Files.Add(file);
                else 
                    LogHelper.Log($"Il file selezionato non esiste o non è un pdf, quindi verrà escluso. {file}", LogType.Error);
            }

            Files.Sort();

            if(Files.Count() == 2)
            {
                Cover = Files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains("cover"));

                if (Cover != null) 
                    Files.Remove(Cover);
            }

            string OutFileName = $"{Path.GetDirectoryName(Files.FirstOrDefault())}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(Files.FirstOrDefault())}";

            switch (Files.Count)
            {
                case 0:
                    Menu.Create();
                    break;
                case 1:
                    if(Cover == null) PDFInterface.SplitPDF(Files.FirstOrDefault(), $"{OutFileName}_split");
                    else PDFInterface.ReplaceCoverPDF(Files.FirstOrDefault(), Cover, $"{OutFileName}_merged.pdf");
                    break;
                default:
                    PDFInterface.MergePDF(Files, $"{OutFileName}_merged.pdf");
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