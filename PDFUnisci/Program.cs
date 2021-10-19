using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using LogManager;

namespace PDFUnisci
{
	class Program
	{
        static void OpenFile(string Filename)
        {
            if (File.Exists(Filename))
            {
                try
                {
                    Process fileopener = new Process();

                    fileopener.StartInfo.FileName = "explorer";
                    fileopener.StartInfo.Arguments = "\"" + Filename + "\"";
                    fileopener.Start();
                }
                catch (Exception e)
                {
                    LogHelper.Log(e.ToString(), LogType.Error);
                }
            }
        }

		static void Main(string[] args)
        {
            List<string> argsL = args.ToList();
            List<string> Files = new List<string>();
            List<string> Images = new List<string>();

            string Cover = null;
            bool flat = false;
            bool splitAll = false;
            bool autoOpenFile = false;
            int singlePageSplit = 0;
            string createNewPageFormat = null;


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
                        case "-np":

                            if (argsL.Count() > i + 1)
                            {
                                createNewPageFormat = argsL[i + 1];
                                i++;
                            } else
                            {
                                createNewPageFormat = "A4";
                            }

                            Config.ExitConfirmation = 0;

                            break;

                        case "-o":
                            autoOpenFile = true;
                            break;

                        case "-b":
                            PDFInterface.Bookmarks = 1;
                            break;

                        case "-s":
                            splitAll = true;
                            break;

                        case "-flat":
                            flat = true;
                            break;

                        case "-singlepagesplit":
                            if (argsL.Count() >= i+1)
                            {
                                bool r = Int32.TryParse(argsL[i + 1].ToLower(), out singlePageSplit);
                                if (!r)
                                {
                                    singlePageSplit = 0;
                                    break;
                                }
                            }
                            i++;
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
            string OutFileNameFinal = "";

            if (Files.Count() == 2)
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
                OutFileNameFinal = $"{OutFileNameImg}_ImgMerged.pdf";
                PDFInterface.ImgToPDF(Images, OutFileNameFinal);
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
            else if (createNewPageFormat != null)
            {
                //Create a new file in the temp folder with random filename
                OutFileNameFinal = System.IO.Path.GetTempPath() + "PDFUnisci_" + Guid.NewGuid().ToString() + ".pdf";

                PDFInterface.CreateEmptyPage(OutFileNameFinal, pageSize: createNewPageFormat);

                LogHelper.Log($"Created a new Empty Page with size: {createNewPageFormat}", LogType.Successful);
                LogHelper.Log($"File location: {OutFileNameFinal}", LogType.Normal);

            }
            else
            {
                switch (Files.Count)
                {
                    case 0:
                        break;
                    case 1:
                        if (Cover == null) PDFInterface.SplitPDF(Files.FirstOrDefault(), $"{OutFileName}_split", singlePageSplit);
                        else {
                            OutFileNameFinal = $"{OutFileName}_merged.pdf";
                            PDFInterface.ReplaceCoverPDF(Files.FirstOrDefault(), Cover, OutFileNameFinal); 
                        }
                        break;
                    default:

                        if (splitAll)
                        {
                            foreach (string file in Files)
                            {
                                OutFileName = $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(file)}";
                                PDFInterface.SplitPDF(file, $"{OutFileName}_split", singlePageSplit);
                            }
                        }
                        else
                        {
                            OutFileNameFinal = $"{OutFileName}_merged.pdf";
                            PDFInterface.MergePDF(Files, OutFileNameFinal);
                        }
                        break;
                }
            }

            if (autoOpenFile)
            {
                LogHelper.Log($"File auto open enabled", LogType.Successful);
                OpenFile(OutFileNameFinal);
            }

			if (Config.ExitConfirmation == 1)
			{
				Console.Write("\nPress enter to close...");
				Console.Read();
			}
        }
    }
}