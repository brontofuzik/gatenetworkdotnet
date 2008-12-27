using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A network gate type.
    /// </summary>
    public class NetworkGateType
        : AbstractCompositeGateType
    {
        #region Public instance methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputPlugNames"></param>
        public override void SetInputPlugNames(string[] inputPlugNames)
        {
            // Validate the number of the names of the input plugs.
            if (inputPlugNames.Length == 0)
            {
                throw new Exception("Syntax error (Network has to have at least one input).");
            }

            base.SetInputPlugNames( inputPlugNames );
        }

        /// <summary>
        /// 
        /// </summary>
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

        #endregion // Public instance methods
    }
}
