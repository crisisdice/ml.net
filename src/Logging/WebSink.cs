using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Sentiment.Logging
{
    public class WebSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IHubContext<LoggingHub> _hubContext;

        public WebSink(IFormatProvider formatProvider, IHubContext<LoggingHub> hubContext)
        {
            _formatProvider = formatProvider;
            _hubContext = hubContext;
        }

        public void Emit(LogEvent logEvent)
        {
            var assembly = logEvent.Properties.TryGetValue("SourceContext", out var context) ? context.ToString() : "Program";

            var message = new LogEventMessage
            {
                Entry = logEvent.RenderMessage(_formatProvider),
                Preface = $"{logEvent.Timestamp} [{assembly}] ",
                Level = logEvent.Level.ToString()
            };

            Task.Run(() => _hubContext.Clients.All.SendAsync("ReceiveMessage", message));
        }   
    }

    public static class MySinkExtensions
    {
        public static LoggerConfiguration WebSink(
            this LoggerSinkConfiguration loggerConfiguration,
            IHubContext<LoggingHub> service,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new WebSink(formatProvider, service));
        }
    }

    public class LogEventMessage
    {
        public string Entry { get; set; }
        public string Preface { get; set; }
        public string Level { get; set; }
    }
    public class LoggingHub : Hub
    {
    }
}