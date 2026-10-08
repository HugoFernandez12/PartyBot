namespace PartyBot.Parts
{
    /// <summary>Qué hace una pieza cuando el jugador la controla. Cada valor corresponde a un PartBehaviour.</summary>
    public enum PartBehaviourKind
    {
        /// <summary>Pieza pasiva (núcleo, bloques).</summary>
        None,
        /// <summary>Tracción tipo tanque con el stick izquierdo / WASD.</summary>
        Wheel,
        /// <summary>Empujón hacia donde apunta con el gatillo derecho / Espacio.</summary>
        Thruster,
    }
}
