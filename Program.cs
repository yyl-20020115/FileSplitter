namespace FileSplitter;
public class Program
{
    public const string DefaultName = "default";
    public const string DefaultExtension = ".lua";
    public const char LocalPrefix = '@';
    public const string BarPrefix = "====";
    public static int DefaultIndex = 0;
    public static string EnsurePath(string path)
    {
        path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var parts = path.Split(Path.DirectorySeparatorChar);
        if (parts.Length > 1)
        {
            var full = parts[0];
            for (int i = 1; i < parts.Length - 1; i++)
            {
                full = Path.Combine(full, parts[i]);
                if (!Directory.Exists(full))
                {
                    Directory.CreateDirectory(full);
                }
            }

        }

        return path;
    }
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("FileSplitter [file] [output-dir]");
        }
        else if (args.Length >= 1)
        {
            var file = args[0];
            var output = args.Length > 1 ? args[1] : Environment.CurrentDirectory;
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            if (File.Exists(file))
            {
                using var reader = new StreamReader(file);
                string? line = null;
                var name = string.Empty;
                var splitter = string.Empty;
                while (0 == splitter.Length && null != (line = reader.ReadLine()))
                {
                    if (line.StartsWith(BarPrefix))
                    {
                        splitter = line;
                        name = (reader.ReadLine() ?? string.Empty).Trim();
                        name = name.Length == 0 ? DefaultName + DefaultIndex : name;
                        break;
                    }
                }
                while (null != line && !string.IsNullOrEmpty(name))
                {
                    if (name.StartsWith(LocalPrefix))
                    {
                        name = name[1..];
                    }
                    if (!name.EndsWith(DefaultExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        name += DefaultExtension;
                    }
                    var path = EnsurePath(Path.Combine(output, name));

                    using var writer = new StreamWriter(path);

                    while (0 < splitter.Length && null != (line = reader.ReadLine()))
                    {
                        if (line == splitter)
                        {
                            name = (reader.ReadLine() ?? string.Empty).Trim();
                            name = name.Length == 0 ? DefaultName + DefaultIndex : name;
                            break;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }
    }
}