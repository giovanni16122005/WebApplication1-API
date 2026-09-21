using System;
using System.Diagnostics;
using System.IO;
using Serilog;
using Serilog.Events;

namespace Common.Log
{
    /// <summary>
    /// Serilog padronizado:
    /// - Arquivo: C:\ProgramData\IVT\<Service>\logs\service-.log
    /// - Event Viewer: fonte = <Service>, log = Application
    /// - Console: somente em DEBUG
    /// </summary>
    public sealed class ServiceLog : IDisposable
    {
        public string ServiceName { get; }
        public string LogsDirectory { get; }
        public Serilog.ILogger Logger { get; }

        public ServiceLog(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("serviceName inválido.", nameof(serviceName));

            ServiceName = serviceName.Trim();
            LogsDirectory = Path.Combine(@"C:\ProgramData", "Giovanni", ServiceName, "logs");
            try { Directory.CreateDirectory(LogsDirectory); } catch { /* ignore */ }

            // Event Source
            try
            {
                if (!EventLog.SourceExists(ServiceName))
                    EventLog.CreateEventSource(ServiceName, "Application");
            }
            catch { /* ignore */ }

            var cfg = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()

                .WriteTo.File(
                    path: Path.Combine(LogsDirectory, "service-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 10,
                    shared: true
                )
                .WriteTo.EventLog(
                    source: ServiceName,
                    manageEventSource: false,
                    restrictedToMinimumLevel: LogEventLevel.Information
                );

            Logger = cfg.CreateLogger();

            // set Serilog global
            global::Serilog.Log.Logger = Logger;
        }

        public void Dispose()
        {
            try { global::Serilog.Log.CloseAndFlush(); } catch { /* ignore */ }
        }
    }
}