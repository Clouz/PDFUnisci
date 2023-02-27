using System.IO;

using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.IO.Source;
using iText.StyledXmlParser.Jsoup.Parser;
using iText.Forms;

using LogManager;
using System.Linq;
using System;
using iText.Kernel.Utils;

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

        static int _FlatOnlyFirstPage = 0;
        public static int FlatOnlyFirstPage
        {
            get
            {
                return _FlatOnlyFirstPage;
            }

            set
            {
                if (value >= 1) _FlatOnlyFirstPage = 1;
                else _FlatOnlyFirstPage = 0;
            }
        }

        public static void MergePDF(List<string> files, string OutFile)
        {

            LogHelper.Log("Join all files into a single PDF", LogType.Successful);

            PdfDocument pdfDoc;

            try
            {
                PdfWriter writer = new PdfWriter(OutFile);
                writer.SetSmartMode(true);
                pdfDoc = new PdfDocument(writer);
                pdfDoc.InitializeOutlines();
                PdfReader reader;
                PdfDocument pdfInnerDoc;
            
                foreach (string file in files)
                {
                    LogHelper.Log($"Add the file: {file}");

                    // create a PDF in memory
                    reader = new PdfReader(file);

                    //if you want to (non-trivially) change a document, the PdfDocument needs to be initialized with a PdfWriter;
                    //so if you want to change and copy from a document, you cannot use the same PdfDocument instance for both actions!
                    pdfInnerDoc = new PdfDocument(reader);
                    pdfInnerDoc.CopyPagesTo(1, pdfInnerDoc.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());

                    //TODO: AddBookmarks(files, pdf);
                    pdfInnerDoc.Close();
                }

                pdfDoc.Close();
            }
            catch (Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
                
            }
        }

        // public static void ReplaceCoverPDF(string InFile, string InCover, string OutFile)
        // {
        //     if(CoverFunction == 0) 
        //     {
        //         List<string> Files = new List<string>
        //         {
        //             InFile,
        //             InCover
        //         };
        //         Files.Sort();
        //         MergePDF(Files, OutFile);
        //         return;
        //     }

        //     LogHelper.Log("Replace the cover to the original file", LogType.Successful);

        //     FileStream stream = null;
        //     Document doc = null;
        //     PdfCopy pdf = null;

        //     try
        //     {
        //         stream = new FileStream(OutFile, FileMode.Create);
        //         doc = new Document();
        //         pdf = new PdfCopy(doc, stream);

        //         doc.Open();

        //         //Aggiungo la cover
        //         iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InCover);
        //         int coverPage = reader.NumberOfPages;

        //         LogHelper.Log($"Add the cover: {InCover} of {coverPage} pages");
        //         pdf.AddDocument(reader);
        //         reader.Close();

        //         //Aggiungo il resto del documento
        //         reader = new iTextSharp.text.pdf.PdfReader(InFile);
        //         int count = reader.NumberOfPages;
        //         coverPage++;
        //         List<int> pages = Enumerable.Range(coverPage, count - coverPage + 1).ToList();

        //         LogHelper.Log($"Add the file: {InFile} from Page: {coverPage}");
        //         pdf.AddDocument(reader, pages);

        //         reader.Close();

        //         AddBookmarks(InFile, pdf);
        //     }
        //     catch(Exception e)
        //     {
        //         LogHelper.Log(e.ToString(), LogType.Error);
        //     }
        //     finally
        //     {
        //         pdf?.Dispose();
        //         doc?.Dispose();
        //         stream?.Dispose();
        //     }

        // }

        // public static void CreateEmptyPage(string OutFile, string pageSize = "A4", bool rotate = false)
        // {
  
        //     Rectangle ps;

        //     switch (pageSize)
        //     {
        //         case "A3":
        //             ps = PageSize.A3;
        //             break;
        //         case "A1":
        //             ps = PageSize.A1;
        //             break;
        //         case "A0":
        //             ps = PageSize.A0;
        //             break;
        //         default:
        //             ps = PageSize.A4;
        //             break;
        //     }

        //     if (rotate) {
        //         ps.Rotate();
        //     }

        //     try
        //     {
        //         System.IO.FileStream fs = new FileStream(OutFile, FileMode.Create);
        //         Document document = new Document(ps);
        //         PdfWriter writer = PdfWriter.GetInstance(document, fs);

        //         document.Open();

        //         writer.PageEmpty = false;
        //         document.NewPage();
        //         document.Close();
        //         writer.Close();
        //         fs.Close();

        //     }
        //     catch (Exception e)
        //     {
        //         LogHelper.Log(e.ToString(), LogType.Error);
        //     }

        // }

        public static void SplitPDF(string InFiles, string OutDir = "", int PageN = 0)
        {
            string outFiles = OutDir + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles);

            if (OutDir != ""){
                LogHelper.Log($"I create the directory: {OutDir}");
                Directory.CreateDirectory(OutDir);
            }

            LogHelper.Log("I split the file into individual PDF", LogType.Successful);

            try
            {
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(InFiles));
                IList<PdfDocument> splitDocuments = new PdfSplitter(pdfDoc).SplitByPageCount(1);

                int NumPages = pdfDoc.GetNumberOfPages();

                int digitN = NumPages.ToString().Length;
                if (digitN < DefaultDigit) digitN = (int)DefaultDigit;
                PageN = 0;

                foreach (PdfDocument doc in splitDocuments)
                {
                    PageN++;
                    string outFile = string.Format("{0}_Page {1:D" + digitN + "}.pdf", outFiles, PageN);
                    LogHelper.Log($"Page: {Path.GetFileNameWithoutExtension(outFile)}");

                    PdfReader reader = doc.GetReader();
                    PdfWriter writer = new PdfWriter(outFile);
                    PdfDocument SinglePageDoc = new PdfDocument(reader, writer);

                    
                    SinglePageDoc.Close();
                    
                    doc.Close();
                }

                pdfDoc.Close();

            }
            catch(Exception e)
            {
                LogHelper.Log(e.ToString(), LogType.Error);
            }
            finally
            {
            }
        }


        // private static void AddBookmarks(List<string> files, PdfCopy OutFile)
        // {
            
        //     // Create a list for the bookmarks
        //     List<Dictionary<String, Object>> bookmarks =  new List<Dictionary<String, Object>>();

        //     int page_offset = 0;

        //     PdfReader reader = null;

        //     try
        //     {
        //         for (int i = 0; i < files.Count; i++)
        //         {
        //             reader = new PdfReader(files[i]);

        //             // merge the bookmarks
        //             IList<Dictionary<String, Object>> tmp = SimpleBookmark.GetBookmark(reader);
                    
        //             SimpleBookmark.ShiftPageNumbers(tmp, page_offset, null);

        //             if (Bookmarks == 1)
        //             {
        //                 tmp = new List<Dictionary<String, Object>>() {
        //                     new Dictionary<string, object>()
        //                     {
        //                         ["Title"] = $"{Path.GetFileNameWithoutExtension(files[i])}",
        //                         ["Page"] = $"{page_offset+1}",
        //                         ["Action"] = "GoTo",
        //                         ["Kids"] = tmp
        //                     }
        //                 };
        //             }


        //             if (tmp != null)
        //             {
        //                 foreach (var d in tmp)
        //                 {
        //                         bookmarks.Add(d);
        //                 }
        //              }
                
        //             // add the pages
        //             int n = reader.NumberOfPages;

        //             page_offset += n;
        //         }

        //         // Add the merged bookmarks
        //         OutFile.Outlines = bookmarks;

        //         LogHelper.Log("Bookmarks copied successfully", LogType.Successful);
        //     }
        //     catch (Exception e)
        //     {
        //         LogHelper.Log(e.ToString(), LogType.Error);
        //     }
        //     finally
        //     {
        //         reader?.Dispose();
        //     }
        // }

        // private static void AddBookmarks(string file, PdfCopy OutFile) {
        //     List<string> files = new List<string>() { file };

        //     AddBookmarks(files, OutFile);
        // }

        // public static void ImgToPDF(List<string> files, string OutFile)
        // {
        //     LogHelper.Log("Join all immages into a single PDF", LogType.Successful);

        //     Document doc = null;
        //     FileStream fs = null;
        //     PdfWriter writer = null;

        //     try
        //     {
        //         doc = new Document();
        //         fs = new FileStream(OutFile, FileMode.Create, FileAccess.Write, FileShare.None);
        //         writer = PdfWriter.GetInstance(doc, fs);

        //         doc.Open();

        //         foreach (var ImgFile in files)
        //         {
        //             LogHelper.Log($"Add the file: {ImgFile}");

        //             Image img = iTextSharp.text.Image.GetInstance(ImgFile);
        //             doc.SetPageSize(new iTextSharp.text.Rectangle(0, 0, img.Width, img.Height, 0));
        //             doc.NewPage();

        //             img.SetAbsolutePosition(0, 0);
        //             writer.DirectContent.AddImage(img);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         LogHelper.Log(e.ToString(), LogType.Error);
        //     }
        //     finally
        //     {
        //         doc?.Dispose();
        //         fs?.Dispose();
        //         writer?.Dispose();
        //     }
        // }

        // public static void FlatPDF(List<string> files)
        // {

        //     LogHelper.Log("Flatt all PDF comments", LogType.Successful);

        //     foreach (var file in files)
        //     {
        //         string OutFile = $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(file)}_flat.pdf";

        //         LogHelper.Log($"Flattening file: { Path.GetFileNameWithoutExtension(file)}");

        //         using (FileStream stream = new FileStream(OutFile, FileMode.Create))
        //         {
        //             PdfReader reader = null;
        //             PdfStamper stamper = null;
        //             try
        //             {
        //                 reader = new PdfReader(file);

        //                 stamper = new PdfStamper(reader, stream)
        //                 {
        //                     FormFlattening = true,
        //                     AnnotationFlattening = true,
        //                     FreeTextFlattening = true,
        //                 };
        //                 stamper.Close();
        //             }
        //             catch (Exception e)
        //             {
        //                 LogHelper.Log(e.ToString(), LogType.Error);

        //                 reader?.Close();
        //             }
        //             finally
        //             {
        //                 reader?.Close();
        //             }
        //         }
        //     }
        // }


        // public static void FlatPDFonlyFistPage(List<string> files)
        // {

        //     LogHelper.Log("Flatt all PDF comments", LogType.Successful);

        //     foreach (var file in files)
        //     {
        //         string OutFile = $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(file)}_flat.pdf";

        //         LogHelper.Log($"Flattening file: { Path.GetFileNameWithoutExtension(file)}");

        //         using (FileStream stream = new FileStream(OutFile, FileMode.Create))
        //         {
        //             PdfReader reader = null;
        //             PdfStamper stamper = null;
        //             try
        //             {
        //                 var memStream1 = new MemoryStream();

        //                 reader = new PdfReader(file);
        //                 reader.SelectPages("1");

        //                 stamper = new PdfStamper(reader, memStream1)
        //                 {
        //                     FormFlattening = true,
        //                     AnnotationFlattening = true,
        //                     FreeTextFlattening = true,
        //                 };
        //                 stamper.Close();
        //                 PdfReader readerfirstPage = new PdfReader(memStream1.ToArray());
                        
        //                 Document doc = new Document();
        //                 PdfCopy pdf = new PdfCopy(doc, stream);
        //                 doc.Open();
        //                 pdf.AddDocument(readerfirstPage);

        //                 PdfReader readerOtherPages = new PdfReader(file);
        //                 if (readerOtherPages.NumberOfPages > 2)
        //                 {
        //                     readerOtherPages.SelectPages($"2-{readerOtherPages.NumberOfPages}");
        //                     pdf.AddDocument(readerOtherPages);
        //                 }
        //                 doc.Close();
        //                 pdf.Close();

        //             }
        //             catch (Exception e)
        //             {
        //                 LogHelper.Log(e.ToString(), LogType.Error);

        //                 reader?.Close();
        //             }
        //             finally
        //             {
        //                 reader?.Close();
        //             }
        //         }
        //     }
        // }
    }
}