using System;
using System.IO;
using Mortar;
using Microsoft.Xna.Framework;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("FruitNinja")]

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        try
        {
            using var game = new TheGame();
            game.Run();
        }
        catch (Exception ex)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "FruitNinja-crash.log");
            File.WriteAllText(logPath, ex.ToString());
            throw;
        }
    }
}
