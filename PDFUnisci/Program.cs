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
                if (file.StartsWith("-"))
                {
                    switch (file.ToLower())
                    {
                        case "-b":
                            PDFInterface.Bookmarks = 1;
                            break;

                        default:
                            LogHelper.Log($"The argument option does not exist will be excluded. {file}", LogType.Error);
                            break;
                    }
                }
                else if (File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf")
                    Files.Add(file);
                else
                    LogHelper.Log($"The selected file does not exist or is not a PDF, and will be excluded. {file}", LogType.Error);
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
				Console.Write("\nPress enter to close...");
				Console.Read();
			}
        }
    }
}