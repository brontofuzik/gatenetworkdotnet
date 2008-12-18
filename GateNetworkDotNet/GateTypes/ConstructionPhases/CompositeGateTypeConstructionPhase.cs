namespace GateNetworkDotNet.GateTypes.ConstructionPhases
{
    /// <summary>
    /// The phase of a composite gate type construction process.
    /// </summary>
    public enum CompositeGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUTS,
        OUTPUTS,
        GATES,
        CONNECTIONS,
        END
    }
}
