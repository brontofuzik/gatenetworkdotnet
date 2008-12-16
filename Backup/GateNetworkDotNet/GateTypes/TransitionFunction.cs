using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A transition function of a gate.
    /// </summary>
    public class TransitionFunction
    {
        #region Private instance fields

        /// <summary>
        /// The number of the input plugs.
        /// </summary>
        private int inputPlugCount;

        /// <summary>
        /// The number of the output plugs.
        /// </summary>
        private int outputPlugCount;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary< string, string > dictionary;

        #endregion // Private instance fields

        #region Public instance constructors

        /// <summary>
        /// Creates a new transition function.
        /// </summary>
        /// 
        /// <param name="inputPlugCount">The number of the input plugs.</param>
        /// <param name="outputPlugCount">The number of the output plugs.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>dictionary</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition 1: <c>inputPlugCount</c> is less than zero.
        /// Consition 2: <c>outputPlugCopunt</c> is less than one.
        /// </exception>
        public TransitionFunction( int inputPlugCount, int outputPlugCount, Dictionary< string, string > dictionary )
        {
            // Validate the number of the input plugs.
            if (inputPlugCount < 0)
            {
                throw new ArgumentException( "inputPlugCount" );
            }
            this.inputPlugCount = inputPlugCount;
            
            // Validate the number of the output plugs.
            if (outputPlugCount < 1)
            {
                throw new ArgumentException( "outputPlugCount" );
            }
            this.outputPlugCount = outputPlugCount;

            // Validate the dictionary.
            if (dictionary == null)
            {
                throw new ArgumentNullException( "dictionary" );
            }
            this.dictionary = dictionary;
        }

        #endregion // Public instance construtors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function for the given inputs.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs.</param>
        /// 
        /// <returns>
        /// The outputs.
        /// </returns>
        public string[] Evaluate( string[] inputPlugValues )
        {
            // Build the inputs.
            StringBuilder inputsSB = new StringBuilder();
            for (int i = 0; i < inputPlugCount; i++)
            {
                inputsSB.Append( inputPlugValues[ i ] );
            }
            string inputs = inputsSB.ToString();

            // Get the outputs.
            // TODO: Think about replacing the fllowing piece of code with TryGetMethod.
            string outputs;
            try
            {
                // The transition function contains the mapping for the inputs.
                outputs = dictionary[ inputs ];    
            }
            catch (KeyNotFoundException e)
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                outputs = (inputs.Contains( "?" )) ?
                    new String( '?', outputPlugCount ) :
                    new String( '0', outputPlugCount );
            }

            // De-build the outputs.
            string[] outputPlugValues = new string[ outputPlugCount ];
            for (int i = 0; i < outputPlugCount; i++)
            {
                outputPlugValues[ i ] = outputs.Substring( i, 1 );
            }

            return outputPlugValues;
        }

        #endregion // Public instance methods
    }
}
