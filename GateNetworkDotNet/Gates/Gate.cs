using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates.Plugs;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// An abstract gate.
    /// </summary>
    public abstract class Gate
        : IGate
    {
        #region Private instance fields

        /// <summary>
        /// The name of the gate.
        /// </summary>
        private string name;

        /// <summary>
        /// The type of the gate.
        /// </summary>
        private GateType type;

        /// <summary>
        /// The input plugs.
        /// </summary>
        private InputPlug[] inputPlugs;

        /// <summary>
        /// The output plugs.
        /// </summary>
        private OutputPlug[] outputPlugs;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        /// 
        /// <value>
        /// The name of the gate type.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the type of the gate.
        /// </summary>
        /// 
        /// <value>
        /// The type of the gate.
        /// </value>
        public GateType Type
        {
            get
            {
                return type;
            }
        }

        public InputPlug[] InputPlugs
        {
            get
            {
                return inputPlugs;
            }
        }

        /// <summary>
        /// The number of the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the input plugs.
        /// </value>
        public int InputPlugCount
        {
            get
            {
                return type.InputPlugCount;
            }
        }

        public OutputPlug[] OutputPlugs
        {
            get
            {
                return outputPlugs;
            }
        }

        /// <summary>
        /// Gets the number of the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the output plugs.
        /// </value>
        public int OutputPlugCount
        {
            get
            {
                return type.OutputPlugCount;
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate.</param>
        /// <param name="type">The type of the gate.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        protected Gate( string name, GateType type )
        {
            // Validate the name.
            if (!GateType.IsLegalIdentifier( name ))
            {
                throw new IllegalIdentifierException( name );
            }
            this.name = name;

            // Validate the type.
            if (type == null)
            {
                throw new ArgumentNullException( "type" );
            }
            this.type = type;

            // Create the input plugs.
            inputPlugs = new InputPlug[ InputPlugCount ];
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugs[ i ] = new InputPlug();
            }

            // Create the output plugs.
            outputPlugs = new OutputPlug[ OutputPlugCount ];
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugs[ i ] = new OutputPlug();
            }
        }

        #endregion // Protected instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition fucntion of the gate.
        /// </summary>
        public abstract void Evaluate();

        #endregion // Public instance methods
    }
}
