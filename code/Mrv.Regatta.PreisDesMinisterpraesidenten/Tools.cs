using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public static class Tools
    {
        /// <summary>
        /// Gets the name of the excel column.
        /// </summary>
        /// <param name="columnNumber">The column number.</param>
        /// <returns></returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }


        #region Output Text to Console and Debug Window

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        public static void OutputText()
        {
            OutputText("");
        }

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="autoReturn">if set to <c>true</c> [automatic return].</param>
        public static void OutputText(string text, bool autoReturn = true)
        {
            _OutputTextHelper(text, autoReturn);
        }

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="autoReturn">if set to <c>true</c> [automatic return].</param>
        public static void OutputText(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor, bool autoReturn = true)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            _OutputTextHelper(text, autoReturn);
        }

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="autoReturn">if set to <c>true</c> [automatic return].</param>
        public static void OutputText(string text, ConsoleColor foregroundColor, bool autoReturn = true)
        {
            Console.ForegroundColor = foregroundColor;
            _OutputTextHelper(text, autoReturn);
        }

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="text">The text.</param>
        /// <param name="autoReturn">if set to <c>true</c> [automatic return].</param>
        public static void OutputText(ConsoleColor backgroundColor, string text, bool autoReturn = true)
        {
            Console.BackgroundColor = backgroundColor;
            _OutputTextHelper(text, autoReturn);
        }

        /// <summary>
        /// Outputs text to console and debug/output window.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="autoReturn">if set to <c>true</c> [automatic return].</param>
        public static void _OutputTextHelper(string text, bool autoReturn = true)
        {
            if (autoReturn)
            {
                Console.WriteLine(text);
                Debug.WriteLine(text);
            }
            else
            {
                Console.Write(text);
                Debug.Write(text);
            }
            ResetConsoleColors();
        }

        public static void ResetConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        #endregion Output Text to Console and Debug Window

    }
}
