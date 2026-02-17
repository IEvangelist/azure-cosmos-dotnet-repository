using System.Text;
using System.Text.RegularExpressions;
using Docfx.Dotnet;
using YamlDotNet.RepresentationModel;

namespace Hex1b.DocGenerator;

/// <summary>
/// Generates VitePress-compatible markdown API documentation from the Hex1b library.
/// Uses DocFX as a library for metadata extraction, then converts YAML to clean markdown.
/// </summary>
public partial class Program
{
    public static async Task Main(string[] args)
    {
        var projectRoot = FindProjectRoot();
        var outputDir = Path.Combine(projectRoot, "src", "content", "reference");
        var docGeneratorDir = Path.Combine(projectRoot, "src", "DocGenerator");
        var yamlOutputDir = Path.Combine(docGeneratorDir, "_metadata");
        var docfxJsonPath = Path.Combine(docGeneratorDir, "docfx.json");

        Console.WriteLine($"Project root: {projectRoot}");
        Console.WriteLine($"Output directory: {outputDir}");

        // Ensure the metadata output directory exists
        Directory.CreateDirectory(yamlOutputDir);

        // Step 1: Generate YAML metadata using DocFX library API
        Console.WriteLine("Generating API metadata with DocFX...");

        if (!File.Exists(docfxJsonPath))
        {
            Console.Error.WriteLine($"docfx.json not found at {docfxJsonPath}");
            Environment.Exit(1);
        }

        // Use DocFX as a library to generate metadata
        await DotnetApiCatalog.GenerateManagedReferenceYamlFiles(docfxJsonPath);

        Console.WriteLine("Converting to VitePress markdown...");

        // Keep the index.md file we created manually
        var indexPath = Path.Combine(outputDir, "index.md");
        string? existingIndex = null;
        if (File.Exists(indexPath))
            existingIndex = await File.ReadAllTextAsync(indexPath);

        // Clean output directory (except index.md)
        if (Directory.Exists(outputDir))
        {
            foreach (var file in Directory.GetFiles(outputDir, "*.md"))
            {
                if (Path.GetFileName(file) != "index.md")
                    File.Delete(file);
            }
        }
        else
        {
            Directory.CreateDirectory(outputDir);
        }

        // Restore the index.md
        if (existingIndex != null)
            await File.WriteAllTextAsync(indexPath, existingIndex);

        // Convert YAML to clean markdown
        var converter = new YamlToMarkdownConverter(yamlOutputDir, outputDir);
        await converter.ConvertAllAsync();

        Console.WriteLine("Done!");
    }

    private static string FindProjectRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "Microsoft.Azure.CosmosRepository.slnx")))
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new InvalidOperationException("Could not find solution root (looking for Microsoft.Azure.CosmosRepository.slnx)");
    }
}

/// <summary>
/// Converts DocFX YAML metadata files to VitePress-compatible markdown.
/// </summary>
public partial class YamlToMarkdownConverter(string yamlDir, string outputDir)
{
    private readonly string _yamlDir = yamlDir;
    private readonly string _outputDir = outputDir;
    private readonly Dictionary<string, ApiItem> _items = [];

    public async Task ConvertAllAsync()
    {
        // First pass: load all YAML files and index items by UID
        foreach (var yamlFile in Directory.GetFiles(_yamlDir, "*.yml"))
        {
            if (Path.GetFileName(yamlFile) == "toc.yml")
                continue;

            var content = await File.ReadAllTextAsync(yamlFile);
            ParseYamlFile(content);
        }

        Console.WriteLine($"Loaded {_items.Count} API items");

        // Second pass: generate markdown for each item
        var generatedCount = 0;
        foreach (var item in _items.Values)
        {
            if (item.Type == "Namespace" || IsTopLevelType(item.Type))
            {
                var markdown = GenerateMarkdown(item);
                var fileName = SanitizeFileName(item.Uid) + ".md";
                var filePath = Path.Combine(_outputDir, fileName);
                await File.WriteAllTextAsync(filePath, markdown);
                generatedCount++;
            }
        }

        Console.WriteLine($"Generated {generatedCount} markdown files");
    }

    private static bool IsTopLevelType(string? type) =>
        type is "Class" or "Struct" or "Interface" or "Enum" or "Delegate";

    private void ParseYamlFile(string content)
    {
        // Skip the YAML MIME header
        if (content.StartsWith("### YamlMime:"))
        {
            var firstNewline = content.IndexOf('\n');
            if (firstNewline >= 0)
                content = content[(firstNewline + 1)..];
        }

        using var reader = new StringReader(content);
        var yaml = new YamlStream();

        try
        {
            yaml.Load(reader);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to parse YAML: {ex.Message}");
            return;
        }

        if (yaml.Documents.Count == 0) return;

        var root = yaml.Documents[0].RootNode as YamlMappingNode;
        if (root == null) return;

        // Parse items array
        if (root.Children.TryGetValue(new YamlScalarNode("items"), out var itemsNode) &&
            itemsNode is YamlSequenceNode items)
        {
            foreach (var itemNode in items.OfType<YamlMappingNode>())
            {
                var apiItem = ParseApiItem(itemNode);
                if (apiItem != null)
                    _items[apiItem.Uid] = apiItem;
            }
        }
    }

    private static ApiItem? ParseApiItem(YamlMappingNode node)
    {
        var item = new ApiItem();

        foreach (var (key, value) in node.Children)
        {
            var keyStr = (key as YamlScalarNode)?.Value;
            if (keyStr == null) continue;

            switch (keyStr)
            {
                case "uid":
                    item.Uid = GetScalarValue(value) ?? "";
                    break;
                case "name":
                    item.Name = GetScalarValue(value);
                    break;
                case "nameWithType":
                    item.NameWithType = GetScalarValue(value);
                    break;
                case "fullName":
                    item.FullName = GetScalarValue(value);
                    break;
                case "type":
                    item.Type = GetScalarValue(value);
                    break;
                case "namespace":
                    item.Namespace = GetScalarValue(value);
                    break;
                case "summary":
                    item.Summary = GetScalarValue(value);
                    break;
                case "remarks":
                    item.Remarks = GetScalarValue(value);
                    break;
                case "example":
                    if (value is YamlSequenceNode exampleSeq)
                    {
                        var examples = exampleSeq.OfType<YamlScalarNode>()
                            .Select(n => n.Value ?? "")
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToList();
                        item.Example = string.Join("\n\n", examples);
                    }
                    break;
                case "parent":
                    item.Parent = GetScalarValue(value);
                    break;
                case "syntax":
                    if (value is YamlMappingNode syntaxNode)
                    {
                        foreach (var (sKey, sVal) in syntaxNode.Children)
                        {
                            var sKeyStr = (sKey as YamlScalarNode)?.Value;
                            if (sKeyStr == "content")
                                item.SyntaxContent = GetScalarValue(sVal);
                            else if (sKeyStr == "parameters" && sVal is YamlSequenceNode paramsSeq)
                            {
                                foreach (var param in paramsSeq.OfType<YamlMappingNode>())
                                {
                                    var paramItem = new ParameterItem();
                                    foreach (var (pKey, pVal) in param.Children)
                                    {
                                        var pKeyStr = (pKey as YamlScalarNode)?.Value;
                                        if (pKeyStr == "id")
                                            paramItem.Id = GetScalarValue(pVal);
                                        else if (pKeyStr == "type")
                                            paramItem.Type = GetScalarValue(pVal);
                                        else if (pKeyStr == "description")
                                            paramItem.Description = GetScalarValue(pVal);
                                    }
                                    item.Parameters.Add(paramItem);
                                }
                            }
                            else if (sKeyStr == "return" && sVal is YamlMappingNode returnNode)
                            {
                                foreach (var (rKey, rVal) in returnNode.Children)
                                {
                                    var rKeyStr = (rKey as YamlScalarNode)?.Value;
                                    if (rKeyStr == "type")
                                        item.ReturnType = GetScalarValue(rVal);
                                    else if (rKeyStr == "description")
                                        item.ReturnDescription = GetScalarValue(rVal);
                                }
                            }
                        }
                    }
                    break;
                case "children":
                    if (value is YamlSequenceNode childrenSeq)
                    {
                        foreach (var child in childrenSeq.OfType<YamlScalarNode>())
                            item.Children.Add(child.Value ?? "");
                    }
                    break;
                case "inheritance":
                    if (value is YamlSequenceNode inheritanceSeq)
                    {
                        foreach (var inh in inheritanceSeq.OfType<YamlScalarNode>())
                            item.Inheritance.Add(inh.Value ?? "");
                    }
                    break;
                case "implements":
                    if (value is YamlSequenceNode implementsSeq)
                    {
                        foreach (var impl in implementsSeq.OfType<YamlScalarNode>())
                            item.Implements.Add(impl.Value ?? "");
                    }
                    break;
            }
        }

        return string.IsNullOrEmpty(item.Uid) ? null : item;
    }

    private static string? GetScalarValue(YamlNode node) =>
        (node as YamlScalarNode)?.Value;

    private string GenerateMarkdown(ApiItem item)
    {
        var sb = new StringBuilder();

        // Frontmatter
        sb.AppendLine("---");
        sb.AppendLine($"title: \"{EscapeYaml(item.Name ?? item.Uid)}\"");
        sb.AppendLine($"description: \"{EscapeYaml(CleanSummary(item.Summary) ?? "")}\"");
        sb.AppendLine($"api_type: \"{item.Type?.ToLowerInvariant() ?? "unknown"}\"");
        sb.AppendLine($"namespace: \"{item.Namespace ?? ""}\"");
        sb.AppendLine("---");
        sb.AppendLine();

        // Title - escape generics for VitePress/Vue
        sb.AppendLine($"# {EscapeGenerics(item.Name ?? item.Uid)}");
        sb.AppendLine();

        // Namespace and assembly
        if (!string.IsNullOrEmpty(item.Namespace))
        {
            sb.AppendLine($"**Namespace:** {item.Namespace}");
            sb.AppendLine();
        }
        sb.AppendLine("**Assembly:** Hex1b.dll");
        sb.AppendLine();

        // Summary
        if (!string.IsNullOrEmpty(item.Summary))
        {
            sb.AppendLine(ConvertXmlToMarkdown(item.Summary));
            sb.AppendLine();
        }

        // Syntax
        if (!string.IsNullOrEmpty(item.SyntaxContent))
        {
            sb.AppendLine("```csharp");
            sb.AppendLine(item.SyntaxContent);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        // Type parameters (for generics)
        // TODO: Parse type parameters from syntax

        // Inheritance
        if (item.Inheritance.Count > 0)
        {
            sb.AppendLine("## Inheritance");
            sb.AppendLine();
            var inheritanceChain = item.Inheritance
                .Select(uid => FormatTypeLink(uid))
                .ToList();
            inheritanceChain.Add($"**{EscapeGenerics(item.Name ?? item.Uid)}**");
            sb.AppendLine(string.Join(" → ", inheritanceChain));
            sb.AppendLine();
        }

        // Implements
        if (item.Implements.Count > 0)
        {
            sb.AppendLine("## Implements");
            sb.AppendLine();
            foreach (var impl in item.Implements)
            {
                sb.AppendLine($"- {FormatTypeLink(impl)}");
            }
            sb.AppendLine();
        }

        // Children (for namespaces and types)
        if (item.Children.Count > 0)
        {
            var childItems = item.Children
                .Select(uid => _items.GetValueOrDefault(uid))
                .Where(c => c != null)
                .Cast<ApiItem>()
                .GroupBy(c => c.Type)
                .OrderBy(g => GetTypeOrder(g.Key));

            foreach (var group in childItems)
            {
                var sectionTitle = GetSectionTitle(group.Key);
                sb.AppendLine($"## {sectionTitle}");
                sb.AppendLine();

                foreach (var child in group.OrderBy(c => c.Name))
                {
                    if (IsTopLevelType(child.Type))
                    {
                        // Link to separate page
                        sb.AppendLine($"### [{EscapeGenerics(child.Name ?? child.Uid)}]({SanitizeFileName(child.Uid)}.md)");
                    }
                    else
                    {
                        // Inline member
                        sb.AppendLine($"### {EscapeGenerics(child.Name ?? child.Uid)}");
                    }
                    sb.AppendLine();

                    if (!string.IsNullOrEmpty(child.Summary))
                    {
                        sb.AppendLine(ConvertXmlToMarkdown(child.Summary));
                        sb.AppendLine();
                    }

                    // Parameters
                    if (child.Parameters.Count > 0)
                    {
                        sb.AppendLine("**Parameters:**");
                        sb.AppendLine();
                        foreach (var param in child.Parameters)
                        {
                            var typeLink = !string.IsNullOrEmpty(param.Type) ? FormatTypeLink(param.Type) : "";
                            sb.AppendLine($"- `{param.Id}` ({typeLink}): {ConvertXmlToMarkdown(param.Description ?? "")}");
                        }
                        sb.AppendLine();
                    }

                    // Return type
                    if (!string.IsNullOrEmpty(child.ReturnType))
                    {
                        sb.AppendLine($"**Returns:** {FormatTypeLink(child.ReturnType)}");
                        if (!string.IsNullOrEmpty(child.ReturnDescription))
                        {
                            sb.AppendLine();
                            sb.AppendLine(ConvertXmlToMarkdown(child.ReturnDescription));
                        }
                        sb.AppendLine();
                    }

                    if (!string.IsNullOrEmpty(child.SyntaxContent))
                    {
                        sb.AppendLine("```csharp");
                        sb.AppendLine(child.SyntaxContent);
                        sb.AppendLine("```");
                        sb.AppendLine();
                    }
                }
            }
        }

        // Remarks
        if (!string.IsNullOrEmpty(item.Remarks))
        {
            sb.AppendLine("## Remarks");
            sb.AppendLine();
            sb.AppendLine(ConvertXmlToMarkdown(item.Remarks));
            sb.AppendLine();
        }

        // Examples
        if (!string.IsNullOrEmpty(item.Example))
        {
            sb.AppendLine("## Examples");
            sb.AppendLine();
            sb.AppendLine(ConvertXmlToMarkdown(item.Example));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string EscapeYaml(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", "");

    private static string EscapeGenerics(string value) =>
        value.Replace("<", "\\<").Replace(">", "\\>");

    private static string? CleanSummary(string? summary)
    {
        if (string.IsNullOrEmpty(summary)) return null;
        // Remove XML tags and normalize whitespace
        var clean = XmlTagRegex().Replace(summary, "");
        clean = WhitespaceRegex().Replace(clean, " ").Trim();
        return clean;
    }

    private string FormatTypeLink(string uid)
    {
        // Handle generic parameters in uid like System.IEquatable{Hex1b.Input.Hex1bEvent}
        var displayUid = uid.Replace('{', '<').Replace('}', '>');

        // Extract the base type name (before generic params) and the generic params separately
        var genericStart = uid.IndexOf('{');
        var baseUid = genericStart >= 0 ? uid[..genericStart] : uid;
        var genericParams = genericStart >= 0 ? uid[genericStart..].Replace('{', '<').Replace('}', '>') : "";

        // Get the short name of the base type (last segment before generics)
        var baseDisplayName = baseUid.Split('.').Last();

        // Format the generic type arguments nicely
        var formattedGenericParams = "";
        if (!string.IsNullOrEmpty(genericParams))
        {
            // Extract the inner types and format them with short names
            var innerTypes = genericParams[1..^1].Split(',');
            var formattedInner = innerTypes.Select(t => t.Trim().Split('.').Last());
            formattedGenericParams = $"<{string.Join(", ", formattedInner)}>";
        }

        var displayName = baseDisplayName + formattedGenericParams;

        // External types (System.*)
        if (uid.StartsWith("System."))
        {
            // Convert to MS Docs format: handle generic arity for URL
            var baseType = baseUid.Split('`')[0];
            var msDocsUid = baseType.ToLowerInvariant();
            // For generic types, add -1, -2 etc. suffix based on arity
            if (genericStart >= 0)
            {
                var arity = genericParams.Count(c => c == ',') + 1;
                msDocsUid += $"-{arity}";
            }
            return $"[{EscapeGenerics(displayName)}](https://learn.microsoft.com/dotnet/api/{msDocsUid})";
        }

        // Internal types - try the base type without generics first
        if (_items.ContainsKey(baseUid))
        {
            return $"[{EscapeGenerics(displayName)}]({SanitizeFileName(baseUid)}.md)";
        }

        // Try with full uid
        if (_items.ContainsKey(uid))
        {
            return $"[{EscapeGenerics(displayName)}]({SanitizeFileName(uid)}.md)";
        }

        return EscapeGenerics(displayName);
    }

    private string ConvertXmlToMarkdown(string xml)
    {
        var result = xml;

        // Convert HTML code blocks first (from DocFX)
        result = HtmlCodeBlockRegex().Replace(result, m =>
        {
            var code = m.Groups[1].Value
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&amp;", "&")
                .Replace("&quot;", "\"");
            return $"\n```csharp\n{code}\n```\n";
        });

        // Convert <p> tags to paragraphs
        result = result.Replace("<p>", "\n\n").Replace("</p>", "\n\n");

        // Convert <pre> tags
        result = HtmlPreRegex().Replace(result, "\n```\n$1\n```\n");

        // Convert <see cref="X"/> to links
        result = SeeCrefRegex().Replace(result, m =>
        {
            var cref = m.Groups[1].Value;
            return FormatTypeLink(cref);
        });

        // Convert <paramref name="X"/> to code
        result = ParamRefRegex().Replace(result, "`$1`");

        // Convert <typeparamref name="X"/> to code
        result = TypeParamRefRegex().Replace(result, "`$1`");

        // Convert <c>X</c> to inline code
        result = CCodeRegex().Replace(result, "`$1`");

        // Convert <code>X</code> to inline code (DocFX converts <c> to <code>)
        result = CodeBlockRegex().Replace(result, "`$1`");

        // Remove remaining XML/HTML tags
        result = XmlTagRegex().Replace(result, "");

        // Clean up multiple blank lines
        result = MultipleNewlinesRegex().Replace(result, "\n\n");

        return result.Trim();
    }

    private static int GetTypeOrder(string? type) => type switch
    {
        "Class" => 1,
        "Struct" => 2,
        "Interface" => 3,
        "Enum" => 4,
        "Delegate" => 5,
        "Constructor" => 10,
        "Property" => 11,
        "Method" => 12,
        "Field" => 13,
        "Event" => 14,
        _ => 99
    };

    private static string GetSectionTitle(string? type) => type switch
    {
        "Class" => "Classes",
        "Struct" => "Structs",
        "Interface" => "Interfaces",
        "Enum" => "Enums",
        "Delegate" => "Delegates",
        "Constructor" => "Constructors",
        "Property" => "Properties",
        "Method" => "Methods",
        "Field" => "Fields",
        "Event" => "Events",
        _ => "Members"
    };

    private static string SanitizeFileName(string uid) =>
        uid.Replace('`', '-').Replace('<', '-').Replace('>', '-').Replace(',', '-').Replace('{', '-').Replace('}', '-');

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex XmlTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"<see\s+cref=""([^""]+)""\s*/>")]
    private static partial Regex SeeCrefRegex();

    [GeneratedRegex(@"<paramref\s+name=""([^""]+)""\s*/>")]
    private static partial Regex ParamRefRegex();

    [GeneratedRegex(@"<typeparamref\s+name=""([^""]+)""\s*/>")]
    private static partial Regex TypeParamRefRegex();

    [GeneratedRegex(@"<c>([^<]+)</c>")]
    private static partial Regex CCodeRegex();

    [GeneratedRegex(@"<code>([^<]+)</code>", RegexOptions.Singleline)]
    private static partial Regex CodeBlockRegex();

    [GeneratedRegex(@"<pre><code[^>]*>(.+?)</code></pre>", RegexOptions.Singleline)]
    private static partial Regex HtmlCodeBlockRegex();

    [GeneratedRegex(@"<pre>(.+?)</pre>", RegexOptions.Singleline)]
    private static partial Regex HtmlPreRegex();

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex MultipleNewlinesRegex();
}

/// <summary>
/// Represents an API item parsed from DocFX YAML.
/// </summary>
public class ApiItem
{
    public string Uid { get; set; } = "";
    public string? Name { get; set; }
    public string? NameWithType { get; set; }
    public string? FullName { get; set; }
    public string? Type { get; set; }
    public string? Namespace { get; set; }
    public string? Summary { get; set; }
    public string? Remarks { get; set; }
    public string? Example { get; set; }
    public string? SyntaxContent { get; set; }
    public string? Parent { get; set; }
    public string? ReturnType { get; set; }
    public string? ReturnDescription { get; set; }
    public List<string> Children { get; } = [];
    public List<string> Inheritance { get; } = [];
    public List<string> Implements { get; } = [];
    public List<ParameterItem> Parameters { get; } = [];
}

/// <summary>
/// Represents a method/constructor parameter.
/// </summary>
public class ParameterItem
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
}