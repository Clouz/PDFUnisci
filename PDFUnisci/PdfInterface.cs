using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

using LogManager;
using System.Linq;
using System;

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

        static int _Bookmarks = 0;
        public static int Bookmarks
        {
            get
            {
                return _Bookmarks;
            }

            set
            {
                if (value >= 1) _Bookmarks = 1;
                else _Bookmarks = 0;
            }
        }

        public static void MergePDF(List<string> files, string OutFile)
        {

            LogHelper.Log("I join all files into a single PDF", LogType.Successful);

            FileStream stream = null;
            Document doc = null;
            PdfCopy pdf = null;

            try
            {
                stream = new FileStream(OutFile, FileMode.Create);
                doc = new Document();
                pdf = new PdfCopy(doc, stream);

                doc.Open();

                foreach (string file in files)
                {
                    LogHelper.Log($"Add the file: {file}");
                    pdf.AddDocument(new iTextSharp.text.pdf.PdfReader(file));
                }

                AddBookmarks(files, pdf);
            }
            catch (Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
                pdf?.Dispose();
                doc?.Dispose();
                stream?.Dispose();
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

            LogHelper.Log("Replace the cover to the original file", LogType.Successful);

            FileStream stream = null;
            Document doc = null;
            PdfCopy pdf = null;

            try
            {
                stream = new FileStream(OutFile, FileMode.Create);
                doc = new Document();
                pdf = new PdfCopy(doc, stream);

                doc.Open();

                //Aggiungo la cover
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InCover);
                int coverPage = reader.NumberOfPages;

                LogHelper.Log($"Add the cover: {InCover} of {coverPage} pages");
                pdf.AddDocument(reader);
                reader.Close();

                //Aggiungo il resto del documento
                reader = new iTextSharp.text.pdf.PdfReader(InFile);
                int count = reader.NumberOfPages;
                coverPage++;
                List<int> pages = Enumerable.Range(coverPage, count - coverPage + 1).ToList();

                LogHelper.Log($"Add the file: {InFile} from Page: {coverPage}");
                pdf.AddDocument(reader, pages);

                reader.Close();
            }
            catch(Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
                pdf?.Dispose();
                doc?.Dispose();
                stream?.Dispose();
            }

        }

        public static void SplitPDF(string InFiles, string OutDir)
        {
            string outFiles = OutDir + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles);

            LogHelper.Log($"I create the directory: {OutDir}");
            Directory.CreateDirectory(OutDir);

            LogHelper.Log("I split the file into individual PDF", LogType.Successful);

            PdfReader reader = null;

            try
            {
                reader = new PdfReader(InFiles);

                int NumPages = reader.NumberOfPages;

                int digitN = NumPages.ToString().Length;
                if (digitN < DefaultDigit) digitN = (int)DefaultDigit;

                for (int i = 1; i <= NumPages; i++)
                {
                    string outFile = string.Format("{0}_Page {1:D" + digitN + "}.pdf", outFiles, i);
                    FileStream stream = new FileStream(outFile, FileMode.Create);

                    LogHelper.Log($"Page: {Path.GetFileNameWithoutExtension(outFile)}");
                    Document doc = new Document();
                    PdfCopy pdf = new PdfCopy(doc, stream);

                    doc.Open();
                    PdfImportedPage page = pdf.GetImportedPage(reader, i);
                    pdf.AddPage(page);

                    pdf.Dispose();
                    doc.Dispose();
                    stream.Dispose();
                }

            }
            catch(Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
                reader?.Dispose();
            }
        }


        private static void AddBookmarks(List<string> files, PdfCopy OutFile)
        {
            // Create a list for the bookmarks
            List<Dictionary<String, Object>> bookmarks =  new List<Dictionary<String, Object>>();

            int page_offset = 0;

            PdfReader reader = null;

            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    reader = new PdfReader(files[i]);

                    // merge the bookmarks
                    IList<Dictionary<String, Object>> tmp = SimpleBookmark.GetBookmark(reader);
                    SimpleBookmark.ShiftPageNumbers(tmp, page_offset, null);

                    foreach (var d in tmp) bookmarks.Add(d);
                
                    // add the pages
                    int n = reader.NumberOfPages;

                    page_offset += n;
                }

                // Add the merged bookmarks
                OutFile.Outlines = bookmarks;
                LogHelper.Log("Bookmarks copied successfully", LogType.Successful);
            }
            catch (Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
                reader?.Dispose();
            }
        }
    }
}