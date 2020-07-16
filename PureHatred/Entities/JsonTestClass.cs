using System;
using System.Collections.Generic;
using System.Text;

namespace PureHatred.Entities
{
    class JsonTestClass
	{
        public string foreground { get; set; }
        public string background { get; set; }
        public string name { get; set; }
        public int glyph { get; set; }
        public int hungerComplex { get; set; }
        public int hungerSimple { get; set; }
        public int hpMax { get; set; }
        public int hpCurrent { get; set; }
        public object owner { get; set; }
    }
}
