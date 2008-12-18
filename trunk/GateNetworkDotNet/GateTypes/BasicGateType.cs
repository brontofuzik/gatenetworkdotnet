using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Plugs;

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
        /// The transitions.
        /// </summary>
        private Dictionary< string, string > transitions;

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
        /// <param name="name">The name of the basic gate type.</param>
        ///
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>name</c> is not a legal identifier.
        /// </exception>
        public BasicGateType( string name )
            : base( name )
        {
            transitions = new Dictionary< string, string >();
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Adds a transition to the transitions
        /// </summary>
        /// 
        /// <param name="transition">The transition.</param>
        public void AddTransition( string transition )
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
            string inputs = inputsSB.ToString();

            // Build the outputs string.
            StringBuilder outputsSB = new StringBuilder();
            for (int i = InputPlugCount; i < TransitionLength; i++)
            {
                outputsSB.Append( inputsAndOutputs[ i ] );
            }
            string outputs = outputsSB.ToString();

            // Add the (inputs, outputs) key-value-pair into the dictionary.
            transitions.Add( inputs, outputs );
        }

        /// <summary>
        /// Instantiates the basic gate object.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate object.</param>
        /// 
        /// <returns>
        /// The basic gate object.
        /// </returns>
        public override Gate Instantiate( string name )
        {
            return new BasicGate( name, this );
        }

        /// <summary>
        /// Evaluates the transition function of the abstract gate type.
        /// </summary>
        /// <param name="inputPlugs">The input plugs.</param>
        /// <param name="outputPlugs">The output plugs.</param>
        public void Evaluate( Plug[] inputPlugs, Plug[] outputPlugs )
        {
            // Get the values of the input plugs.
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append( inputPlugs[ i ].Value );
            }
            string inputPlugValues = inputPlugValuesSB.ToString();

            // Get the outputs.
            // TODO: Think about replacing the following piece of code with TryGetMethod.
            string outputPlugValues;
            try
            {
                // The transition function contains the mapping for the inputs.
                outputPlugValues = transitions[ inputPlugValues ];
            }
            catch (KeyNotFoundException e)
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                outputPlugValues = (inputPlugValues.Contains( "?" )) ?
                    new String( '?', OutputPlugCount ) :
                    new String( '0', OutputPlugCount );
            }

            // Set the values of the output plugs.
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugs[ i ].Value = outputPlugValues.Substring( i, 1 );
            }
        }
        #endregion // Public instance methods
    }
}
