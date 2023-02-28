using System;
using System.IO;
using System.Reflection;
//using IWshRuntimeLibrary;

namespace PDFUnisci
{
    public static class Menu
    {
        public static void Create() {
            string AssemblyVersion = "2.00";
            string AssemblyTitle = "PDFUnisci";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{AssemblyTitle} {AssemblyVersion}\nCopyright(C) 2023 by Claudio Mola\n");
            Console.ResetColor();

            Console.WriteLine("usage: PDFUnisci [options] filenames");
            Console.WriteLine("\toptions:");
            Console.WriteLine("\t\t-b\t\t\tAdd Bookmarks");
            Console.WriteLine("\t\t-s\t\t\tSplit all the files in dedicated folders");
            Console.WriteLine("\t\t-flat\t\t\tAll the comments will be flattened");
            Console.WriteLine("\t\t-singlePageSplit\tOnly the selected page will be splitted");
            Console.WriteLine("\t\t-np\t\t\tCreate new empty page in a temp folder");
            Console.WriteLine("\t\t-o\t\t\tAuto open the created file");
            Console.WriteLine("\n");

            Console.WriteLine("More information: https://github.com/Clouz/PDFUnisci\n");

            Console.WriteLine("This program is free software: you can redistribute it and / or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.\nYou should have received a copy of the GNU General Public License along with this program. If not, see < http://www.gnu.org/licenses/>.\n\n");

            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.WriteLine("Instructions:");
            Console.ResetColor();

            Console.WriteLine("0 Command-Line Argument:\nThis menu is shown with the option to create or remove a link to 'Send to'.\n");
            Console.WriteLine("1 Command-Line Argument:\nThe PFD is divided into single pages inside a new folder.\n");
            Console.WriteLine("2 or more Command-Line Arguments:\nThis creates a new PDF that combine them in alphabetical order. If one of the files in question contains the word 'cover', it will replace the front pages of the next pdf.\n");
            
            string sedto = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            string collegamento = sedto + Path.DirectorySeparatorChar + "PDFUnisci.lnk";
            string collegamento2 = sedto + Path.DirectorySeparatorChar + "PDFUnisci Flat.lnk";

            if (System.IO.File.Exists(collegamento)) {
                Console.WriteLine("Do you want to delete the link to 'Send to'? [Yes, No]");
                Console.Write("> ");
                string? risposta = Console.ReadLine();

                if(risposta.ToLower() == "yes" || risposta.ToLower() == "y")
                {
                    try
                    {
                        System.IO.File.Delete(collegamento);
                        System.IO.File.Delete(collegamento2);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Console.Write("Link removed...");
                    Console.Read();
                }
                else
                {
                    Console.Write("Never mind...");
                    Console.Read();
                }
            }
            else {

                Console.WriteLine("You want to create a link to 'Send to'? [Yes, No]");
                Console.Write("> ");
                string? risposta = Console.ReadLine();

                if (risposta.ToLower() == "yes" || risposta.ToLower() == "y")
                {

                    // //Creo il collegamento
                    // //object shDesktop = (object)"Desktop";
                    // WshShell shell = new WshShell();
                    // IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(collegamento);
                    // shortcut.Description = "Shortcut for PDFUnisci";
                    // shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    // shortcut.Save();

                    // //Creo il collegamento2
                    // WshShell shell2 = new WshShell();
                    // IWshShortcut shortcut2 = (IWshShortcut)shell2.CreateShortcut(collegamento2);
                    // shortcut2.Description = "Shortcut for PDFUnisci flat";
                    // shortcut2.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    // shortcut2.Arguments = "-flat";
                    // shortcut2.Save();

                    Console.Write("Link created...");
                    Console.Read();
                }
                else
                {
                    Console.Write("Never mind...");
                    Console.Read();
                }
			} 
        }

    }
}