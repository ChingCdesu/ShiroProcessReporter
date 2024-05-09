using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShiroProcessReporter.Loggers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Providers
{
    public sealed class MemoryLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, MemoryLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new MemoryLogger());

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
