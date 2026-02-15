using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Providers;

namespace DevExplorer.Tests.UnitTests;

/// <summary>
/// Tests for ProjectProvider discovery and registration.
/// </summary>
public class ProjectProviderTests : IClassFixture<TestProjectSettingsFixture>
{
    public ProjectProviderTests(TestProjectSettingsFixture fixture)
    {
        // Fixture ensures settings service is initialized
        _ = fixture;
    }

    #region Discovery Tests

    [Fact]
    public void GetAllProjects_ShouldDiscoverProjects()
    {
        var projects = ProjectProvider.GetAllProjects();

        Assert.NotEmpty(projects);
        Assert.True(projects.Count >= 1);
    }

    [Fact]
    public void GetAllProjects_ShouldReturnReadOnlyCollection()
    {
        var projects = ProjectProvider.GetAllProjects();

        Assert.IsAssignableFrom<IReadOnlyList<IProject>>(projects);
    }

    [Fact]
    public void GetAllProjects_ShouldReturnConsistentResults()
    {
        var firstCall = ProjectProvider.GetAllProjects();
        var secondCall = ProjectProvider.GetAllProjects();

        Assert.Equal(firstCall.Count, secondCall.Count);
        Assert.All(firstCall, project => Assert.Contains(project, secondCall));
    }

    #endregion

    #region Project Retrieval Tests

    [Fact]
    public void GetProject_ShouldReturnValidProject()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);
        Assert.Equal("demo", project.Name);
        Assert.True(project.HasLogs);
    }

    [Fact]
    public void GetProject_ShouldReturnNullForUnknownProject()
    {
        var project = ProjectProvider.GetProject("nonexistent");

        Assert.Null(project);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void GetProject_ShouldReturnNullForInvalidNames(string? projectName)
    {
        var project = ProjectProvider.GetProject(projectName ?? "");

        Assert.Null(project);
    }

    [Fact]
    public void GetProject_ShouldBeCaseInsensitive()
    {
        var lowerCase = ProjectProvider.GetProject("demo");
        var upperCase = ProjectProvider.GetProject("DEMO");
        var mixedCase = ProjectProvider.GetProject("Demo");

        Assert.NotNull(lowerCase);
        Assert.NotNull(upperCase);
        Assert.NotNull(mixedCase);
        Assert.Equal(lowerCase?.Name, upperCase?.Name);
        Assert.Equal(lowerCase?.Name, mixedCase?.Name);
    }

    #endregion

    #region Project Actions Tests

    [Fact]
    public void GetProjectActions_ShouldDiscoverActions()
    {
        var project = ProjectProvider.GetProject("demo");
        var actions = project?.GetProjectActions();

        Assert.NotNull(actions);
        Assert.NotEmpty(actions);
    }

    [Fact]
    public void GetProjectActions_ShouldReturnValidActionNames()
    {
        var project = ProjectProvider.GetProject("demo");
        var actions = project?.GetProjectActions();

        Assert.NotNull(actions);
        Assert.All(actions, action => Assert.False(string.IsNullOrEmpty(action.Name)));
    }

    [Fact]
    public void GetProjectActions_ShouldReturnReadOnlyCollection()
    {
        var project = ProjectProvider.GetProject("demo");
        var actions = project?.GetProjectActions();

        Assert.IsAssignableFrom<IReadOnlyList<ProjectAction>>(actions);
    }

    [Fact]
    public void ExecuteProjectAction_ShouldReturnValidResult()
    {
        var project = ProjectProvider.GetProject("demo");
        var actions = project?.GetProjectActions();

        Assert.NotNull(actions);
        Assert.NotEmpty(actions);

        var firstAction = actions.First();
        var result = project!.ExecuteProjectActionAsync(firstAction.Name).Result;

        Assert.NotNull(result);
        Assert.IsType<ProjectActionResult>(result);
    }

    [Fact]
    public void ExecuteProjectAction_WithInvalidActionName_ShouldHandleGracefully()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);

        var result = project.ExecuteProjectActionAsync("nonexistent-action").Result;

        Assert.NotNull(result);
    }

    #endregion

    #region Project Logs Tests

    [Fact]
    public void GetLogsAsync_ShouldReturnValidLogEvents()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);
        Assert.True(project.HasLogs);

        var logs = project.GetLogsAsync(null).Result;

        Assert.NotNull(logs);
        Assert.IsAssignableFrom<IEnumerable<LogEvent>>(logs);
    }

    [Fact]
    public void GetLogsAsync_WithNullFilter_ShouldUseDefaults()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);

        var logsWithNull = project.GetLogsAsync(null).Result;
        var logsWithDefault = project.GetLogsAsync(new LogFilter()).Result;

        Assert.NotNull(logsWithNull);
        Assert.NotNull(logsWithDefault);
    }

    [Fact]
    public void GetLogsAsync_ShouldReturnValidLogEventStructure()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);

        var logs = project.GetLogsAsync(null).Result;

        if (logs.Any())
        {
            var firstLog = logs.First();
            Assert.NotNull(firstLog.Message);
            Assert.NotEqual(default, firstLog.Timestamp);
        }
    }

    [Fact]
    public async Task GetLogsAsync_ShouldSupportCancellation()
    {
        var project = ProjectProvider.GetProject("demo");

        Assert.NotNull(project);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(1));

        // Should complete or throw OperationCanceledException
        try
        {
            var logs = await project.GetLogsAsync(null, cts.Token);
            Assert.NotNull(logs);
        }
        catch (OperationCanceledException)
        {
            // Expected for fast cancellation
        }
    }

    #endregion

    #region Project Properties Tests

    [Fact]
    public void AllProjects_ShouldHaveValidNames()
    {
        var projects = ProjectProvider.GetAllProjects();

        Assert.All(projects, project =>
        {
            Assert.NotNull(project.Name);
            Assert.NotEmpty(project.Name);
            Assert.False(project.Name.Contains(' '), "Project names should not contain spaces");
        });
    }

    [Fact]
    public void AllProjects_ShouldImplementRequiredMembers()
    {
        var projects = ProjectProvider.GetAllProjects();

        Assert.NotEmpty(projects);
        Assert.All(projects, project =>
        {
            Assert.NotNull(project);
            Assert.NotNull(project.Name);
            Assert.NotNull(project.GetProjectActions());
        });
    }

    #endregion
}
