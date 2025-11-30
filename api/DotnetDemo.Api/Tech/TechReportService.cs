using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using DotnetDemo.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotnetDemo.Tech;

public class TechReportService : ITechReportService
{
    private readonly IWebHostEnvironment _environment;
    private readonly DatabaseOptions _databaseOptions;

    public TechReportService(
        IWebHostEnvironment environment,
        IOptions<DatabaseOptions> databaseOptions)
    {
        _environment = environment;
        _databaseOptions = databaseOptions.Value;
    }

    public Task<string> BuildHtmlAsync(CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder();
        builder.Append("""
<!DOCTYPE html>
<html lang="pt-BR">
  <head>
    <meta charset="utf-8" />
    <title>dotnet-demo :: /tech</title>
    <style>
      body { font-family: 'Segoe UI', system-ui, -apple-system, sans-serif; background: #0d1117; color: #e6edf3; margin: 0; padding: 2rem; }
      h1 { margin-top: 0; }
      section { margin-bottom: 2rem; }
      .tech-table { width: 100%; border-collapse: collapse; background: #161b22; border: 1px solid #30363d; }
      .tech-table th, .tech-table td { border: 1px solid #30363d; padding: 0.5rem 0.75rem; text-align: left; vertical-align: top; }
      .tech-table th { width: 20%; background: #1f242d; font-weight: 600; }
      .badge { display: inline-block; padding: 0.15rem 0.4rem; border-radius: 4px; background: #238636; color: #fff; font-size: 0.75rem; }
      pre { white-space: pre-wrap; word-break: break-word; margin: 0; font-family: 'Fira Code', monospace; }
      .card { background: #161b22; border: 1px solid #30363d; border-radius: 6px; padding: 1rem; }
      .card h2 { margin-top: 0; font-size: 1rem; text-transform: uppercase; letter-spacing: 0.1rem; color: #8b949e; }
      table.compact th { width: auto; }
    </style>
  </head>
  <body>
""");

        builder.AppendLine($"    <h1>/tech — dotnet-demo diagnostics</h1>");
        builder.AppendLine($"    <p class=\"badge\">Gerado em {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");

        AppendSection(builder, "Host", GetHostInfo());
        AppendSection(builder, "Runtime", GetRuntimeInfo());
        AppendSection(builder, "Banco de Dados", GetDatabaseInfo());
        AppendSection(builder, "Configuração", GetConfigurationInfo());
        AppendSection(builder, "Variáveis de Ambiente", GetEnvironmentVariables());
        AppendSection(builder, "Assemblies carregados", GetAssembliesInfo());
        AppendLicense(builder);

        builder.Append("""
  </body>
</html>
""");

        return Task.FromResult(builder.ToString());
    }

    private static void AppendSection(StringBuilder builder, string title, IDictionary<string, string> rows)
    {
        builder.AppendLine("""
    <section>
      <div class="card">
""");
        builder.AppendLine($"        <h2>{title}</h2>");
        builder.AppendLine("""
        <table class="tech-table">
          <tbody>
""");
        foreach (var row in rows)
        {
            builder.AppendLine("            <tr>");
            builder.AppendLine($"              <th>{System.Net.WebUtility.HtmlEncode(row.Key)}</th>");
            builder.AppendLine($"              <td>{System.Net.WebUtility.HtmlEncode(row.Value)}</td>");
            builder.AppendLine("            </tr>");
        }

        builder.AppendLine("""
          </tbody>
        </table>
      </div>
    </section>
""");
    }

    private IDictionary<string, string> GetHostInfo()
    {
        return new Dictionary<string, string>
        {
            ["Hostname"] = Environment.MachineName,
            ["OS"] = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            ["Processadores"] = Environment.ProcessorCount.ToString(),
            ["Memória usada (MB)"] = (Process.GetCurrentProcess().WorkingSet64 / 1_048_576d).ToString("N2"),
            ["PID"] = Environment.ProcessId.ToString()
        };
    }

    private IDictionary<string, string> GetRuntimeInfo()
    {
        return new Dictionary<string, string>
        {
            ["Versão .NET"] = Environment.Version.ToString(),
            ["Framework"] = Assembly.GetExecutingAssembly().GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName ?? "N/D",
            ["Ambiente"] = _environment.EnvironmentName,
            ["Diretório de Conteúdo"] = _environment.ContentRootPath
        };
    }

    private IDictionary<string, string> GetDatabaseInfo()
    {
        return new Dictionary<string, string>
        {
            ["Provider"] = _databaseOptions.Provider,
            ["ConnectionString"] = _databaseOptions.ConnectionString
        };
    }

    private IDictionary<string, string> GetConfigurationInfo()
    {
        return new Dictionary<string, string>
        {
            ["Timezone local"] = TimeZoneInfo.Local.DisplayName,
            ["Plataforma"] = Environment.OSVersion.Platform.ToString(),
            ["64 bits"] = Environment.Is64BitProcess.ToString()
        };
    }

    private IDictionary<string, string> GetEnvironmentVariables()
    {
        var result = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = entry.Key?.ToString() ?? string.Empty;
            var value = entry.Value?.ToString() ?? string.Empty;
            if (IsSensitive(key))
            {
                value = "[FILTERED]";
            }

            result[key] = value;
        }

        return result;
    }

    private static bool IsSensitive(string key)
    {
        var keywords = new[] { "SECRET", "PASSWORD", "TOKEN", "KEY", "PWD", "PASS" };
        return keywords.Any(keyword => key.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private IDictionary<string, string> GetAssembliesInfo()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => $"{a.GetName().Name} - {a.GetName().Version}")
            .OrderBy(name => name)
            .ToArray();

        return new Dictionary<string, string>
        {
            ["Assemblies"] = string.Join("\n", assemblies)
        };
    }

    private void AppendLicense(StringBuilder builder)
    {
        var license = LocateLicenseText();
        builder.AppendLine("""
    <section>
      <div class="card">
        <h2>Licença</h2>
        <pre>
""");
        builder.AppendLine(System.Net.WebUtility.HtmlEncode(license));
        builder.AppendLine("""
        </pre>
      </div>
    </section>
""");
    }

    private string LocateLicenseText()
    {
        var possiblePaths = new[]
        {
            Path.Combine(_environment.ContentRootPath, "LICENSE"),
            Path.Combine(_environment.ContentRootPath, "..", "LICENSE")
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
        }

        return "Arquivo LICENSE não encontrado.";
    }
}

