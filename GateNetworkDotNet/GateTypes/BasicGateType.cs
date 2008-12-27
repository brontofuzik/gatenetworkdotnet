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
        : AbstractGateType
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
        /// 
        /// </summary>
        public Dictionary<string, string> Transitions
        {
            get
            {
                return transitions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        public BasicGateType()
        {
            transitions = new Dictionary< string, string >();

            constructionPhase = BasicGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="gateTypes"></param>
        public override void Configure( string line, Dictionary<string, AbstractGateType> gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals("inputs") || keyword.Equals("outputs") || keyword.Equals("end"))
            {
                base.Configure(line, gateTypes);
            }
            else
            {
                // Set the transitions.
                string[] transition = ParseTransition(line);
                AddTransition(transition);
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
        public string[] ParseTransition(string line)
        {
            // Split the line into words.
            string[] words = line.Split(' ');
            
            // Return all the words.
            return words;
        }

        /// <summary>
        /// Sets the name of the basic gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate type.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        public override void SetName( string name )
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
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>inputPlugNames</c> is <c>null</c>.
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
        /// Condition: <c>outputPlugNames</c> is <c>null</c>.
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
        /// Adds a transition to the transitions.
        /// </summary>
        /// 
        /// <param name="transition">The transition.</param>
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
                throw new ArgumentNullException("transition");
            }

            if (transition.Length != TransitionLength)
            {
                throw new Exception( "Syntax error (" + transition + ")." );
            }
            foreach (string transitionValue in transition)
            {
                if (!Plug.IsLegalPlugValue( transitionValue ))
                {
                    throw new Exception("Syntax error (" + transition + ").");
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
        public override void EndConstruction()
        {
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception( "Missing keyword." );
            }
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
        public override AbstractGate Instantiate( string name )
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
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        TRANSITIONS,
        END
    }
}
