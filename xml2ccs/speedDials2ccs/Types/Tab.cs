using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace speedDials2ccs.Types
{
    class Tab
    {
        public bool enabled = true;
        public string panel_tag = "defViewPanel";
        public int show_subtabs = 10;
        public string type = "autTab";
        public List<Subtab> subtabs;

        public Tab() 
        {
            subtabs = new List<Subtab>();
        }
    }
}
