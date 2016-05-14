# GZip
GZip Middleware for ASP.NET Core

Insert the following code into Configure(...) in Startup.cs right before app.UseMvc(....);

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
      ...
      app.UseGZipMiddleware();

      app.UseMvc(routes =>
          ...
      );
}
```