using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Yvsim.EditorUtilities.Samples.Editor
{
    /// <summary>生成可供现有编辑器工具操作的独立素材；不新增 Inspector 或工具窗口。</summary>
    public static class EditorUtilitiesSampleBuilder
    {
        private const string MenuPath = "Yvsim/示例/Editor Utilities/创建并打开工具练习场景";

        [MenuItem(MenuPath)]
        private static void CreateAndOpen()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            // A clean, untitled scene is skipped by SaveCurrentModifiedScenesIfUserWantsTo,
            // but Unity cannot create an additive scene until that scene has a saved path.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && string.IsNullOrEmpty(scene.path) && !EditorSceneManager.SaveScene(scene))
                    return;
            }
            string scenePath = CreateSampleScene();
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            Debug.Log("编辑器工具练习场景已创建：" + scenePath + "。请按 Tool Playground/README.md 使用现有 Yvsim 工具操作示例素材。");
        }

        [MenuItem(MenuPath, true)]
        private static bool CanCreate() => !EditorApplication.isPlayingOrWillChangePlaymode;

        /// <summary>创建唯一素材目录和场景，返回场景路径；生成完毕后恢复原有场景集合。</summary>
        public static string CreateSampleScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play Mode 后再生成示例。");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && string.IsNullOrEmpty(scene.path))
                    throw new InvalidOperationException("当前存在尚未保存命名的场景。请先通过 File > Save 保存，或关闭该场景，再生成示例；CLI 调用方应先准备已保存的场景。尚未创建任何示例文件。");
            }

            Shader shader = GraphicsSettings.currentRenderPipeline != null
                ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
            if (shader == null)
                throw new InvalidOperationException("此素材生成器支持 Built-in Standard 或 URP Lit，请先在当前渲染管线准备可用 Shader。");

            EnsureFolder("Assets/YvsimSamples");
            string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/YvsimSamples/EditorUtilitiesPlayground");
            EnsureFolder(folder);
            Texture2D checker = CreateCheckerTexture(folder + "/Checker.png");
            Material orange = CreateMaterial(shader, new Color(1f, 0.45f, 0.12f), checker, folder + "/Orange.mat");
            CreateMaterial(shader, new Color(0.1f, 0.5f, 1f), checker, folder + "/Blue.mat");

            Scene previousScene = SceneManager.GetActiveScene();
            Scene sampleScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            string scenePath = folder + "/ToolPlayground.unity";
            try
            {
                SceneManager.SetActiveScene(sampleScene);
                var root = new GameObject("Tool Playground");
                GameObject template = GameObject.CreatePrimitive(PrimitiveType.Cube);
                template.name = "Sample Cube";
                template.GetComponent<Renderer>().sharedMaterial = orange;
                template.AddComponent<Rigidbody>().isKinematic = true;
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(template, folder + "/SampleCube.prefab");
                UnityEngine.Object.DestroyImmediate(template);
                if (prefab == null) throw new IOException("无法保存示例 Prefab。");

                var source = (GameObject)PrefabUtility.InstantiatePrefab(prefab, sampleScene);
                source.name = "Source - Copy Components or Inspect Overrides";
                source.transform.SetParent(root.transform, false);
                source.transform.localPosition = new Vector3(-2f, 0f, 0f);
                source.transform.localRotation = Quaternion.Euler(0f, 30f, 0f);
                PrefabUtility.RecordPrefabInstancePropertyModifications(source.transform);
                BoxCollider sourceCollider = source.GetComponent<BoxCollider>();
                sourceCollider.isTrigger = true;
                PrefabUtility.RecordPrefabInstancePropertyModifications(sourceCollider);

                var second = (GameObject)PrefabUtility.InstantiatePrefab(prefab, sampleScene);
                second.name = "Second Prefab - Compare Material and Overrides";
                second.transform.SetParent(root.transform, false);
                second.transform.localPosition = new Vector3(2f, 0f, 0f);
                PrefabUtility.RecordPrefabInstancePropertyModifications(second.transform);

                var target = new GameObject("Paste Target - Initially Empty");
                target.transform.SetParent(root.transform, false);
                target.transform.localPosition = new Vector3(0f, -2f, 0f);

                Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.transform.position = new Vector3(0f, 3f, -9f);
                camera.transform.LookAt(Vector3.zero);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.15f, 0.18f, 0.22f);
                camera.gameObject.AddComponent<AudioListener>();
                Light light = new GameObject("Directional Light").AddComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

                if (!EditorSceneManager.SaveScene(sampleScene, scenePath))
                    throw new IOException("无法保存示例场景：" + scenePath);
                return scenePath;
            }
            finally
            {
                EditorSceneManager.CloseScene(sampleScene, true);
                if (previousScene.IsValid() && previousScene.isLoaded) SceneManager.SetActiveScene(previousScene);
            }
        }

        private static Texture2D CreateCheckerTexture(string path)
        {
            var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            try
            {
                var pixels = new Color32[128 * 128];
                for (int y = 0; y < 128; y++)
                    for (int x = 0; x < 128; x++)
                        pixels[y * 128 + x] = (x / 16 + y / 16) % 2 == 0
                            ? new Color32(255, 255, 255, 255) : new Color32(160, 160, 160, 255);
                texture.SetPixels32(pixels);
                texture.Apply();
                using (var stream = File.Open(Path.Combine(Application.dataPath, path.Substring("Assets/".Length)), FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] png = texture.EncodeToPNG();
                    stream.Write(png, 0, png.Length);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
            AssetDatabase.ImportAsset(path);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Material CreateMaterial(Shader shader, Color color, Texture2D texture, string path)
        {
            var material = new Material(shader);
            material.SetColor(material.HasProperty("_BaseColor") ? "_BaseColor" : "_Color", color);
            material.SetTexture(material.HasProperty("_BaseMap") ? "_BaseMap" : "_MainTex", texture);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
