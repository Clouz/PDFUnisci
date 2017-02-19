using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

using LogManager;
using System.Linq;

namespace PDFUnisci
{
    public static class PDFInterface
    {
        static int _DefaultDigit = 3;

        public static int DefaultDigit
        { 
            get
            {
                return _DefaultDigit;
            }

            set
            {
                if (value >= 10) _DefaultDigit = 10;
                else if (value <= 1) _DefaultDigit = 1;
                else _DefaultDigit = value;
            } 
        }

        static int _CoverFunction = 1;
        public static int CoverFunction 
        { 
            get
            {
                return _CoverFunction;
            }

            set
            { 
                if(value >= 1) _CoverFunction = 1;
                else _CoverFunction = 0;
            } 
        }
               
        public static void MergePDF(List<string> files, string OutFile)
        {
            LogHelper.Log("Unisco tutti i file in un unico PDF", LogType.Successful);

            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                foreach (string file in files)
                {
                    //todo: approfondire crash
                    LogHelper.Log($"Aggiungo il file: {file}");
                    pdf.AddDocument(new iTextSharp.text.pdf.PdfReader(file));
                }
            }
        }

        public static void ReplaceCoverPDF(string InFile, string InCover, string OutFile)
        {
            if(CoverFunction == 0) 
            {
                List<string> Files = new List<string>
                {
                    InFile,
                    InCover
                };
                Files.Sort();
                MergePDF(Files, OutFile);
                return;
            }

            LogHelper.Log("Sostituisco la cover al file originario", LogType.Successful);

            //Apro il file della cover
            using (FileStream stream = new FileStream(OutFile, FileMode.Create))
            using (Document doc = new Document())
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                //Aggiungo la cover
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InCover);
                int coverPage = reader.NumberOfPages;

                LogHelper.Log($"Aggiungo la cover: {InCover} di {coverPage} pagine");
                pdf.AddDocument(reader);
                reader.Close();

                //Aggiungo il resto del documento
                reader = new iTextSharp.text.pdf.PdfReader(InFile);
                int count = reader.NumberOfPages;
                coverPage++;
                List<int> pages = Enumerable.Range(coverPage, count - coverPage + 1).ToList();

                LogHelper.Log($"Aggiungo il file: {InFile} da pagina: {coverPage}");
                pdf.AddDocument(reader, pages);

                reader.Close();
            }
        }

        public static void SplitPDF(string InFiles, string OutDir)
        {
            string outFiles = OutDir + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles);

            LogHelper.Log($"Creo la directory in: {OutDir}");
            Directory.CreateDirectory(OutDir);

            LogHelper.Log("Divido il file in tanti PDF singoli", LogType.Successful);

            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InFiles))
            {
                int NumPages = reader.NumberOfPages;

                int digitN = NumPages.ToString().Length;
                if (digitN < DefaultDigit) digitN = (int)DefaultDigit;

                for (int i = 1; i <= NumPages; i++)
                {
                    string outFile = string.Format("{0}_Page {1:D" + digitN + "}.pdf", outFiles, i);
                    FileStream stream = new FileStream(outFile, FileMode.Create);

                    LogHelper.Log($"Pagina: {Path.GetFileNameWithoutExtension(outFile)}");
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