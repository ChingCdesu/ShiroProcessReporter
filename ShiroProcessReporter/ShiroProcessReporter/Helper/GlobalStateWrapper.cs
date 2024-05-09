using ShiroProcessReporter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public class GlobalStateWrapper
    {
        public ObservableCollection<LogEntry> Logs => GlobalState.Instance.Logs;
    }
}
