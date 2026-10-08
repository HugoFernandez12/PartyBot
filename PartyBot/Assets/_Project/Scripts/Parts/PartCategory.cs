namespace PartyBot.Parts
{
    public enum PartCategory
    {
        /// <summary>Núcleo: cada robot necesita exactamente uno.</summary>
        Core,
        /// <summary>Bloques que dan forma y protección.</summary>
        Structure,
        /// <summary>Ruedas, propulsores… lo que mueve el robot.</summary>
        Locomotion,
        /// <summary>Palas, martillos, sierras, cañones.</summary>
        Weapon,
        /// <summary>Todo lo demás (escudos, imanes…).</summary>
        Utility,
    }
}
