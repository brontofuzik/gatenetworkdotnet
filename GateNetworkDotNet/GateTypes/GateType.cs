using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;

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
        private List< string > inputPlugsNames;

        /// <summary>
        /// The names of the output plugs.
        /// </summary>
        private List< string > outputPlugsNames;

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
                return inputPlugsNames.Count;
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
                return outputPlugsNames.Count;
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputPlugsNames">The names of the input plugs.</param>
        /// <param name="outputPlugsNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="GateNetworkDorNet.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputPlugsNames</c> contains an illegal name.
        /// Condition 3: <c>outputPlugsNames</c> contains an illegal name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>inputPlugsNames</c> is <c>null</c>.
        /// Condition 2: <c>outputPlugsNames</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>outputPlugsNames</c> is empty.
        /// </exception>
        protected GateType( string name, List< string > inputPlugsNames, List< string > outputPlugsNames )
        {
            // Validate the name.
            // The name is valid if it is a legal name.
            if (!IsLegalIdentifier( name ))
            {
                throw new IllegalIdentifierException( name );
            }
            this.name = name;

            // Validate the names of the input plugs.
            if (inputPlugsNames == null)
            {
                throw new ArgumentNullException( "inputPlugsNames" );
            }
            foreach (string inputPlugName in inputPlugsNames)
            {
                if (!IsLegalIdentifier( inputPlugName ))
                {
                    throw new IllegalIdentifierException( inputPlugName );
                }
            }
            this.inputPlugsNames = inputPlugsNames;

            // Validate the names of the output plugs.
            if (outputPlugsNames == null)
            {
                throw new ArgumentNullException( "outputPlugsNames" );
            }
            if (outputPlugsNames.Count < 1)
            {
                throw new ArgumentException( "outputPlugsNames" );
            }
            foreach (string outputPlugName in outputPlugsNames)
            {
                if (!IsLegalIdentifier( outputPlugName ))
                {
                    throw new IllegalIdentifierException( outputPlugName );
                }
            }
            this.outputPlugsNames = outputPlugsNames;
        }

        #endregion // Protected instance constructors

        #region Public static methods

        /// <summary>
        /// Determines whether an identifier is legal.
        /// </summary>
        /// 
        /// <param name="name">The identifier (i.e. name of a gate type, gate instance, gate input or output, network).</param>
        /// 
        /// <returns>
        /// <c>True</c> if the identifier is legal, <c>false</c> otherwise.
        /// </returns>
        public static bool IsLegalIdentifier( string identifier )
        {
            string legalIdentifierPattern = @"^.*$";
            Regex legalIdentifierRegex = new Regex( legalIdentifierPattern );

            return legalIdentifierRegex.IsMatch( identifier );
        }

        #endregion // Public static methods

        #region Public instance mehods

        /// <summary>
        /// Evaluates the transition function of the gate type.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs.</param>
        /// 
        /// <returns>
        /// The outputs.
        /// </returns>
        public abstract string[] Evaluate( string[] inputs );

        #endregion // Public instance methods
    }
}
