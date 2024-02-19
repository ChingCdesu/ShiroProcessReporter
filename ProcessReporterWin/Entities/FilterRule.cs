using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReporterWin.Entities
{
    public class FilterRule
    {
        public string KeyWord { get; set; } = string.Empty;
        public bool Editing { get; set; }
        public bool NotEditing { get => !Editing; }
    }
}
