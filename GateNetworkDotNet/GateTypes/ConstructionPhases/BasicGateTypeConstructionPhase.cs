namespace GateNetworkDotNet.GateTypes.ConstructionPhases
{
    /// <summary>
    /// The phase of a basic gate construction process.
    /// </summary>
    public enum BasicGateTypeConstructionPhase
    {
        BEGIN,          // Construction beginning.
        NAME,           // Constructing name.
        INPUTS,         // Constructing inputs.
        OUTPUTS,        // Constructing output.
        TRANSITIONS,    // Constructing transitions.
        END             // Construction end.
    }
}
