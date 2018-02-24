using Mrv.Regatta.PreisDesMinisterpraesidenten.Db.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    class Program
    {
        static void Main(string[] args)
        {
            // Nicht behandelte Fehler abfangen
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            // Version ausgeben
            Tools.OutputText("Programm-Version: " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(), ConsoleColor.DarkGray);

            // Berechnung durchführen
            var calculation = new Calculation();
            calculation.Run();

            Tools.OutputText("");
            Tools.OutputText("Fertig. Beliebige Taste zum Beenden drücken...");

            Console.ReadKey();
        }

        /// <summary>
        /// NIicht behandelte Fehler behandeln
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Tools.OutputText();
            Tools.OutputText();
            Tools.OutputText();

            // Fehler ausgeben
            var exception = e.ExceptionObject;
            Tools.OutputText(exception.ToString(), ConsoleColor.Red);

            Console.ReadKey();

            // Beenden
            Environment.Exit(0);
        }
    }
}
