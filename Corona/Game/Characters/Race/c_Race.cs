using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona
{
    public enum Race
    {
        HUMAN=0,
        ELF,
        DWARF,
        ORC,
        NODEATH
    }

    public class c_Race
    {
        public Race _race;

        public c_Race(Race r)
        {
            _race = r;
        }

        public Race GetRace => _race;
    }
}
