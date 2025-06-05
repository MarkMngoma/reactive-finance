using System.Text;
using log4net.Appender;
using log4net.Core;

namespace Server.Main.Reactor.Configuration;

public class AnsiConsoleAppender : ConsoleAppender
{
    public AnsiConsoleAppender()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            Console.OutputEncoding = Encoding.UTF8;
            EnableAnsiOnWindows();
        }
    }

    protected override void Append(LoggingEvent loggingEvent)
    {
        string coloredMessage = FormatLogMessage(loggingEvent);
        Console.Write(coloredMessage);
    }

    private string FormatLogMessage(LoggingEvent loggingEvent)
    {
        const string Reset = "\x1B[0m";
        string timestampColor = "\x1B[33m";
        string levelColor = GetLevelColor(loggingEvent.Level);
        string threadColor = "\x1B[90m";
        string loggerColor = "\x1B[36m";
        string ndcColor = "\x1B[95m";
        string messageColor = "\x1B[0m";

        string? ndc = loggingEvent.LookupProperty("NDC") as string;
        string ndcPart = !string.IsNullOrEmpty(ndc) 
            ? $"{ndcColor}[{ndc}]{Reset} " 
            : string.Empty;

        return string.Create(null, stackalloc char[256],
          $"{timestampColor}{loggingEvent.TimeStamp:HH:mm:ss.fff}{Reset} " +
          $"{levelColor}{loggingEvent.Level.ToString().PadRight(5)}{Reset} " +
          $"{threadColor}[{loggingEvent.ThreadName}]{Reset} " +
          $"{loggerColor}{loggingEvent.LoggerName}{Reset} " +
          $"{ndcPart}- {messageColor}{loggingEvent.RenderedMessage}{Reset}\n"
        );
    }

    private string GetLevelColor(Level level)
    {
        return level switch
        {
            var l when l == Level.Debug => "\x1B[36m",
            var l when l == Level.Info  => "\x1B[32m",
            var l when l == Level.Warn  => "\x1B[33m",
            var l when l == Level.Error => "\x1B[31m",
            var l when l == Level.Fatal => "\x1B[35m",
            _ => "\x1B[0m"                             
        };
    }

    private static void EnableAnsiOnWindows()
    {
        var stdOut = GetStdHandle(-11);
        GetConsoleMode(stdOut, out uint mode);
        SetConsoleMode(stdOut, mode | 0x0004);
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr handle, out uint mode);
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr handle, uint mode);
    }
}