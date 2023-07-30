using System;
using System.IO;
using UnityEditor;


public static class BatchTest
{
    [MenuItem("DEBUG/Batch Test")]
    public static void Test()
    {
        var buildScenes = EditorBuildSettings.scenes;
        Console.WriteLine("[HSI] Hello from batch mode, build scenes:");
        foreach (var scene in buildScenes)
        {
            Console.WriteLine($"-- {scene.path}");
        }
    }
}