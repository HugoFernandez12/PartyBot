using PartyBot.Core;
using PartyBot.Parts;
using UnityEngine;

namespace PartyBot.Robot
{
    /// <summary>
    /// Convierte un <see cref="RobotData"/> en un robot físico en la escena.
    /// No valida las reglas de montaje: eso es cosa de <see cref="RobotValidator"/> antes de llamar a Build.
    /// </summary>
    public static class RobotAssembler
    {
        // Valores de la prueba de física (tarea 1.1). Se ajustarán en la tarea 1.8.
        const float LinearDamping = 2f;
        const float AngularDamping = 4f;

        /// <summary>Monta el robot con el núcleo en <paramref name="position"/>. Devuelve null si no hay núcleo.</summary>
        public static RobotBody Build(RobotData data, PartCatalog catalog, Vector2 position, Transform parent = null)
        {
            int coreIndex = data.parts.FindIndex(p => p != null && catalog.TryGet(p.partId, out var part) && part.IsCore);
            if (coreIndex < 0)
            {
                Debug.LogError("[RobotAssembler] El robot no tiene núcleo.");
                return null;
            }

            var origin = data.parts[coreIndex].Cell;
            var root = new GameObject(string.IsNullOrWhiteSpace(data.name) ? "Robot" : data.name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = LinearDamping;
            rb.angularDamping = AngularDamping;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var body = root.AddComponent<RobotBody>();
            body.Init(data, rb);

            for (int i = 0; i < data.parts.Count; i++)
            {
                var placed = data.parts[i];
                if (placed == null || !catalog.TryGet(placed.partId, out var definition))
                {
                    Debug.LogWarning($"[RobotAssembler] Pieza desconocida '{placed?.partId}' (#{i}), se ignora.");
                    continue;
                }

                body.AddPart(CreatePart(definition, placed, i, origin, root.transform));
            }

            body.RecalculateMass();
            return body;
        }

        static RobotPart CreatePart(PartDefinition definition, PlacedPart placed, int index, Vector2Int origin, Transform root)
        {
            var go = new GameObject($"{definition.DisplayName} #{index}");
            go.transform.SetParent(root, false);
            go.transform.localPosition = (Vector2)(placed.Cell - origin);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, 90f * GridMath.NormalizeRotation(placed.rotation));

            // Una celda = un hijo. La rotación del padre se encarga de girar la forma.
            foreach (var shapeCell in definition.Shape)
            {
                var cell = new GameObject($"Cell {shapeCell.x},{shapeCell.y}");
                cell.transform.SetParent(go.transform, false);
                cell.transform.localPosition = (Vector2)shapeCell;

                var renderer = cell.AddComponent<SpriteRenderer>();
                renderer.sprite = definition.Sprite;
                renderer.color = definition.Color;

                cell.AddComponent<BoxCollider2D>().size = Vector2.one;
            }

            var part = go.AddComponent<RobotPart>();
            part.Init(definition, index, placed);
            return part;
        }
    }
}
