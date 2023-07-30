using System;
using UnityEditor;
using UnityEngine;

namespace QuickEye.SceneInjection
{
    [CreateAssetMenu]
    public class MandatorySceneConfig : ScriptableObject
    {
        public SceneAsset scene;

        // show warning if key is already taken
        // between 0 and 1 exclusive, base 10
        public string orderKey;

        public decimal OrderIndex => decimal.Parse($"0.{orderKey}");
        //in editor, show the list of all mandatory scenes

        private void OnValidate()
        {
            MandatorySceneDatabase.SetDirty(this);
        }
    }
}