using System;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;

namespace PDFUnisci
{
    public static class Menu
    {
        public static void Create() {
            string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string AssemblyTitle = Assembly.GetExecutingAssembly().GetName().Name;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{AssemblyTitle} {AssemblyVersion}\nCopyright(C) 2016 by Claudio Mola");
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

                if(risposta.ToLower() == "si" || risposta.ToLower() == "s")
                {
                    System.IO.File.Delete(collegamento);
                    Console.Write("Collegamento eliminato...");
                    Console.Read();
                }
                else
                {
                    Console.Write("Come non detto...");
                    Console.Read();
                }
            }
            else {

                Console.WriteLine("Vuoi creare un collegamento ad 'Invia a'? [Si, No]");
                Console.Write("> ");
                string risposta = Console.ReadLine();

                if (risposta.ToLower() == "si" || risposta.ToLower() == "s")
                {

                    //Creo il collegamento
                    object shDesktop = (object)"Desktop";
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(collegamento);
                    shortcut.Description = "Shortcut for PDFUnisci";
                    shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    shortcut.Save();

                    Console.Write("Collegamento creato...");
                    Console.Read();
                }
                else
                {
                    Console.Write("Come non detto...");
                    Console.Read();
                }
			} 
        }

    }
}