using System;
using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace PDFUnisci
{

    public static class PdfInterface
    {
        static private void UnisciPDF(List<string> files, string OutFile)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Unisco tutti i file in un unico PDF\n");
            Console.ResetColor();

            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                foreach (string file in files)
                {
                    //Verifico se esiste il file e se ha come estensione .pdf
                    if (System.IO.File.Exists(file) && Path.GetExtension(file).ToLower() == ".pdf")
                    {
                        Console.WriteLine($"Apro il file: {file}");
                        pdf.AddDocument(new iTextSharp.text.pdf.PdfReader(file));
                    }
                    else
                    {
                        Console.WriteLine($"{file}\t- File non valido, verrà escluso dal PDF");
                    }
                }

                //se doc non è null lo chiude. (Null-conditional operators ?. ??)
                doc?.Close();
            }
        }

    }

}