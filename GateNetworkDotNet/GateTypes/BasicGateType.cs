using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// The type of a basic gate.
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
        /// The phase of construction.
        /// </summary>
        private BasicGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the transitions.
        /// </summary>
        /// 
        /// <value>
        /// The transitions.
        /// </value>
        public Dictionary< string, string > Transitions
        {
            get
            {
                return transitions;
            }
        }

        /// <summary>
        /// Gets the number of transitions.
        /// </summary>
        /// 
        /// <value>
        /// The number of transitions.
        /// </value>
        public int TransitionCount
        {
            get
            {
                return transitions.Count;
            }
        }

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
        /// Determines whether the type of a basic gate is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the type of a basic gate is constructed, <c>false</c> otherwise.
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
        /// Creates a new type of a basic gate.
        /// </summary>
        public BasicGateType()
        {
            transitions = new Dictionary< string, string >();

            constructionPhase = BasicGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Configures the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="line">The line (from the configuration file).</param>
        /// <param name="gateTypes">The (already defined) types of gates.</param>
        public override void Configure( string line, Dictionary< string, GateType > gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ) || keyword.Equals( "outputs" ) || keyword.Equals( "end" ))
            {
                base.Configure( line, gateTypes );
            }
            else
            {
                // Set the transitions.
                string[] transition = ParseTransition( line );
                AddTransition( transition );
            }
        }

        /// <summary>
        /// Parses a line for a transition.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The transition.
        /// </returns>
        public string[] ParseTransition( string line)
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );
            
            // Return all the words.
            return words;
        }

        /// <summary>
        /// Sets the name of the basic type of a gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic type of a gate.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        protected override void SetName( string name )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.NAME)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetName( name );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetInputPlugNames( inputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetOutputPlugNames( string[] outputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetOutputPlugNames( outputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.TRANSITIONS;
        }

        /// <summary>
        /// Adds a transition to the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="transition">The transition.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>transition</c> is null.
        /// </exception>
        /// 
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>transition</c> contains a transition of incorrect length.
        /// Condition 3: <c>transition</c> contains an illegal value of an (input or output) plug.
        /// Condition 4: <c>transition</c> contains a transiiton that has already been added to the transitions.
        /// </exception>
        public void AddTransition( string[] transition )
        {
            // Validate the phase of construciton.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the transition.
            if (transition == null)
            {
                throw new ArgumentNullException( "transition" );
            }
            if (transition.Length != TransitionLength)
            {
                throw new Exception( "Syntax error (" + transition + ")." );
            }
            foreach (string transitionValue in transition)
            {
                if (!Plug.IsLegalPlugValue( transitionValue ))
                {
                    throw new Exception( "Syntax error (" + transition + ")." );
                }
            }

            //
            // Build the string containing the values of the input strings.
            //
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append( transition[ i ] + " " );
            }
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove( inputPlugValuesSB.Length - 1, 1 );
            }
            string inputPlugValues = inputPlugValuesSB.ToString();

            if (transitions.ContainsKey( inputPlugValues ))
            {
                throw new Exception( "Duplicate (" + transition + ")." );
            }

            //
            // Build the string containing the values of the output strings.
            //
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = InputPlugCount; i < TransitionLength; i++)
            {
                outputPlugValuesSB.Append( transition[ i ] + " " );
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
        /// 
        /// <exception cref="Syste.Exception">
        /// Condition: The method has been called in the wrong phase of construction.
        /// </exception>
        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate.</param>
        /// 
        /// <returns>
        /// The basic gate (as an (abstract) gate).
        /// </returns>
        public override Gate Instantiate( string name )
        {
            return new BasicGate( name, this );
        }

        /// <summary>
        /// Evaluates the transiiotn function of the type of a basic gate.
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
                if (outputPlugValuesSB.Length != 0)
                {
                    outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
                }
                outputPlugValues = outputPlugValuesSB.ToString();
            }

            // Return the values of the output plugs.
            return outputPlugValues;
        }
        #endregion // Public instance methods
    }

    /// <summary>
    /// The phase of construction of a type of a basic gate.
    /// </summary>
    enum BasicGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        TRANSITIONS,
        END
    }
}
