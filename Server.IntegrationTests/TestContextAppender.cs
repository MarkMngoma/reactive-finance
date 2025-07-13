using log4net.Appender;
using log4net.Core;
using NUnit.Framework;

namespace Server.IntegrationTests;

public class TestContextAppender: AppenderSkeleton
{
  protected override void Append(LoggingEvent loggingEvent)
  {
    TestContext.Out.WriteLine(RenderLoggingEvent(loggingEvent));
  }
}
