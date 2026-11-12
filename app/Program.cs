using System.Globalization;

namespace Werkdagboek;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var dutch = CultureInfo.GetCultureInfo("nl-NL");
        CultureInfo.DefaultThreadCurrentCulture = dutch;
        CultureInfo.DefaultThreadCurrentUICulture = dutch;
        Thread.CurrentThread.CurrentCulture = dutch;
        Thread.CurrentThread.CurrentUICulture = dutch;

        ApplicationConfiguration.Initialize();
        Application.Run(new WerkdagForm());
    }
}
