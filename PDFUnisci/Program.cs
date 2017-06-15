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
            List<string> argsL = args.ToList();

            string Cover = null;

            //leggo il file di configurazione
			Config.LeggiXML();

            for (int i = 0; i < argsL.Count; i++)
            {
                if (File.Exists(argsL[i]))
                {
                    //FileAttributes f = File.GetAttributes(file);

                    if (Path.GetExtension(argsL[i]).ToLower() == ".pdf")
                    {
                        Files.Add(argsL[i]);
                    }
                    else
                    {
                        LogHelper.Log($"The selected file is not a PDF, and will be excluded. {argsL[i]}", LogType.Error);
                    }
                }
                else if (Directory.Exists(argsL[i]))
                {
                    foreach (var item in Directory.EnumerateFiles(argsL[i]))
                    {
                        argsL.Add(item);
                    }
                }
                else
                {
                    switch (argsL[i].ToLower())
                    {
                        case "-b":
                            PDFInterface.Bookmarks = 1;
                            break;

                        default:
                            LogHelper.Log($"The argument option does not exist will be excluded. {argsL[i]}", LogType.Error);
                            break;
                    }
                }
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