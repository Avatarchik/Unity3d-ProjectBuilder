namespace Com.ISI.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuildOptions", menuName = "Build System/BuildOptions")]
    public class BuildOptions : ScriptableObject
    {
        [SerializeField]
        private List<BuildSetting> _buildSettings;

        public void SaveLastOptions(BuildTarget bt, UnityEditor.BuildOptions op)
        {
            Target = bt;
            Options = op;
            EditorUtility.SetDirty(this);
        }
        public List<BuildSetting> BuildSettings
        {
            get
            {
                return _buildSettings;
            }

            set
            {
                _buildSettings = value;
            }
        }
        [HideInInspector]

        public BuildTarget Target;
        [HideInInspector]

        public UnityEditor.BuildOptions Options;
    }

    [Serializable]
    public class BuildSetting
    {
        [SerializeField]
        private string _description;

        [SerializeField]
        private List<SceneAsset> scenes;

        [SerializeField]
        private GameObject _possibleProjectContext;

        public List<SceneAsset> Scenes
        {
            get
            {
                return scenes;
            }

            set
            {
                scenes = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public GameObject PossibleProjectContextAndOtherResources
        {
            get
            {
                return _possibleProjectContext;
            }

            set
            {
                _possibleProjectContext = value;
            }
        }
    }


}