using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddCustomLogger(); // see below class CustomLoggerExtension

var app = builder.Build();

app.MapGet("/", async (r) => { 
	r.RequestServices.GetService<ILoggerFactory>()!.CreateLogger("")!.LogWarning("Look Mom! -> Debug Window!");
	r.Response.StatusCode=200; 
	r.Response.ContentType = "text/plain";
	await r.Response.WriteAsync("hello world"); 
});

app.Run();

[ProviderAlias("Custom")] // <-- Something I can remember (see appsettings.json), but BREAKING CHANGE between v8/9 and v10  
public class CustomLogProviderWithReallyLongNameThatIWillNeverRememberWhenConfiguringAppSettings() : ILoggerProvider
{
	private ConcurrentDictionary<string, CustomLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
	public ILogger CreateLogger(string categoryName)
    { return _loggers.GetOrAdd(categoryName, name => new CustomLogger()); }
	public void Dispose() { _loggers.Clear(); }
}

public static class CustomLoggerExtension
{
	public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder)
	{
		builder.Services.AddSingleton<ILoggerProvider, CustomLogProviderWithReallyLongNameThatIWillNeverRememberWhenConfiguringAppSettings>();
		builder.Services.TryAddEnumerable(
			ServiceDescriptor.Singleton<ILoggerProvider, CustomLogProviderWithReallyLongNameThatIWillNeverRememberWhenConfiguringAppSettings>(
				sp => sp.GetRequiredService<CustomLogProviderWithReallyLongNameThatIWillNeverRememberWhenConfiguringAppSettings>()));
		return builder;
	}
}


/// <summary>
/// Just log to debug window
/// </summary>
public class CustomLogger(): ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;
	public bool IsEnabled(LogLevel logLevel)=>true;
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		System.Diagnostics.Debug.WriteLine("Custom: " + logLevel.ToString() + ": " + formatter(state, exception));
	}
}

