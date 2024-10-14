using Cake.CloudFormation;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;
using Cake.Common.Tools.DotNet.Build;
using Cake.AWS.ElasticBeanstalk;
using Amazon;
using Cake.AWS.S3;
using Cake.Common.Diagnostics;
using System.Threading.Tasks;
using Cake.Common;

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
    public string S3Bucket { get; set; } = "indg-assignment-base-eu-west-1-418272758451";
    public string ElasticStackName { get; set; } = "indg-image-service";
    public string BaseStackName { get; set; } = "indg-assignment-base";
    public string ApplicationName { get; set; } = "indg-image-service-Application";
    public string ApplicationEnvironmentName { get; set; } = "indg-image-service";
    public string AwsAccessKey { get; set; }
    public string AwsSecretKey { get; set; }
    public string AwsExecutionRole { get; set; }


    public BuildContext(ICakeContext context)
        : base(context)
    {
        context.VerboseVerbosity();

        AwsAccessKey = context.Argument("awsAccessKey", string.Empty);
        AwsSecretKey = context.Argument("awsSecretKey", string.Empty);
        AwsExecutionRole = context.Argument("awsExecutionRole", string.Empty);
    }
}

[TaskName("RestoreNugetPackages")]
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
            Configuration = "Release",
            NoRestore = true
        });
    }
}

[TaskName("DeployBaseCloudFormation")]
[IsDependentOn(typeof(BuildTask))]
public sealed class DeployBaseCloudFormationTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CloudFormationDeploy(new DeployArguments
        {
            RoleArn = context.AwsExecutionRole,
            TemplateFile = context.File("../../deploy/config/cloudFormation/cloudFormationBase.yaml"),
            StackName = context.BaseStackName,
            Capabilities = new System.Collections.Generic.List<string> { { "CAPABILITY_IAM" }, { "CAPABILITY_NAMED_IAM" }, { "CAPABILITY_AUTO_EXPAND" } }
        });
    }
}


[TaskName("UploadWebServiceToS3")]
[IsDependentOn(typeof(DeployBaseCloudFormationTask))]
public sealed class UploadWebServiceToS3Task : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        await context.S3Upload(context.File("../../output/artifacts/INDG.Image.Service-webservice.zip"), "INDG.Image.Service-webservice.zip", new UploadSettings
        {
            Region = RegionEndpoint.EUWest1,
            BucketName = context.S3Bucket,
            AccessKey = context.AwsAccessKey,
            SecretKey = context.AwsSecretKey
        });
    }
}

[TaskName("DeployElasticBeanstalk")]
[IsDependentOn(typeof(UploadWebServiceToS3Task))]
public sealed class DeployElasticBeanstalkTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CloudFormationDeploy(new DeployArguments
        {
            RoleArn = context.AwsExecutionRole,
            TemplateFile = context.File("../../deploy/config/cloudFormation/elasticBeanstalk.yaml"),
            StackName = context.ElasticStackName,
            Capabilities = new System.Collections.Generic.List<string> { { "CAPABILITY_IAM" }, { "CAPABILITY_NAMED_IAM" }, { "CAPABILITY_AUTO_EXPAND" } },
            ParameterOverrides = new System.Collections.Generic.Dictionary<string, string> { { "S3Key", "INDG.Image.Service-webservice.zip" } }
        });
    }
}

[TaskName("DeployWebApplication")]
[IsDependentOn(typeof(DeployElasticBeanstalkTask))]
public sealed class DeployWebApplicationTask : AsyncFrostingTask<BuildContext> 
{ 
    public override async Task RunAsync(BuildContext context)
    {
        var versionLabel = System.Guid.NewGuid().ToString();
        await context.CreateApplicationVersionAsync(context.ApplicationName, $"New Version: {versionLabel}", versionLabel, "indg-assignment-base-eu-west-1-418272758451", "INDG.Image.Service-webservice.zip", false, new ElasticBeanstalkSettings
        {
            Region = RegionEndpoint.EUWest1,
            BucketName = context.S3Bucket,
            AccessKey = context.AwsAccessKey,
            SecretKey = context.AwsSecretKey
        });

        await context.DeployApplicationVersionAsync(context.ApplicationName, context.ApplicationEnvironmentName, versionLabel, new ElasticBeanstalkSettings
        {
            Region = RegionEndpoint.EUWest1,
            BucketName = context.S3Bucket,
            AccessKey = context.AwsAccessKey,
            SecretKey = context.AwsSecretKey
        });
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(DeployWebApplicationTask))]
public class DefaultTask : FrostingTask
{
}