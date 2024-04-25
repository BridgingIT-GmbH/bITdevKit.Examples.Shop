namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System.IO;
using System.Reflection;

public static class EmbeddedResource // TODO: move to Common.Utilities
{
    public static string GetContent(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var baseName = assembly.GetName().Name;
        var resourceName = path
            .TrimStart('.')
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
        var fullPath = baseName + "." + resourceName;

        using var stream = assembly.GetManifestResourceStream(fullPath) ?? throw new FileNotFoundException($"Embedded resource {fullPath} not found");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}