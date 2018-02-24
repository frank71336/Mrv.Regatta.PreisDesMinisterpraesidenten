using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public static class ExtensionMethods
    {
        public static string Col(this int columnNumber)
        {
            return Tools.GetExcelColumnName(columnNumber);
        }

    }
}
