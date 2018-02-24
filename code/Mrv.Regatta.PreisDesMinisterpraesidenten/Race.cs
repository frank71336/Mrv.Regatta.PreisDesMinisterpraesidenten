using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public class Race
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Race"/> class.
        /// </summary>
        public Race()
        {
            Clubs = new List<Club>();
        }

        public string Name { get; set; }
        public int RowersCount { get; set; }
        public List<Club> Clubs { get; set; }

    }
}
