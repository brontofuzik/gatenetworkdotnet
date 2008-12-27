using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A type of a network gate.
    /// </summary>
    public class NetworkGateType
        : CompositeGateType
    {
        #region Public instance methods

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>inputPlugNames</c> contains no explicitly defined input plug.
        /// </exception>
        public override void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the names of the input plugs.
            if (inputPlugNames.Length == 0)
            {
                throw new Exception( "Syntax error (Network has to have at least one input)." );
            }

            base.SetInputPlugNames( inputPlugNames );
        }

        /// <summary>
        /// Validates the connections.
        /// </summary>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: An (explicitly defined) input plug of a type of a composite gate remained unconnected.
        /// </exception>
        public override void ValidateConnections()
        {
            base.ValidateConnections();

            foreach (string inputPlugName in InputPlugNames)
            {
                if (inputPlugName.Equals( "0" ) || inputPlugName.Equals( "1" ))
                {
                    continue;
                }
                if (!Connections.ContainsValue( inputPlugName ))
                {
                    throw new Exception( "Binding rule broken" );
                }
            }
        }

        #endregion // Public instance methods
    }
}
