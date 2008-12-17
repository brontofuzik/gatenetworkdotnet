using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Plugs;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// An abstract gate type.
    /// </summary>
    public abstract class GateType
       : IGateType
    {
        #region Private instance fields

        /// <summary>
        /// The name of the gate type.
        /// </summary>
        private string name;

        /// <summary>
        /// The names of the input plugs.
        /// </summary>
        private string[] inputPlugNames;

        /// <summary>
        /// The names of the output plugs.
        /// </summary>
        private string[] outputPlugNames;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the name of the gate type.
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
        /// 
        /// </summary>
        public string[] InputPlugNames
        {
            get
            {
                return inputPlugNames;
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
                return inputPlugNames.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] OutputPlugNames
        {
            get
            {
                return outputPlugNames;
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
                return outputPlugNames.Length;
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="GateNetworkDorNet.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputPlugNames</c> contains an illegal name.
        /// Condition 3: <c>outputPlugNames</c> contains an illegal name.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Consition 1: <c>inputPlugNames</c> contains less than zero plug name. 
        /// Condition 2: <c>outputPlugNames</c> contains less than one plug name. 
        /// </exception>
        protected GateType( string name, string inputPlugNames, string outputPlugNames )
        {
            //
            // Validate the name.
            //
            if (!Program.IsLegalIdentifier( name ))
            {
                throw new IllegalIdentifierException( name );
            }
            this.name = name;

            //
            // Validate the names of the input plugs.
            //
            this.inputPlugNames = (inputPlugNames.Length > 0) ? inputPlugNames.Split( ' ' ) : new string[ 0 ];
            if (InputPlugCount < 0)
            {
                throw new ArgumentException( "inputPlugNames" );
            }
            foreach (string inputPlugName in this.inputPlugNames)
            {
                if (!Program.IsLegalIdentifier( inputPlugName ))
                {
                    throw new IllegalIdentifierException( inputPlugName );
                }
            }

            //
            // Validate the names of the output plugs.
            //
            this.outputPlugNames = (outputPlugNames.Length > 0) ? outputPlugNames.Split( ' ' ) : new string[ 0 ];
            if (OutputPlugCount < 1)
            {
                throw new ArgumentException( "outputPlugNames" );
            }
            foreach (string outputPlugName in this.outputPlugNames)
            {
                if (!Program.IsLegalIdentifier( outputPlugName ))
                {
                    throw new IllegalIdentifierException( outputPlugName );
                }
            }
        }

        #endregion // Protected instance constructors

        #region Public instance mehods

        /// <summary>
        /// Gets the index of an input plug specified by its name.
        /// </summary>
        /// 
        /// <param name="name">The name of the input plug.</param>
        /// 
        /// <returns>
        /// The index of the input plug.
        /// </returns>
        public int GetInputPlugIndex( string name )
        {
            for (int i = 0; i < InputPlugCount; i++)
            {
                if (inputPlugNames[ i ].Equals( name ))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of an output plug specified by its name.
        /// </summary>
        /// 
        /// <param name="name">The name of the output plug.</param>
        /// 
        /// <returns>
        /// The index of the output plug.
        /// </returns>
        public int GetOutputPlugIndex( string name )
        {
            for (int i = 0; i < OutputPlugCount; i++)
            {
                if (OutputPlugNames[ i ].Equals( name ))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Instantiates the (abstract) gate object.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) gate object.</param>
        /// 
        /// <returns>
        /// The (abstract) gate object.
        /// </returns>
        public abstract Gate Instantiate( string name );

        /// <summary>
        /// Evaluates the transition function of the abstract gate type.
        /// </summary>
        /// <param name="inputPlugs">The input plugs.</param>
        /// <param name="outputPlugs">The output plugs.</param>
        public abstract void Evaluate( Plug[] inputPlugs, Plug[] outputPlugs );

        #endregion // Public instance methods
    }
}
