using System;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using System.Xml.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFUnisci
{
	class Program
	{
		string[] files;     //contiene tutti i file
		string fileCover;   //contiene il file cover
		bool cover;         //true se esiste una cover
		int lenFiles;       //Conteggio file inclusa la cover

		//Config
		int DefaultDigit { get; set; }          //Imposta i digit che avrà il nome file diviso
		int ExitConfirmation { get; set; }		//Chiede conferma di uscita
		int CoverFunction { get; set; }			//Abilita la funzione unisci cover

		public Program(string[] files) {

			this.files = files;
			this.lenFiles = files.Length;
			this.cover = false;

			DefaultDigit = 3;
			ExitConfirmation = 0;
			CoverFunction = 1;

			//leggo il file di configurazione
			LeggiXML();

			//ordino l'array
			Array.Sort(this.files);

            //Verifico la presenza di un file che contiene nel nomefile cover
            if(lenFiles == 2)
                foreach (string i in this.files)
                    if (Path.GetFileNameWithoutExtension(i).ToLower().Contains("cover")) {
                        cover = true;
                        fileCover = i;
                        break;
                    }
                
            //creo un nuovo array senza la cover
            if(cover && this.CoverFunction == 1)
                this.files = this.files.Except(new string[] { fileCover }).ToArray();

            //mostro il contenuto
            if (lenFiles != 0)
                StampaContenuto();  
        }

        public int GetLenFiles() {
            return lenFiles;
        }

        public bool GetCover() {
            return cover;
        }

        public void StampaContenuto() {
            Console.WriteLine("\n------");
            Console.WriteLine("Cover: {0}", cover);
            if(cover)
                Console.WriteLine("Cover: {0}", fileCover);

            for (int i = 0; i < files.Length; i++) {
                Console.WriteLine("File {0}: {1}", i, files[i]);
            }

            Console.WriteLine("------\n");
        }

        static void Menu() {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("PDFUnisci 1.09\nCopyright(C) 2016 by Claudio Mola");
            Console.ResetColor();

            Console.WriteLine("\nThis program is free software: you can redistribute it and / or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.\nYou should have received a copy of the GNU General Public License along with this program. If not, see < http://www.gnu.org/licenses/>.\n\n");

            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.WriteLine("Istruzioni:");
            Console.ResetColor();

            Console.WriteLine("0 file in argomento:\nViene mostrato questo menù con la possibilità di creare o rimuovere un collegamento ad 'Invia a'.\n");
            Console.WriteLine("1 file pdf in argomento:\nViene creata una cartella in cui all'interno viene diviso il PDF in singole pagine.\n");
            Console.WriteLine("2 o più file pdf in argomento:\nViene creato un nuovo file che li unirà in ordine alfabetico. Se uno dei file in argomento contiene la parola 'cover', esso sostituirà le prime pagine del pdf successivo.\n");
            
            string sedto = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            string collegamento = sedto + Path.DirectorySeparatorChar + "PDFUnisci.lnk";

            if (System.IO.File.Exists(collegamento)) {
                Console.WriteLine("Vuoi eliminare il collegamenti ad 'Invia a'? [Si, No]");
                Console.Write("> ");
                string risposta = Console.ReadLine();

                if(risposta.ToLower() == "si" || risposta.ToLower() == "s") {
                    System.IO.File.Delete(collegamento);
                    Console.WriteLine("Collegamento eliminato...");
                }
                else {
                    Console.WriteLine("Come non detto...");
                    Console.Read();
                }
            }
            else {

                Console.WriteLine("Vuoi creare un collegamento ad 'Invia a'? [Si, No]");
                Console.Write("> ");
                string risposta = Console.ReadLine();

                if (risposta.ToLower() == "si" || risposta.ToLower() == "s") {

                    //Creo il collegamento
                    object shDesktop = (object)"Desktop";
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(collegamento);
                    shortcut.Description = "Shortcut for PDFUnisci";
                    shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    shortcut.Save();

                    Console.WriteLine("Collegamento creato...");      
                }
                else
                    Console.WriteLine("Come non detto...");
					Console.Read();
			} 
        }

        private void UnisciPDF()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unisco tutti i file in un unico PDF\n");
            Console.ResetColor();

            //Creo il nomefile
            string OutFile = Path.GetDirectoryName(files[0]) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(files[0]) + "_merged.pdf";

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

        private void DividiPDF() //ok
        {
            string InFiles = files[0];

            //Creo la cartella dove verranno salvati i file
            string pathDir = Path.GetDirectoryName(InFiles) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles) + "_split";
            string OutFile = pathDir + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(InFiles);

            Console.WriteLine($"Creo la directory in: {pathDir}");
            Directory.CreateDirectory(pathDir);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Divido il file in tanti PDF singoli\n");
            Console.ResetColor();

            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(InFiles))
            {
                int NumPages = reader.NumberOfPages;

                for (int i = 1; i <= NumPages; i++)
                {
                    int digitN = NumPages;
                    if (digitN < DefaultDigit)
                        digitN = DefaultDigit;

                    string outFile = string.Format("{0}_Page {1:D" + digitN + "}.pdf", OutFile, i);
                    FileStream stream = new FileStream(outFile, FileMode.Create);

                    Console.WriteLine($"Apro il file: {InFiles}");
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

        private void SostituisciCover()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sostituisco la cover al file originario\n");
            Console.ResetColor();

            //Numero di pagine della cover
            int coverPage = 0;

            int len = files.Length;

            string InFiles = files[0];

            // NomeFile
            string filename = Path.GetDirectoryName(files[0]) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(files[0]) + "_merged.pdf";

            if (System.IO.File.Exists(InFiles) && Path.GetExtension(InFiles).ToLower() == ".pdf")
            {
                //Apro il file della cover
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (Document doc = new Document())
                using (PdfCopy pdf = new PdfCopy(doc, stream))
                {
                    doc.Open();

                    //Apro il file della cover
                    iTextSharp.text.pdf.PdfReader reader = reader = new iTextSharp.text.pdf.PdfReader(fileCover);
                    PdfImportedPage page = null;

                    //Ne conto le pagine
                    coverPage = reader.NumberOfPages;
                    Console.WriteLine($"Apro la cover: {fileCover}\tdi {coverPage} pagine");

                    //Ogni pagina della cover viene copiata nel file di destinazione
                    for (int i = 0; i < coverPage; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }
                    reader.Close();

                    //Apro il documento
                    reader = new iTextSharp.text.pdf.PdfReader(InFiles);

                    //Ne conto le pagine
                    int count = reader.NumberOfPages;
                    Console.WriteLine($"Apro il file: {InFiles}\tda pagina: {coverPage}");

                    //Continuo a scrivere sul file di destinazione
                    for (int i = coverPage; i < count; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }
                    reader.Close();
                    pdf.FreeReader(reader);
                }
            }
            else
            {
                Console.WriteLine("{0}\t- File non trovato, verrà escluso dal PDF", InFiles);
            }
        }


		private void LeggiXML()
		{

			XDocument config;

			Console.WriteLine("\n------");
			Console.WriteLine("Lettura configurazione");

			try
			{
				config = XDocument.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.xml");
				System.Console.WriteLine("Il file config.xml è stato letto correttamente");
			}
			catch (System.IO.FileNotFoundException)
			{
				config = new XDocument(
					new XComment("PDFUnisci config"),
					new XElement("config",
						new XComment("Indica il numero di digit quando si divide un PDF"),
						new XElement("DefaultDigit", DefaultDigit),
						new XComment("Il programma rimane aperto fino alla pressione di un tasto"),
						new XElement("ExitConfirmation", ExitConfirmation),
						new XComment("Se impostatp a zero disattiva la funziona per sostituire la cover"),
						new XElement("CoverFunction", CoverFunction)));
				config.Declaration = new XDeclaration("1.0", "utf-8", "true");
				config.Save(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.xml");

				System.Console.WriteLine("Il file config.xml non è stato trovato, verrà creato");
			}

			
			int i = 0;
			bool test = int.TryParse(config.Elements("config").Select(o => o.Element("DefaultDigit").Value).First(), out i);
			if (test)
			{
				if (i < 1)
					i = 1;
				DefaultDigit = i;
				System.Console.WriteLine("DefaultDigit: {0}", DefaultDigit);
			}
			else
			{
				System.Console.WriteLine("DefaultDigit: Impossibile leggere la configurazione, verrà impostato il valore di default({0})", DefaultDigit);
			}

			i = 0;
			test = int.TryParse(config.Elements("config").Select(o => o.Element("ExitConfirmation").Value).First(), out i);
			if (test)
			{
				if (i != 1 && i != 0)
					i = 0;
				ExitConfirmation = i;
				System.Console.WriteLine("ExitConfirmation: {0}", ExitConfirmation);
			}
			else
			{
				System.Console.WriteLine("ExitConfirmation: Impossibile leggere la configurazione, verrà impostato il valore di default({0})", ExitConfirmation);
			}

			i = 0;
			test = int.TryParse(config.Elements("config").Select(o => o.Element("CoverFunction").Value).First(), out i);
			if (test)
			{
				if (i != 1 && i != 0)
					i = 1;
				CoverFunction = i;
				System.Console.WriteLine("CoverFunction: {0}", CoverFunction);
			}
			else
			{
				System.Console.WriteLine("coverFunction: Impossibile leggere la configurazione, verrà impostato il valore di default({0})", CoverFunction);
			}

			Console.WriteLine("------\n");

		}

		static void Main(string[] args)
        {
            Program prog = new Program(args);

            //basandomi sulla quantità di file nell'array scelgo un azione da compiere
            switch (prog.GetLenFiles())
            {
                case 0:
                    Program.Menu();
                    break;
                case 1:
                    prog.DividiPDF();
                    break;
                default:
                    if (prog.GetCover() && prog.CoverFunction == 1)
                        prog.SostituisciCover();
                    else
                        prog.UnisciPDF();
                    break;
            }

			if (prog.ExitConfirmation == 1)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("\nPremere invio per chiudere :)");
				Console.ResetColor();

				Console.Read();
			}

        }

    }
}