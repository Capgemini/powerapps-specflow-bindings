#addin nuget:?package=Cake.Npm&version=0.13.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = Directory("./src/Example/bin") + Directory(configuration);
var npmInstallSettings = new NpmInstallSettings {
    WorkingDirectory = Directory("./src/Capgemini.Test.Xrm")
};
Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("./src/Capgemini.Test.Xrm.sln");
});

Task("Restore-Npm-Packages")
    .Does(() =>
{
    NpmInstall(npmInstallSettings);
});

Task("Default")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Restore-Npm-Packages")
    .Does(() =>
{
    MSBuild("./src/Capgemini.Test.Xrm.sln", settings =>
        settings.SetConfiguration(configuration));
});

RunTarget(target);