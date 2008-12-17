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
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// <param name="outputs">The names of the output plugs.</param>
        /// <param name="transitions">The transitions.</param>
        /// 
        /// <exception cref="GateNetworkDotNet.Exceptions.IllegalTransitionException">
        /// Consition: the number of inputs and outputs is not equal to the length of the transition.
        /// </exception>
        /// <exception cref="GateNetworkDorNet.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputPlugNames</c> contains an illegal name.
        /// Condition 3: <c>outputPlugNames</c> contains an illegal name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>transitions</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Consition 1: <c>inputPlugNames</c> contains less than zero plug name. 
        /// Condition 2: <c>outputPlugNames</c> contains less than one plug name. 
        /// </exception>
        public BasicGateType( string name, string inputPlugNames, string outputPlugNames, List< string > transitions )
            : base( name, inputPlugNames, outputPlugNames )
        {
            //
            // Validate and construct the transitions.
            //
            if (transitions == null)
            {
                throw new ArgumentNullException( "transitions" );
            }

            this.transitions = new Dictionary< string, string >();
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
                this.transitions.Add( inputsStr, outputsStr );
            }
        }

        #endregion // Public instance constructors

        #region Public instance methods

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
        public override void Evaluate( Plug[] inputPlugs, Plug[] outputPlugs )
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
