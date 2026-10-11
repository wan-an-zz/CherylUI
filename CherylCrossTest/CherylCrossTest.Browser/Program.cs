using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;
using ReactiveUI.Avalonia;
using CherylCrossTest;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
    private static void Main(string[] args) => BuildAvaloniaApp()
        .UseReactiveUI(_ => { })
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
