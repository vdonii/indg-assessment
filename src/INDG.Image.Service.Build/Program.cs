using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using System.Linq;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string OutputDir { get; set; }
    public string ArtifactsDir { get; set; }
    public string MsBuildConfiguration { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Release");
        OutputDir = context.Directory("../../output");
        ArtifactsDir = OutputDir + context.Directory("artifacts");
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"../**/bin/Release");
        context.CleanDirectory($"../../output");
    }
}

[TaskName("RestoreNugetPackages")]
[IsDependentOn(typeof(CleanTask))]
public sealed class RestoreNugetPackagesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var restoreSettings = new DotNetRestoreSettings
        {
            NoCache = true
        };

        context.DotNetRestore("../..", restoreSettings);
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(RestoreNugetPackagesTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild("../../INDG.Image.Service.sln", new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration,
            NoRestore = true
        });
    }
}

[TaskName("RunUnitTests")]
[IsDependentOn(typeof(BuildTask))]
public sealed class RunUnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var testResultDir = context.MakeAbsolute(context.Directory("../../test-results"));
        context.EnsureDirectoryExists(testResultDir);

        var testProjects = context.GetFiles("../../test/**/*Tests.csproj");
        var testProjectNames = testProjects.Select(x => x.GetFilenameWithoutExtension().FullPath).ToList();

        var paths = testProjectNames.Select(x => (FilePath)$"../../test/{x}/bin/Release/net8.0/{x}.dll").ToList();

        foreach ( var path in paths)
        {
            context.Information(path);
        }

        var processArgumentsBuilder = new ProcessArgumentBuilder();
        processArgumentsBuilder.Append("vstest");
        processArgumentsBuilder.Append(string.Join(" ", paths));
        processArgumentsBuilder.Append("--parallel");
        processArgumentsBuilder.Append("--logger:\"trx\"");
        processArgumentsBuilder.Append($"--TestAdapterPath:../test/{testProjectNames.First()}/bin/Release/net8.0");
        processArgumentsBuilder.Append($"--ResultsDirectory:{testResultDir}");

        context.DotNetTool("../", processArgumentsBuilder.Render());

        context.CopyFiles($"{testResultDir}/TestResults/*.trx", testResultDir);
    }
}

[TaskName("CreateArtifact")]
[IsDependentOn(typeof(RunUnitTestsTask))]
public sealed class CreateArtifactTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var serviceName = "INDG.Image.Service";
        context.Information($"Creating {serviceName}");

        var outputDirectory = context.ArtifactsDir + context.Directory(serviceName);

        context.DotNetPublish(context.Directory("../../src") + context.Directory(serviceName), new DotNetPublishSettings
        {
            Configuration = context.MsBuildConfiguration,
            OutputDirectory = outputDirectory,
            NoRestore = true,
            NoBuild = true
        });

        context.Zip(outputDirectory, context.ArtifactsDir + context.File($"/{serviceName}.zip"));

        var webServiceDirectory = outputDirectory + context.Directory(serviceName + "-webservice");
        context.CreateDirectory(webServiceDirectory);
        context.CopyFileToDirectory(context.File("../../build/aws-windows-deployment-manifest.json"), webServiceDirectory);
        context.CopyFileToDirectory(context.ArtifactsDir + context.File($"/{serviceName}.zip"), webServiceDirectory);

        context.Zip(webServiceDirectory, context.ArtifactsDir + context.File($"/{serviceName}-webservice.zip"));
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(CreateArtifactTask))]
public class DefaultTask : FrostingTask
{
}