using System;
using System.IO;

namespace FruitNinja.Port;

internal static class DesktopStorage
{
    private static readonly string Root = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FruitNinja");

    static DesktopStorage()
    {
        Directory.CreateDirectory(Root);
    }

    public static byte[] ReadAllBytes(string fileName)
    {
        string path = Path.Combine(Root, fileName.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(path))
            return null;
        return File.ReadAllBytes(path);
    }

    public static void WriteAllBytes(string fileName, byte[] data)
    {
        string path = Path.Combine(Root, fileName.Replace('/', Path.DirectorySeparatorChar));
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, data);
    }

    public static bool FileExists(string fileName)
    {
        string path = Path.Combine(Root, fileName.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(path);
    }

    public static void CreateDirectory(string dirName)
    {
        string path = Path.Combine(Root, dirName.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(path);
    }

    public static bool DirectoryExists(string dirName)
    {
        string path = Path.Combine(Root, dirName.Replace('/', Path.DirectorySeparatorChar));
        return Directory.Exists(path);
    }

    public static Stream OpenFile(string fileName, FileMode mode)
    {
        string path = Path.Combine(Root, fileName.Replace('/', Path.DirectorySeparatorChar));
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        return File.Open(path, mode);
    }
}
