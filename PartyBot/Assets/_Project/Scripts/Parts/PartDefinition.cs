using System.Collections.Generic;
using System.Linq;
using PartyBot.Core;
using UnityEngine;

namespace PartyBot.Parts
{
    /// <summary>
    /// Ficha de una pieza: qué es, cómo se ve, qué celdas ocupa y sus stats.
    /// Se crea con clic derecho > Create > PartyBot > Pieza.
    /// </summary>
    [CreateAssetMenu(fileName = "NuevaPieza", menuName = "PartyBot/Pieza", order = 0)]
    public class PartDefinition : ScriptableObject
    {
        [SerializeField, Tooltip("Identificador que se guarda en el JSON de los robots. Vacío = nombre del asset. No cambiarlo una vez haya robots guardados.")]
        string id;
        [SerializeField, Tooltip("Nombre que se muestra en el juego. Vacío = nombre del asset.")]
        string displayName;
        [SerializeField] PartCategory category = PartCategory.Structure;

        [Header("Aspecto")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color color = Color.white;

        [Header("Forma")]
        [SerializeField, Tooltip("Celdas que ocupa sin rotar, relativas a su celda de origen (0,0).")]
        Vector2Int[] shape = { Vector2Int.zero };

        [Header("Stats")]
        [SerializeField, Min(0.01f)] float mass = 1f;
        [SerializeField, Min(1)] int maxHealth = 10;
        [SerializeField, Min(0), Tooltip("Lo que cuesta del presupuesto de montaje.")] int cost = 1;

        [Header("Comportamiento")]
        [SerializeField] PartBehaviourKind behaviour = PartBehaviourKind.None;
        [SerializeField, Min(0f), Tooltip("Fuerza que aplica la pieza (ruedas, propulsores…).")] float power = 10f;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public PartCategory Category => category;
        public bool IsCore => category == PartCategory.Core;
        public Sprite Sprite => sprite;
        public Color Color => color;
        public IReadOnlyList<Vector2Int> Shape => shape;
        public float Mass => mass;
        public int MaxHealth => maxHealth;
        public int Cost => cost;
        public PartBehaviourKind Behaviour => behaviour;
        public float Power => power;

        /// <summary>Celdas de la rejilla que ocupa la pieza colocada en <paramref name="origin"/> con esa rotación.</summary>
        public IEnumerable<Vector2Int> GetCells(Vector2Int origin, int rotation)
        {
            foreach (var cell in shape)
                yield return origin + GridMath.Rotate(cell, rotation);
        }

        void OnValidate()
        {
            if (shape == null || shape.Length == 0)
                shape = new[] { Vector2Int.zero };
            else if (!shape.Contains(Vector2Int.zero))
                Debug.LogWarning($"[PartDefinition] '{name}': la forma debería incluir la celda (0,0), que es el punto de agarre y giro.", this);
        }

        internal static PartDefinition Create(string id, PartCategory category, Vector2Int[] shape = null, int cost = 1, float mass = 1f,
            PartBehaviourKind behaviour = PartBehaviourKind.None, float power = 10f)
        {
            var part = CreateInstance<PartDefinition>();
            part.name = id;
            part.id = id;
            part.category = category;
            part.shape = shape ?? new[] { Vector2Int.zero };
            part.cost = cost;
            part.mass = mass;
            part.behaviour = behaviour;
            part.power = power;
            return part;
        }
    }
}
