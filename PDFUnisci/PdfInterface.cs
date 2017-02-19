using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

using LogManager;

namespace PDFUnisci
{
    public static class PdfInterface
    {
        static int DefaultDigit = 3;
        static private void UnisciPDF(List<string> files, string OutFile)
        {
            LogHelper.Log(LogTarget.Console, LogType.Successful, "Unisco tutti i file in un unico PDF");

            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                foreach (string file in files)
                {
                    LogHelper.Log(LogTarget.Console, LogType.Normal, $"Apro il file: {file}");
                    pdf.AddDocument(new iTextSharp.text.pdf.PdfReader(file));
                }

                doc.Close();
            }
        }

        static private void DividiPDF(string InFiles, string OutDir)
        {
            string OutFile = OutDir + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles);

            LogHelper.Log(LogTarget.Console, LogType.Normal, $"Creo la directory in: {OutDir}");
            Directory.CreateDirectory(OutDir);

            LogHelper.Log(LogTarget.Console, LogType.Successful, "Divido il file in tanti PDF singoli");

            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InFiles))
            {
                int NumPages = reader.NumberOfPages;

                //todo: da verificare, non mi convince
                int digitN = NumPages;
                if (digitN < DefaultDigit)
                    digitN = DefaultDigit;

                for (int i = 1; i <= NumPages; i++)
                {
                    string outFile = string.Format("{0}_Page {1:D" + digitN + "}.pdf", OutFile, i);
                    FileStream stream = new FileStream(outFile, FileMode.Create);

                    LogHelper.Log(LogTarget.Console, LogType.Normal, $"Apro il file: {InFiles}");
                    Document doc = new Document();
                    PdfCopy pdf = new PdfCopy(doc, stream);

                    doc.Open();
                    PdfImportedPage page = pdf.GetImportedPage(reader, i);
                    pdf.AddPage(page);

                    pdf.FreeReader(reader);
                    pdf.Close();
                    stream.Close();
                }

                reader.Close();
            }
        }


    }

}