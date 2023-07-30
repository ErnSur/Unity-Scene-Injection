using System.Linq;
using UnityEditor;

namespace QuickEye.SceneInjection
{
    internal class SceneInjector : AssetPostprocessor
    {
        private static bool _oneTimeAssetsLoadedInvoked;
        
        /// <summary>
        /// In batch mode it is modifying the EditorBuildSettings.scenes
        /// from InitializeOnLoad method has no effect
        /// </summary>
        [InitializeOnLoadMethod]
        private static void RegisterCallbacks()
        {
            EditorBuildSettings.sceneListChanged += ReInjectMandatoryScenes;
            MandatorySceneDatabase.DatabaseUpdated += ReInjectMandatoryScenes;
            Debug.Log($"InitializeOnLoadMethod");
            TryLoadObject("InitializeOnLoadMethod");
            //ReInjectMandatoryScenes();
        }

        private static void TryLoadObject(string source)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                "Assets/Scenes/Mandatory Scene Config 3.asset");
            Debug.Log($"Can load asset {obj != null} | {source}");
        }

        // private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets
        //     , string[] movedFromAssetPaths)
        // {
        //     ReInjectMandatoryScenes();
        //     Debug.Log("Init loaded assets");
        // }

        [MenuItem("Debug/Re Inject")]
        private static void ReInjectMandatoryScenes()
        {
            var mandatoryScenes = MandatorySceneDatabase.GetMandatoryScenes();
            var userScenes = MandatorySceneDatabase.GetUserScenes();
            
            var allScenes = mandatoryScenes.Concat(userScenes)
                .OrderBy(s => s.FractionalIndex)
                .ToArray();

            // return if collections are equal
            var buildScenes = allScenes.Select(s => new EditorBuildSettingsScene(s.ScenePath, s.Enabled)).ToArray();
            if (buildScenes.SequenceEqual(EditorBuildSettings.scenes))
            {
                return;
            }

            EditorBuildSettings.scenes = buildScenes;
            Debug.Log("Re-injected mandatory scenes");
            foreach (var scene in EditorBuildSettings.scenes)
            {
                Debug.Log($"-- {scene.path}");
            }
        }
    }
}