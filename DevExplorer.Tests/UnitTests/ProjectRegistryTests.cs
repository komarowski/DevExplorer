using DevExplorer.Domain.Attributes;
using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Providers;
using DevExplorer.Infrastructure.Services;
using System.Reflection;

namespace DevExplorer.Tests.UnitTests;

public class ProjectRegistryTests
{
    public ProjectRegistryTests()
    {
        ProjectProvider.Initialize();
    }

    [Fact]
    public void Initialize_ShouldDiscoverAllProjects()
    {
        var projects = ProjectProvider.GetAllProjects();

        Assert.NotEmpty(projects);
        Assert.Contains(projects, p => p.Name == "standard");
    }

    [Fact]
    public void GetProjectsByProjectId_ShouldReturnCorrectProject()
    {
        var projects = ProjectProvider.GetProjectsByProjectId("standard");

        Assert.Single(projects);
        Assert.Equal("standard", projects[0].Name);
    }

    [Fact]
    public void GetMethods_ShouldDiscoverAllMarkedMethods()
    {
        var methods = ProjectProvider.GetMethods("standard");

        Assert.NotEmpty(methods);
        Assert.True(methods.Count >= 3);
        Assert.Contains(methods, m => m.Name == "GetTextLogsAsync");
        Assert.Contains(methods, m => m.Name == "QueryAsync");
        Assert.Contains(methods, m => m.Name == "CallApiAsync");
    }
}
