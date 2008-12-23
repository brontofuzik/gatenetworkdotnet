using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// An abstract gate.
    /// </summary>
    public abstract class Gate
    {
        #region Private instance fields

        /// <summary>
        /// The name of the (abstract) gate.
        /// </summary>
        private string name;

        /// <summary>
        /// The type of the (abstract) gate.
        /// </summary>
        private GateType type;

        /// <summary>
        /// The input plugs.
        /// </summary>
        private Plug[] inputPlugs;

        /// <summary>
        /// The output plugs.
        /// </summary>
        private Plug[] outputPlugs;

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
        /// Gets the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The input plugs.
        /// </value>
        public Plug[] InputPlugs
        {
            get
            {
                return inputPlugs;
            }
        }

        /// <summary>
        /// Gets the number of the input plugs.
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

        /// <summary>
        /// Gets the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The output plugs.
        /// </value>
        public Plug[] OutputPlugs
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
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>name</c> is not a legal identifier.
        /// </exception>
        protected Gate( string name, GateType type )
        {
            // Validate the name.
            if (!Program.IsLegalIdentifier( name ))
            {
                throw new ArgumentException( name );
            }
            this.name = name;

            // Validate the (abstract) gate type.
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            this.type = type;

            // Create the input plugs.
            inputPlugs = new Plug[ InputPlugCount ];
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugs[ i ] = new Plug( this );
            }

            // Create the output plugs.
            outputPlugs = new Plug[ OutputPlugCount ];
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugs[ i ] = new Plug( this );
            }
        }

        #endregion // Protected instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        public abstract void SetInputPlugValues( string inputPlugValues );

        /// <summary>
        /// Gets the values of the output plugs.
        /// </summary>
        /// 
        /// <returns>
        /// The values of the output plugs.
        /// </returns>
        public string GetOutputPlugValues()
        {
            StringBuilder outputPlugValues = new StringBuilder();
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugValues.Append( outputPlugs[ i ].Value + " " );
            }
            return outputPlugValues.ToString();
        }

        /// <summary>
        /// Gets an input plug specified by its name.
        /// </summary>
        /// 
        /// <param name="inputPlugName">The name of the input plug.</param>
        /// 
        /// <returns>
        /// The input plug.
        /// </returns>
        public Plug GetInputPlugByName( string inputPlugName )
        {
            int inputPlugIndex = type.GetInputPlugIndex( inputPlugName );
            return (inputPlugIndex != -1) ? InputPlugs[ inputPlugIndex ] : null;
        }

        /// <summary>
        /// Gets an output plug specified by its name.
        /// </summary>
        /// 
        /// <param name="outputPlugName">The name of the output plug.</param>
        /// 
        /// <returns>
        /// The output plug.
        /// </returns>
        public Plug GetOutputPlugByName( string outputPlugName )
        {
            int outputPlugIndex = type.GetOutputPlugIndex( outputPlugName );
            return (outputPlugIndex != -1) ? OutputPlugs[ outputPlugIndex ] : null;
        }

        /// <summary>
        /// Initializes the (abstract) gate.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Evaluates the (abstract) gate.
        /// </summary>
        public abstract void Evaluate();

        #endregion // Public instance methods
    }
}
