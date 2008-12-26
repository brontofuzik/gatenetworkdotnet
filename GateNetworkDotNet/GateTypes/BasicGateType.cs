using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

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

        /// <summary>
        /// The phase of construction of the basic gate.
        /// </summary>
        private BasicGateTypeConstructionPhase constructionPhase;

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

        /// <summary>
        /// Determines whether the basic gate type is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the basic gate type is constructed, <c>false</c> otherwise.
        /// </value>
        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == BasicGateTypeConstructionPhase.END);
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
        public BasicGateType()
        {
            transitions = new Dictionary< string, string >();

            constructionPhase = BasicGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the name of the basic gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate type.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>name</c> is not a legal identifier.
        /// </exception>
        public override void SetName( string name )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.NAME)
            {
                throw new MyException( "Missing keyword." );
            }

            base.SetName( name );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.INPUTS;
        }

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>inputPlugNames</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>inputPlugNames</c> contains an illegal input plug name.
        /// Condition 3: <c>inputPlugNames</c> contains duplicit input plug names.
        /// </exception>
        public override void SetInputPlugNames( string inputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.INPUTS)
            {
                throw new MyException( "Missing keyword." );
            }

            base.SetInputPlugNames( inputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.OUTPUTS;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>outputPlugNames</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>outputPlugNames</c> contains an illegal output plug name.
        /// Condition 3: <c>outputPlugNames</c> contains duplicit output plug names.
        /// Condition 4: <c>outputPlugNames</c> contains less than one output plug name.
        /// </exception>
        public override void SetOutputPlugNames( string outputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.OUTPUTS)
            {
                throw new MyException( "Missing keyword." );
            }

            base.SetOutputPlugNames( outputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.TRANSITIONS;
        }

        /// <summary>
        /// Adds a transition to the transitions.
        /// </summary>
        /// 
        /// <param name="transition">The transition.</param>
        ///
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>transition</c> is not a legal transition.
        /// </exception>
        public void AddTransition( string transition )
        {
            // Validate the phase of construciton.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new MyException( "Missing keyword." );
            }

            // Split the transition.
            string[] inputsAndOutputs = transition.Split( ' ' );

            // Validate the transition.
            if (inputsAndOutputs.Length != TransitionLength)
            {
                throw new MyException( "Syntax error (" + transition + ")." );
            }

            //
            // Build the string containing the values of the input strings.
            //
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append( inputsAndOutputs[ i ] + " " );
            }
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove( inputPlugValuesSB.Length - 1, 1 );
            }
            string inputPlugValues = inputPlugValuesSB.ToString();

            if (transitions.ContainsKey( inputPlugValues ))
            {
                throw new MyException( "Duplicate (" + transition + ")." );
            }

            //
            // Build the string containing the values of the output strings.
            //
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = InputPlugCount; i < TransitionLength; i++)
            {
                outputPlugValuesSB.Append( inputsAndOutputs[ i ] + " " );
            }
            if (outputPlugValuesSB.Length != 0)
            {
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
            }
            string outputPlugValues = outputPlugValuesSB.ToString();

            // Add the (inputPlugValues, outputPlugValues) key-value-pair into the transitions.
            transitions.Add( inputPlugValues, outputPlugValues );
        }

        /// <summary>
        /// Ends the construction process.
        /// </summary>
        public override void EndConstruction()
        {
            constructionPhase = BasicGateTypeConstructionPhase.END;
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
        /// 
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        /// 
        /// <returns>
        /// The values of the output plugs.
        /// </returns>
        public string Evaluate( string inputPlugValues )
        {
            // TODO: Think about replacing the following piece of code with TryGetMethod.
            string outputPlugValues;
            try
            {
                // The transition function contains the mapping for the inputs.
                outputPlugValues = transitions[ inputPlugValues ];
            }
            catch (KeyNotFoundException)
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                string outputPlugValue = inputPlugValues.Contains( "?" ) ? "?" : "0";
                StringBuilder outputPlugValuesSB = new StringBuilder();
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    outputPlugValuesSB.Append( outputPlugValue + " " );
                }
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
                outputPlugValues = outputPlugValuesSB.ToString();
            }

            // Return the values of the output plugs.
            return outputPlugValues;
        }
        #endregion // Public instance methods
    }

    enum BasicGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUTS,
        OUTPUTS,
        TRANSITIONS,
        END
    }
}
