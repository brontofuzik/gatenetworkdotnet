using System;

namespace LogicCircuit.GateTypes
{
    public class NetworkGateType : CompositeGateType
    {
        public override void SetInputPlugNames(string[] inputPlugNames)
        {
            // Validate the names of the input plugs.
            if (inputPlugNames.Length == 0)
            {
                throw new Exception("Syntax error (Network has to have at least one input).");
            }

            base.SetInputPlugNames(inputPlugNames);
        }

        public override void ValidateConnections()
        {
            base.ValidateConnections();

            foreach (string inputPlugName in InputPlugNames)
            {
                if (inputPlugName.Equals("0") || inputPlugName.Equals("1"))
                {
                    continue;
                }
                if (!Connections.ContainsValue(inputPlugName))
                {
                    throw new Exception("Binding rule broken");
                }
            }
        }
    }
}
