using DevExplorer.Domain.Abstractions;
using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Providers;
using Microsoft.Extensions.Options;

namespace DevExplorer.Tests;

/// <summary>
/// Test fixture that sets up the ProjectSettingsService for tests.
/// </summary>
public class TestProjectSettingsFixture : IDisposable
{
    public IProjectSettingsService SettingsService { get; }

    public TestProjectSettingsFixture()
    {
        var options = Options.Create(new EnvironmentSettingsOptions
        {
            DefaultEnvironment = "Develop",
            Environments = new Dictionary<string, EnvironmentSettings>
            {
                ["Develop"] = new()
                {
                    Projects =
                    [
                        new() { Name = "demo", LogDirectory = "demo\\logs", Settings = [] },
                        new() { Name = "demo2", LogDirectory = "demo2\\logs", ConnectionString = "Server=test;Database=test;", Settings = new() { ["ApiToken"] = "test-token" } },
                    ]
                }
            }
        });

        SettingsService = new TestProjectSettingsService(options);
        ProjectBase.SetProjectSettingsService(SettingsService);
        ProjectProvider.SetProjectSettingsService(SettingsService);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Simple implementation of IProjectSettingsService for tests.
/// </summary>
internal class TestProjectSettingsService : IProjectSettingsService
{
    private readonly EnvironmentSettingsOptions _options;
    private string _currentEnvironment;

    public TestProjectSettingsService(IOptions<EnvironmentSettingsOptions> options)
    {
        _options = options.Value;
        _currentEnvironment = _options.DefaultEnvironment;
    }

    public string CurrentEnvironment
    {
        get => _currentEnvironment;
        set
        {
            if (!_currentEnvironment.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                _currentEnvironment = value;
                EnvironmentChanged?.Invoke(value);
            }
        }
    }

    public event Action<string>? EnvironmentChanged;

    public IReadOnlyList<string> AvailableEnvironments =>
        _options.Environments.Keys.ToList().AsReadOnly();

    public ProjectSettings? GetProjectSettings(string projectName)
    {
        if (!_options.Environments.TryGetValue(_currentEnvironment, out var envConfig))
            return null;

        return envConfig.Projects.FirstOrDefault(p =>
            p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<ProjectSettings> GetAllProjectSettings()
    {
        if (!_options.Environments.TryGetValue(_currentEnvironment, out var envConfig))
            return [];

        return envConfig.Projects.AsReadOnly();
    }
}
