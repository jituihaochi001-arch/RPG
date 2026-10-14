using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Command line entry point for building the Windows player.
/// Usage: Unity.exe -batchmode -quit -executeMethod BuildWindows.Build
/// </summary>
public static class BuildWindows
{
    private const string OutputDirectory = "Builds/Windows";
    private const string ExecutableName = "RPG.exe";

    public static void Build()
    {
        // Use the enabled scenes from Build Settings instead of hard coding the scene list.
        string[] scenes = Array.ConvertAll(
            Array.FindAll(EditorBuildSettings.scenes, scene => scene.enabled),
            scene => scene.path);

        if (scenes.Length == 0)
        {
            throw new Exception("Build failed: no enabled scenes found in Build Settings.");
        }

        Directory.CreateDirectory(OutputDirectory);
        string outputPath = Path.Combine(OutputDirectory, ExecutableName);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Build failed: {summary.result}, errors={summary.totalErrors}");
        }

        Debug.Log($"Build succeeded: {outputPath}, size={summary.totalSize} bytes");
    }
}
