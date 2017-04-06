namespace Com.ISI.Editor
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Zenject;

    public class BuildSettingsWindow : ZenjectEditorWindow
    {
        [MenuItem("Window/Build Tool")]
        public static BuildSettingsWindow GetOrCreateWindow()
        {
            var window = EditorWindow.GetWindow<BuildSettingsWindow>();
            window.titleContent = new GUIContent("ProBldr");

            return window;
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ProjectBuilderSelector>().AsSingle().NonLazy();
            //Container.BindInterfacesAndSelfTo<ProjectBuilderTool>().AsSingle().NonLazy();
        }
    }

    internal class ProjectBuilderSelector : IGuiRenderable, ITickable, IInitializable, IDisposable
    {
        private BuildOptions bop;
        private BuildTarget _target;
        private UnityEditor.BuildOptions _options;
        private Vector2 scrollpos;

        public void Initialize()
        {

            var filterOfBuildOp = AssetDatabase.FindAssets("t:ScriptableObject BuildOption");
            bop = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(filterOfBuildOp.First()), typeof(BuildOptions)) as BuildOptions;
            _target = bop.Target;
            _options = bop.Options;
        }

        public void GuiRender()
        {
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollpos))
                {
                    scrollpos = scrollView.scrollPosition;

                    ProjectVersionUpdater();
                    BuildOptionsSelect();
                    BuildSettingsConfigure();
                }
            }
        }

        private void BuildOptionsSelect()
        {
            _target = (BuildTarget)EditorGUILayout.EnumPopup("build target :", _target);
            _options = (UnityEditor.BuildOptions)EditorGUILayout.EnumMaskField("build options :", _options);
            EditorGUILayout.Space();

        }

        public void Tick()
        {
        }

        public void Dispose()
        {
            bop.SaveLastOptions(_target, _options);
        }

        private static void ResetVersion(string[] version, string label, int index)
        {
            if (GUILayout.Button(label))
            {
                int minor = 0;
                version[index] = minor.ToString();
                PlayerSettings.bundleVersion = version[0] + "." + version[1] + "." + version[2];
            }
        }

        private static void IncrementVersion(string[] version, string label, int index)
        {
            if (GUILayout.Button(label))
            {
                int minor = int.Parse(version[index]) + 1;
                version[index] = minor.ToString();
                PlayerSettings.bundleVersion = version[0] + "." + version[1] + "." + version[2];
            }
        }

        private void ProjectVersionUpdater()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Version: " + Application.version + " Product: " + Application.productName);
            string[] version = Application.version.Split('.');
            EditorGUILayout.BeginHorizontal();

            IncrementVersion(version, "minor+", 2);
            IncrementVersion(version, "medium+", 1);
            IncrementVersion(version, "major+", 0);
            GUILayout.Space(20f);
            ResetVersion(version, "reset minor", 2);
            ResetVersion(version, "reset medium", 1);
            ResetVersion(version, "reset major", 0);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20f);
            EditorGUILayout.EndVertical();
        }

        private void BuildSettingsConfigure()
        {
            for (int i = 0; i < bop.BuildSettings.Count; i++)
            {
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("option " + (i + 1) + " : " + bop.BuildSettings[i].Description))
                {
                    ActivateBuildButton(i);
                }
                GUILayout.Space(20f);

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// what the build button does
        /// </summary>
        /// <param name="i">
        /// the index of the buildsetting item to build
        /// </param>
        private void ActivateBuildButton(int i)
        {
            var sceneAssets = bop.BuildSettings[i].Scenes;
            string[] allScenePaths = new string[sceneAssets.Count];

            for (int scene = 0; scene < sceneAssets.Count; scene++)
            {
                allScenePaths[scene] = AssetDatabase.GetAssetPath(sceneAssets[scene]);
            }
            // do temp stuff
            string oldname = PlayerSettings.productName;
            PlayerSettings.productName = bop.BuildSettings[i].Description;
            var prjctx = bop.BuildSettings[i].PossibleProjectContextAndOtherResources;
            string oldPath = null, newPath = null;
            if (prjctx != null)
            {
                oldPath = AssetDatabase.GetAssetPath(prjctx);
                newPath = "Assets/Resources/" + prjctx.name+".prefab";
                Debug.LogError(AssetDatabase.MoveAsset(oldPath, newPath) +"on move assets");
            }
            
            if (_target == 0) Debug.LogError("target not selected");
            
            // initilize build options
            var opt = new BuildPlayerOptions
            {
                scenes = allScenePaths,
                locationPathName = "Builds/"+_target.ToString()+"/" + bop.BuildSettings[i].Description + "/" + bop.BuildSettings[i].Description + ".exe",
                target = _target,
                options = _options,
            };
            // build player
            BuildPipeline.BuildPlayer(opt);
            // undo temp stuff
            if (prjctx != null)
            {
                Debug.Assert(oldPath != null && newPath != null);
                PlayerSettings.productName = oldname;
                AssetDatabase.MoveAsset(newPath, oldPath);
            }
        }
    }
}