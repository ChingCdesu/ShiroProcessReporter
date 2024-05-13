using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using ShiroProcessReporter.Helper;
using ShiroProcessReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Loggers
{
    public sealed class MemoryLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var dispatcher = GlobalState.Instance.LogViewDispatcherQueue;

            var entry = new LogEntry
            {
                LogLevel = logLevel,
                Message = $"[{DateTime.Now} {logLevel}]  {formatter(state, exception)}",
            };

            if (dispatcher == null)
            {
                AddLog(entry);
            }
            else
            {
                dispatcher.TryEnqueue(() =>
                {
                    AddLog(entry);
                });
            }
        }

        private void AddLog(LogEntry entry)
        {
            GlobalState.Instance.Logs.Add(entry);

            while (GlobalState.Instance.Logs.Count > 1000)
            {
                GlobalState.Instance.Logs.RemoveAt(0);
            }
        }
    }
}
