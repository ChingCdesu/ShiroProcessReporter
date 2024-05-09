using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Models
{
    public partial class LogEntry
    {
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
