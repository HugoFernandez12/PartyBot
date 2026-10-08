using System.Collections.Generic;
using UnityEngine;

namespace PartyBot.Parts
{
    /// <summary>
    /// Lista de todas las piezas del juego. Traduce el id guardado en un robot a su PartDefinition.
    /// Se crea con clic derecho > Create > PartyBot > Catálogo de piezas.
    /// </summary>
    [CreateAssetMenu(fileName = "PartCatalog", menuName = "PartyBot/Catálogo de piezas", order = 1)]
    public class PartCatalog : ScriptableObject
    {
        [SerializeField] List<PartDefinition> parts = new();

        Dictionary<string, PartDefinition> byId;

        public IReadOnlyList<PartDefinition> Parts => parts;

        public bool TryGet(string id, out PartDefinition part)
        {
            EnsureIndex();
            return byId.TryGetValue(id ?? string.Empty, out part);
        }

        void EnsureIndex()
        {
            if (byId != null)
                return;

            byId = new Dictionary<string, PartDefinition>();
            foreach (var part in parts)
            {
                if (part == null)
                    continue;
                if (!byId.TryAdd(part.Id, part))
                    Debug.LogWarning($"[PartCatalog] Id repetido '{part.Id}': se ignora '{part.name}'.", this);
            }
        }

        // Al editar la lista en el Inspector, el índice se reconstruye en la siguiente consulta.
        void OnValidate() => byId = null;

        internal static PartCatalog Create(params PartDefinition[] parts)
        {
            var catalog = CreateInstance<PartCatalog>();
            catalog.parts = new List<PartDefinition>(parts);
            return catalog;
        }
    }
}
