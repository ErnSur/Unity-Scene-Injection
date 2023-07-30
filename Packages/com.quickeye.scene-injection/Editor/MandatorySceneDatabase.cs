using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuickEye.SceneInjection.FractionalIndexing;
using UnityEditor;

namespace QuickEye.SceneInjection
{
    internal class MandatorySceneDatabase : AssetPostprocessor
    {
        public static event Action DatabaseUpdated;
        private static MandatorySceneConfig[] _mandatoryScenesCache;

        static MandatorySceneDatabase()
        {
            UpdateMandatorySceneCache();
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets
            , string[] movedFromAssetPaths)
        {
            var x = new Stopwatch();
            x.Start();
            if (importedAssets
                .Select(AssetDatabase.LoadAssetAtPath<MandatorySceneConfig>)
                .Any(asset => asset != null))
            {
                Debug.Log($"config was imported");

                UpdateMandatorySceneCache();
            }

            if (deletedAssets.Length > 0)
            {
                UpdateMandatorySceneCache();
            }

            x.Stop();
            Debug.Log($"OnPostprocessAllAssets: {x.Elapsed}");
            Debug.Log(
                $"-- OnPostprocessAllAssets: {importedAssets.Length},{deletedAssets.Length},{movedAssets.Length},{movedFromAssetPaths.Length}");
            foreach (var imported in importedAssets)
            {
                Debug.Log($"Imported {imported}");
            }
        }

        private static void UpdateMandatorySceneCache()
        {
            var x = new Stopwatch();
            x.Start();

            _mandatoryScenesCache = AssetDatabase.FindAssets($"t:{nameof(MandatorySceneConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MandatorySceneConfig>)
                .Where(c => c.scene != null).ToArray();
            x.Stop();
            //Debug.Log($"Cache updated: {x.Elapsed}");
            DatabaseUpdated?.Invoke();
        }

        public static IndexedScene[] GetMandatoryScenes()
        {
            return _mandatoryScenesCache
                .Select(c => new IndexedScene(c.orderKey, AssetDatabase.GetAssetPath(c.scene)))
                .ToArray();
        }

        public static IndexedScene[] GetUserScenes()
        {
            var buildScenes = EditorBuildSettings.scenes;
            return ToScenesWithIndexBounds(buildScenes.Where(s => !IsMandatoryScene(s.path)))
                .ToArray();
        }

        private static IEnumerable<IndexedScene> ToScenesWithIndexBounds(IEnumerable<EditorBuildSettingsScene> scenes)
        {
            var indexedScene = new IndexedScene("5", "");
            foreach (var scene in scenes)
            {
                indexedScene = new IndexedScene(
                    orderKey: OrderKey.Midpoint(indexedScene.OrderKey, "6", "0123456789"),
                    scenePath: scene.path,
                    enabled: scene.enabled
                );
                yield return indexedScene;
            }
        }

        private static bool IsMandatoryScene(string scenePath)
        {
            return GetMandatoryScenes().Any(s => s.ScenePath == scenePath);
        }

        public static void SetDirty(MandatorySceneConfig mandatorySceneConfig)
        {
            AssetDatabase.SaveAssetIfDirty(mandatorySceneConfig);
        }
    }
}