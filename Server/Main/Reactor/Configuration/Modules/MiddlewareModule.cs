using Server.Main.Reactor.Middleware;

namespace Server.Main.Reactor.Configuration.Modules;

public static class MiddlewareModule
{
  public static void Configure(WebApplication app)
  {
    app.UseMiddleware<NDCRequestLoggingMiddleware>();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseWebSockets();
    app.UseRouting();
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

    if (app.Environment.IsProduction() || app.Environment.IsStaging())
    {
      app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.ShowCommonExtensions();
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscription Reactor API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Subscription Reactor API V2");
        c.RoutePrefix = string.Empty;
      });
    }
  }
}
