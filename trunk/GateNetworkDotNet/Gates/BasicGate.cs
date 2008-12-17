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
            //
            // Initialize the output plugs.
            //
            if (InputPlugCount == 0)
            {
                // Use the transition function to initialize the output plugs.
                Evaluate();
            }
            else
            {
                // Use the undefined value ("?") to initilize the output plugs.
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    OutputPlugs[ i ].Value = "?";
                }
            }
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition fucntion of the gate.
        /// </summary>
        public override void Evaluate()
        {
            Type.Evaluate( InputPlugs, OutputPlugs );
        }

        #endregion // Public instance methods
    }
}
