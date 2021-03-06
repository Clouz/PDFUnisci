﻿using System;
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
            List<string> argsL = args.ToList();
            List<string> Files = new List<string>();
            List<string> Images = new List<string>();

            string Cover = null;
            bool flat = false;


            //leggo il file di configurazione
            Config.LeggiXML();

            for (int i = 0; i < argsL.Count; i++)
            {
                if (File.Exists(argsL[i]))
                {
                    if (Path.GetExtension(argsL[i]).ToLower() == ".pdf")
                    {
                        Files.Add(argsL[i]);
                    }
                    else if (Path.GetExtension(argsL[i]).ToLower() == ".png" || Path.GetExtension(argsL[i]).ToLower() == ".jpg" || Path.GetExtension(argsL[i]).ToLower() == ".jpeg")
                    {
                        Images.Add(argsL[i]);
                    }
                    else
                    {
                        LogHelper.Log($"The selected file is not a PDF or valid immage format (.png | .jpg | .jpeg), and will be excluded. {argsL[i]}", LogType.Warning);
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

                        case "-flat":
                            flat = true;
                            break;

                        default:
                            LogHelper.Log($"The argument option does not exist will be excluded. {argsL[i]}", LogType.Error);
                            break;
                    }
                }
            }

            Files.Sort();
            Images.Sort();

            string OutFileName = $"{Path.GetDirectoryName(Files.FirstOrDefault())}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(Files.FirstOrDefault())}";
            string OutFileNameImg = $"{Path.GetDirectoryName(Images.FirstOrDefault())}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(Images.FirstOrDefault())}";

            if(Files.Count() == 2)
            {
                Cover = Files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains("cover"));

                if (Cover != null) 
                    Files.Remove(Cover);
            }

            if (args.Length == 0)
            {
                Menu.Create();
            }
            else if (Images.Count > 0)
            {
                PDFInterface.ImgToPDF(Images, $"{OutFileNameImg}_ImgMerged.pdf");
            }
            else if(flat)
            {
                if (PDFInterface.FlatOnlyFirstPage == 1)
                {
                    PDFInterface.FlatPDFonlyFistPage(Files);
                }
                else
                {
                    PDFInterface.FlatPDF(Files);
                }
            }
            else
            {
                switch (Files.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if(Cover == null) PDFInterface.SplitPDF(Files.FirstOrDefault(), $"{OutFileName}_split");
                        else PDFInterface.ReplaceCoverPDF(Files.FirstOrDefault(), Cover, $"{OutFileName}_merged.pdf");
                        break;
                    default:
                        PDFInterface.MergePDF(Files, $"{OutFileName}_merged.pdf");
                        break;
                }
            }

			if (Config.ExitConfirmation == 1)
			{
				Console.Write("\nPress enter to close...");
				Console.Read();
			}
        }
    }
}