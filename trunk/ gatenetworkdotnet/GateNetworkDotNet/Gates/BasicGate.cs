using System;
using System.Collections.Generic;

using GateNetworkDotNet.Gates.Plugs;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// A basic gate.
    /// </summary>
    class BasicGate
        : Gate
    {
        #region Public instance contructors

        /// <summary>
        /// Creates a new basic gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate.</param>
        /// <param name="type">The type of the basic gate.</param>
        /// 
        /// <exception cref="GateNetworkDotNet.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal basic gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        public BasicGate( string name, BasicGateType type )
            : base( name, type )
        {
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate.
        /// </summary>
        public override void Evaluate()
        {
            // Get the inpuuts from the input plugs.
            string[] inputPlugValues = new string[ InputPlugCount ];

            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValues[ i ] = InputPlugs[ i ].Value;
            }

            // Evaluate the transition function.
            string[] outputPlugValues = Type.Evaluate( inputPlugValues );

            // Set the outputs to the output plugs.
            for (int i = 0; i < OutputPlugCount; i++)
            {
                OutputPlugs[ i ].Value = outputPlugValues[ i ];
            }
        }

        #endregion // Public instance methods
    }
}
