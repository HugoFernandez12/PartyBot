using PartyBot.Parts;
using PartyBot.Robot;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PartyBot.Editor
{
    /// <summary>Crea las piezas iniciales, su catálogo y un robot de prueba en la escena.</summary>
    static class StarterContentMenu
    {
        const string PartsFolder = "Assets/_Project/Data/Parts";
        const string CatalogPath = "Assets/_Project/Data/PartCatalog.asset";
        const string TuningPath = "Assets/_Project/Data/RobotTuning.asset";

        readonly struct PartSpec
        {
            public readonly string File, Id, DisplayName;
            public readonly PartCategory Category;
            public readonly Color Color;
            public readonly float Mass;
            public readonly int Health, Cost;
            public readonly PartBehaviourKind Behaviour;
            public readonly float Power;

            public PartSpec(string file, string id, string displayName, PartCategory category, Color color, float mass, int health, int cost,
                PartBehaviourKind behaviour = PartBehaviourKind.None, float power = 0f)
            {
                File = file; Id = id; DisplayName = displayName; Category = category;
                Color = color; Mass = mass; Health = health; Cost = cost;
                Behaviour = behaviour; Power = power;
            }
        }

        static readonly PartSpec[] StarterParts =
        {
            new("Core", "core", "Núcleo", PartCategory.Core, new Color(1f, 0.8f, 0.2f), 2f, 30, 0),
            new("Block", "block", "Bloque", PartCategory.Structure, new Color(0.55f, 0.6f, 0.7f), 1f, 15, 1),
            new("Wheel", "wheel", "Rueda", PartCategory.Locomotion, new Color(0.15f, 0.15f, 0.17f), 0.8f, 10, 2, PartBehaviourKind.Wheel, 25f),
            new("Thruster", "thruster", "Propulsor", PartCategory.Locomotion, new Color(0.95f, 0.4f, 0.25f), 0.7f, 8, 2, PartBehaviourKind.Thruster, 80f),
        };

        [MenuItem("PartyBot/Crear piezas iniciales")]
        static void CreateStarterPartsMenu()
        {
            var catalog = EnsureStarterContent();
            Selection.activeObject = catalog;
            Debug.Log($"[PartyBot] Piezas iniciales listas en {PartsFolder} y catálogo en {CatalogPath}.");
        }

        /// <summary>Si ya hay un RobotSpawner en la escena, solo le asigna catálogo y ajustes que le falten.</summary>
        [MenuItem("PartyBot/Añadir robot de prueba a la escena")]
        static void AddTestRobot()
        {
            var catalog = EnsureStarterContent();
            var tuning = EnsureTuning();

            var spawner = Object.FindAnyObjectByType<RobotSpawner>();
            if (spawner == null)
            {
                var go = new GameObject("RobotSpawner");
                spawner = go.AddComponent<RobotSpawner>();
                Undo.RegisterCreatedObjectUndo(go, "Añadir robot de prueba");
            }

            var serialized = new SerializedObject(spawner);
            AssignIfEmpty(serialized.FindProperty("catalog"), catalog);
            AssignIfEmpty(serialized.FindProperty("tuning"), tuning);
            serialized.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(spawner.gameObject.scene);
            Selection.activeObject = spawner.gameObject;
        }

        static void AssignIfEmpty(SerializedProperty property, Object value)
        {
            if (property.objectReferenceValue == null)
                property.objectReferenceValue = value;
        }

        static RobotTuning EnsureTuning()
        {
            var tuning = AssetDatabase.LoadAssetAtPath<RobotTuning>(TuningPath);
            if (tuning != null)
                return tuning;

            tuning = ScriptableObject.CreateInstance<RobotTuning>();
            AssetDatabase.CreateAsset(tuning, TuningPath);
            AssetDatabase.SaveAssets();
            return tuning;
        }

        /// <summary>Crea lo que falte sin tocar lo que ya existe (respeta los ajustes hechos a mano).</summary>
        static PartCatalog EnsureStarterContent()
        {
            var sprite = SandboxSceneBuilder.GetOrCreateWhiteSprite();

            var catalog = AssetDatabase.LoadAssetAtPath<PartCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<PartCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            var catalogSerialized = new SerializedObject(catalog);
            var list = catalogSerialized.FindProperty("parts");

            foreach (var spec in StarterParts)
            {
                var part = EnsurePart(spec, sprite);
                if (!Contains(list, part))
                {
                    list.arraySize++;
                    list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = part;
                }
            }

            catalogSerialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            return catalog;
        }

        static PartDefinition EnsurePart(PartSpec spec, Sprite sprite)
        {
            string path = $"{PartsFolder}/{spec.File}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<PartDefinition>(path);
            if (existing != null)
                return existing;

            var part = ScriptableObject.CreateInstance<PartDefinition>();
            AssetDatabase.CreateAsset(part, path);

            var serialized = new SerializedObject(part);
            serialized.FindProperty("id").stringValue = spec.Id;
            serialized.FindProperty("displayName").stringValue = spec.DisplayName;
            serialized.FindProperty("category").enumValueIndex = (int)spec.Category;
            serialized.FindProperty("sprite").objectReferenceValue = sprite;
            serialized.FindProperty("color").colorValue = spec.Color;
            serialized.FindProperty("mass").floatValue = spec.Mass;
            serialized.FindProperty("maxHealth").intValue = spec.Health;
            serialized.FindProperty("cost").intValue = spec.Cost;
            serialized.FindProperty("behaviour").enumValueIndex = (int)spec.Behaviour;
            serialized.FindProperty("power").floatValue = spec.Power;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return part;
        }

        static bool Contains(SerializedProperty list, Object item)
        {
            for (int i = 0; i < list.arraySize; i++)
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == item)
                    return true;
            return false;
        }
    }
}
