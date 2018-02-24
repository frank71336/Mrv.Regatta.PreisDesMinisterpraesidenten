using Mrv.Regatta.PreisDesMinisterpraesidenten.Db.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public class Club
    {
        public TVerein Verein { get; set; }
        public float Points { get; set; }
    }
}
