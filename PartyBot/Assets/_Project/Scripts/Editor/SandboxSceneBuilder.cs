using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PartyBot.Editor
{
    /// <summary>
    /// Genera la escena Sandbox: arena cerrada con paredes y cámara cenital.
    /// Menú: PartyBot > Crear escena Sandbox. Se puede regenerar cuando cambien las medidas.
    /// </summary>
    public static class SandboxSceneBuilder
    {
        const string ScenePath = "Assets/_Project/Scenes/Sandbox.unity";
        internal const string SpritePath = "Assets/_Project/Art/WhiteSquare.png";
        const string Physics2DSettingsPath = "ProjectSettings/Physics2DSettings.asset";

        // Medidas en unidades de Unity (1 unidad = 1 celda de robot).
        const float ArenaWidth = 30f;
        const float ArenaHeight = 18f;
        const float WallThickness = 1f;
        const float CameraSize = 10.5f;

        static readonly Color BackgroundColor = new Color(0.10f, 0.10f, 0.12f);
        static readonly Color FloorColor = new Color(0.22f, 0.24f, 0.28f);
        static readonly Color WallColor = new Color(0.55f, 0.58f, 0.64f);

        [MenuItem("PartyBot/Crear escena Sandbox")]
        public static void Build()
        {
            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Sandbox", "La escena Sandbox ya existe. ¿Sobrescribirla?", "Sí", "Cancelar"))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            DisableGravity();
            var sprite = GetOrCreateWhiteSprite();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            CreateGlobalLight();
            CreateArena(sprite);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings();
            Debug.Log($"[PartyBot] Escena Sandbox creada en {ScenePath}");
        }

        // Juego cenital: sin gravedad en todo el proyecto.
        static void DisableGravity()
        {
            Physics2D.gravity = Vector2.zero;

            var settings = AssetDatabase.LoadAllAssetsAtPath(Physics2DSettingsPath);
            if (settings.Length == 0)
                return;

            var serialized = new SerializedObject(settings[0]);
            var gravity = serialized.FindProperty("m_Gravity");
            if (gravity == null)
                return;

            gravity.vector2Value = Vector2.zero;
            serialized.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        // Sprite blanco de 1x1 unidad que se tiñe con SpriteRenderer.color.
        internal static Sprite GetOrCreateWhiteSprite()
        {
            if (!File.Exists(SpritePath))
            {
                const int size = 4;
                var texture = new Texture2D(size, size);
                var pixels = new Color[size * size];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = Color.white;
                texture.SetPixels(pixels);
                File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);

                AssetDatabase.ImportAsset(SpritePath);
                var importer = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = size;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        }

        static void CreateCamera()
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 0f, -10f);

            var camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = CameraSize;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BackgroundColor;

            go.AddComponent<UniversalAdditionalCameraData>();
        }

        static void CreateGlobalLight()
        {
            var go = new GameObject("Global Light 2D");
            var light = go.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 1f;
        }

        static void CreateArena(Sprite sprite)
        {
            var arena = new GameObject("Arena").transform;

            var floor = CreateBlock("Floor", sprite, FloorColor, Vector2.zero, new Vector2(ArenaWidth, ArenaHeight), arena);
            floor.GetComponent<SpriteRenderer>().sortingOrder = -10;

            float halfW = ArenaWidth / 2f + WallThickness / 2f;
            float halfH = ArenaHeight / 2f + WallThickness / 2f;
            var horizontal = new Vector2(ArenaWidth + WallThickness * 2f, WallThickness);
            var vertical = new Vector2(WallThickness, ArenaHeight);

            AddCollider(CreateBlock("Wall_Top", sprite, WallColor, new Vector2(0f, halfH), horizontal, arena));
            AddCollider(CreateBlock("Wall_Bottom", sprite, WallColor, new Vector2(0f, -halfH), horizontal, arena));
            AddCollider(CreateBlock("Wall_Left", sprite, WallColor, new Vector2(-halfW, 0f), vertical, arena));
            AddCollider(CreateBlock("Wall_Right", sprite, WallColor, new Vector2(halfW, 0f), vertical, arena));
        }

        static GameObject CreateBlock(string name, Sprite sprite, Color color, Vector2 position, Vector2 size, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            return go;
        }

        // Las paredes son estáticas: collider sin Rigidbody2D.
        static void AddCollider(GameObject go) => go.AddComponent<BoxCollider2D>();

        static void AddToBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var s in scenes)
                if (s.path == ScenePath)
                    return;

            var list = new List<EditorBuildSettingsScene>(scenes)
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = list.ToArray();
        }
    }
}
