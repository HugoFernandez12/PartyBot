using PartyBot.Spikes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PartyBot.Editor
{
    /// <summary>Añade la prueba de física (tarea 1.1) a la escena abierta. Se borra junto con Scripts/Spikes.</summary>
    static class PhysicsSpikeMenu
    {
        [MenuItem("PartyBot/Prueba de física/Añadir a la escena actual")]
        static void AddToScene()
        {
            var existing = Object.FindAnyObjectByType<PhysicsSpike>();
            if (existing != null)
            {
                Selection.activeObject = existing.gameObject;
                Debug.Log("[PartyBot] La prueba de física ya está en la escena.");
                return;
            }

            var go = new GameObject("PhysicsSpike");
            var spike = go.AddComponent<PhysicsSpike>();

            var serialized = new SerializedObject(spike);
            serialized.FindProperty("partSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>(SandboxSceneBuilder.SpritePath);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(go, "Añadir prueba de física");
            EditorSceneManager.MarkSceneDirty(go.scene);
            Selection.activeObject = go;
        }
    }
}
