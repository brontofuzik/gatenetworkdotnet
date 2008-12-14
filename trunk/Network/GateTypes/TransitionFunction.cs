using System;
using System.Collections.Generic;

using Network.Exceptions;

namespace Network.GateTypes
{
    /// <summary>
    /// A transition function of a gate.
    /// </summary>
    public class TransitionFunction
    {
        #region Private instance fields

        /// <summary>
        /// The number of inputs.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The number of outputs.
        /// </summary>
        private int outputCount;

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
        /// <param name="inputCount">The number of inputs.</param>
        /// <param name="outputCount">The number of outputs.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Condition: <c>dictionary</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Condition 1: <c>inputCount</c> is less than zero.
        /// Consition 2: <c>outputCount</c> is less than one.
        /// </exception>
        public TransitionFunction( int inputCount, int outputCount, Dictionary< string, string > dictionary )
        {
            // Validate the number of inputs.
            if (inputCount < 0)
            {
                throw new ArgumentException( "inputCount" );
            }
            this.inputCount = inputCount;
            
            // Validate the number of outputs.
            if (inputCount < 1)
            {
                throw new ArgumentException( "outputCount" );
            }
            this.outputCount = outputCount;

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
        public string Evaluate( string inputs )
        {
            string outputs;

            if (dictionary.ContainsKey( inputs ))
            {
                // The transition function contains the mapping for the inputs.
                outputs = dictionary[ inputs ];
            }
            else
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                outputs = (inputs.Contains( "?" )) ?
                    new String( '?', outputCount ) :
                    new String( '0', outputCount );
            }

            return outputs;
        }

        #endregion // Public instance methods
    }
}
