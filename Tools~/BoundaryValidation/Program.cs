using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Xml.Linq;

if (args.Length != 3)
{
    Console.Error.WriteLine(
        "Usage: BoundaryValidation <project.csproj> <source-directory> <assembly.dll>");
    return 2;
}

string projectPath = Path.GetFullPath(args[0]);
string sourceDirectory = Path.GetFullPath(args[1]);
string assemblyPath = Path.GetFullPath(args[2]);
var errors = new List<string>();

ValidateProject(projectPath, errors);
ValidateSources(sourceDirectory, errors);
ValidateAssemblyReferences(assemblyPath, errors);

if (errors.Count > 0)
{
    Console.Error.WriteLine("Package boundary validation failed:");
    foreach (string error in errors)
    {
        Console.Error.WriteLine($"- {error}");
    }

    return 1;
}

Console.WriteLine(
    "Package boundary validation passed: Jeomseon.Core has no Unity dependency.");
return 0;

static void ValidateProject(string projectPath, ICollection<string> errors)
{
    XDocument project = XDocument.Load(projectPath, LoadOptions.SetLineInfo);
    string[] targetFrameworks = project
        .Descendants()
        .Where(element =>
            element.Name.LocalName is "TargetFramework" or "TargetFrameworks")
        .SelectMany(element => element.Value.Split(';', StringSplitOptions.RemoveEmptyEntries))
        .Select(value => value.Trim())
        .ToArray();

    if (targetFrameworks.Length != 1 || targetFrameworks[0] != "netstandard2.1")
    {
        errors.Add(
            $"{projectPath} must target only netstandard2.1; found: " +
            string.Join(", ", targetFrameworks));
    }

    foreach (XElement item in project.Descendants().Where(element =>
                 element.Name.LocalName is "Reference" or "PackageReference" or "ProjectReference"))
    {
        string include = item.Attribute("Include")?.Value ?? string.Empty;
        if (IsUnityIdentifier(include))
        {
            errors.Add(
                $"{projectPath} contains forbidden {item.Name.LocalName}: {include}");
        }
    }

    foreach (XElement constants in project.Descendants().Where(element =>
                 element.Name.LocalName == "DefineConstants"))
    {
        if (Regex.IsMatch(constants.Value, @"(?:^|;)\s*UNITY(?:_|;|$)"))
        {
            errors.Add($"{projectPath} defines a Unity compilation symbol.");
        }
    }
}

static void ValidateSources(string sourceDirectory, ICollection<string> errors)
{
    var unityDirective = new Regex(
        @"^\s*#\s*(?:if|elif)\b[^\r\n]*\bUNITY(?:_[A-Z0-9_]+)?\b",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    foreach (string sourcePath in Directory.EnumerateFiles(
                 sourceDirectory,
                 "*.cs",
                 SearchOption.AllDirectories))
    {
        string source = File.ReadAllText(sourcePath);
        if (unityDirective.IsMatch(source))
        {
            errors.Add($"{sourcePath} contains a Unity conditional compilation directive.");
        }
    }
}

static void ValidateAssemblyReferences(string assemblyPath, ICollection<string> errors)
{
    using FileStream stream = File.OpenRead(assemblyPath);
    using var peReader = new PEReader(stream);
    if (!peReader.HasMetadata)
    {
        errors.Add($"{assemblyPath} is not a managed assembly.");
        return;
    }

    MetadataReader metadata = peReader.GetMetadataReader();
    foreach (AssemblyReferenceHandle handle in metadata.AssemblyReferences)
    {
        AssemblyReference reference = metadata.GetAssemblyReference(handle);
        string name = metadata.GetString(reference.Name);
        if (IsUnityIdentifier(name))
        {
            errors.Add($"{assemblyPath} references forbidden assembly {name}.");
        }
    }
}

static bool IsUnityIdentifier(string value)
{
    return value.Equals("Unity", StringComparison.OrdinalIgnoreCase) ||
           value.StartsWith("Unity.", StringComparison.OrdinalIgnoreCase) ||
           value.StartsWith("UnityEngine", StringComparison.OrdinalIgnoreCase) ||
           value.StartsWith("UnityEditor", StringComparison.OrdinalIgnoreCase);
}
