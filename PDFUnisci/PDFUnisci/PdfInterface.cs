using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

using LogManager;

namespace PDFUnisci
{

    public static class PdfInterface
    {
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

    }

}