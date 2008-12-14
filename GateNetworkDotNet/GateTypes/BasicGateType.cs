using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A basic gate type.
    /// </summary>
    public class BasicGateType
        : GateType
    {
        #region Private instance fields

        /// <summary>
        /// The transition function.
        /// </summary>
        private TransitionFunction transitionFunction;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the length of a transition.
        /// </summary>
        /// 
        /// <value>
        /// The length of a transiiton.
        /// </value>
        public int TransitionLength
        {
            get
            {
                return InputPlugCount + OutputPlugCount;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new basic gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputs">The list of the inputs' names.</param>
        /// <param name="outputs">The list of the outputs' names.</param>
        /// <param name="transitions">The transitions.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputs</c> contains an illegal name.
        /// Condition 3: <c>outputts</c> contains an illegal name.
        /// </exception>
        /// <exception cref="Network.Exceptions.IllegalTransitionException">
        /// Consition: the number of inputs and outputs is not equal to the length of the transition.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>inputs</c> is <c>null</c>.
        /// Condition 2: <c>outputs</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>outputs</c> is empty.
        /// </exception>
        public BasicGateType( string name, List< string > inputs, List< string > outputs, List< string > transitions )
            : base( name, inputs, outputs )
        {
            Dictionary< string, string > dictionary = new Dictionary< string, string >();

            // Validate the transitions.
            foreach (string transition in transitions)
            {
                // Split the transition line.
                string[] inputsAndOutputs = transition.Split( ' ' );

                if (inputsAndOutputs.Length != TransitionLength)
                {
                    throw new IllegalTransitionException( transition );
                }

                // Build the inputs string.
                StringBuilder inputsSB = new StringBuilder();
                for (int i = 0; i < InputPlugCount; i++)
                {
                    inputsSB.Append( inputsAndOutputs[ i ] );
                }
                string inputsStr = inputsSB.ToString();

                // Build the outputs string.
                StringBuilder outputsSB = new StringBuilder();
                for (int i = InputPlugCount; i < TransitionLength; i++)
                {
                    outputsSB.Append( inputsAndOutputs[ i ] );
                }
                string outputsStr = outputsSB.ToString();

                // Add the (inputs, outputs) key-value-pair into the dictionary.
                dictionary.Add( inputsStr, outputsStr );
            }

            transitionFunction = new TransitionFunction( InputPlugCount, OutputPlugCount, dictionary );
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate type.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The inputs.</param>
        /// 
        /// <returns>
        /// The outputs.
        /// </returns>
        public override string[] Evaluate( string[] inputPlugValues )
        {
            return transitionFunction.Evaluate( inputPlugValues );  
        }
        #endregion // Public instance methods
    }
}
