using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A type of a composite gate.
    /// </summary>
    public class CompositeGateType
        : GateType
    {
        #region Private static fields

        /// <summary>
        /// The names of the implicit input plugs.
        /// </summary>
        private static string[] implicitInputPlugNames = new string[] {"0","1"};

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The nested gates.
        /// </summary>
        private Dictionary<string, GateType> nestedGateTypes;

        /// <summary>
        /// The connections.
        /// </summary>
        private Dictionary<string, string> connections;

        /// <summary>
        /// The phase of construction.
        /// </summary>
        private CompositeGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The nested gates.
        /// </value>
        public Dictionary< string, GateType > NestedGateTypes
        {
            get
            {
                return nestedGateTypes;
            }
        }

        /// <summary>
        /// Gets the number of the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The number of the nested gates.
        /// </value>
        public int NestedGateCount
        {
            get
            {
                return nestedGateTypes.Count;
            }
        }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// 
        /// <value>
        /// The connections.
        /// </value>
        public Dictionary<string, string> Connections
        {
            get
            {
                return connections;
            }
        }

        /// <summary>
        /// The number of the connections.
        /// </summary>
        /// 
        /// <value>
        /// The number of the connections.
        /// </value>
        public int ConnectionCount
        {
            get
            {
                return connections.Count;
            }
        }

        /// <summary>
        /// Determines whether the type of a composite gate is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the type of a composite gate is constructed, <c>false</c> otherwise.
        /// </value>
        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == CompositeGateTypeConstructionPhase.END);
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new type of a composite gate.
        /// </summary>
        public CompositeGateType()
        {
            nestedGateTypes = new Dictionary< string, GateType >();
            connections = new Dictionary< string, string >();

            constructionPhase = CompositeGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public insatnce methods

        /// <summary>
        /// Configures the type of a composite gate.
        /// </summary>
        /// 
        /// <param name="line">The line (from the configuration file).</param>
        /// <param name="gateTypes">The (already defined) types of gates.</param>
        public override void Configure(string line, Dictionary<string, GateType> gateTypes)
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ) || keyword.Equals( "outputs" ) || keyword.Equals( "end" ))
            {
                base.Configure( line, gateTypes );
            }
            else if (keyword.Equals( "gate" ))
            {
                // Set the nested gates.
                string[] nestedGate = ParseNestedGate( line );
                AddNestedGate( nestedGate, gateTypes );
            }
            else
            {
                // Set the connections.
                string[] connection = ParseConnection( line );
                AddConnection( connection );
            }
        }

        /// <summary>
        /// Parses a line for a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The nested gate.
        /// </returns>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>line</c> contains an illegal definition of a nested gate.
        /// </exception>
        public string[] ParseNestedGate( string line )
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );

            // Validate the number of words (3).
            if (words.Length != 3)
            {
                throw new Exception( "Syntax error." );
            }

            // Return only the second and the third word.
            string[] nestedGate = new string[ 2 ];
            for (int i = 0; i < 2; i++)
            {
                nestedGate[ i ] = words[ i + 1 ];
            }
            return nestedGate;
        }

        /// <summary>
        /// Parses a line for a connection.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The connection.
        /// </returns>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>line</c> contains an illegal definition of a connection.
        /// </exception>
        public string[] ParseConnection( string line )
        {
            // Split the line into words.
            string[] words = Regex.Split( line, "->" );

            // Validate the number of words (2).
            if (words.Length != 2)
            {
                throw new Exception( "Syntax error." );
            }

            // Return all the words.
            return words;
        }

        /// <summary>
        /// Sets the name of the composite type of a gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite type of a gate.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        protected override void SetName(string name)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NAME)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetName( name );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES;
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
            if (constructionPhase != CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            string[] extendedInputPlugNames = new string[ inputPlugNames.Length + implicitInputPlugNames.Length ];
            for (int i = 0; i < inputPlugNames.Length; i++)
            {
                extendedInputPlugNames[ i ] = inputPlugNames[ i ];
            }
            for (int i = 0; i < implicitInputPlugNames.Length; i++)
            {
                extendedInputPlugNames[ inputPlugNames.Length + i ] = implicitInputPlugNames[ i ];
            }
            base.SetInputPlugNames( extendedInputPlugNames );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
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
            if (constructionPhase != CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetOutputPlugNames( outputPlugNames );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.NESTED_GATES;
        }

        /// <summary>
        /// Adds a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The nested gate.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>nestedGate</c> is <c>null</c>.
        /// Condition 2: <c>gateTypes</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>nestedGate</c> contains an illegaly defined nested gate.
        /// Condition 3: <c>nestedGate</c> contains an illegal name of a nested gate.
        /// Condition 4: <c>nestedGate</c> contains a duplicit name of a nested gate.
        /// Condition 5: <c>nestedGate</c> contains an undefined type of a nested gate.
        /// </exception>      
        public void AddNestedGate( string[] nestedGate, Dictionary< string, GateType > gateTypes )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NESTED_GATES && constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the nested gate.
            if (nestedGate == null)
            {
                throw new ArgumentNullException( "nestedGate" );
            }
            if (nestedGate.Length != 2)
            {
                throw new Exception( "Syntax error (" + nestedGate + ")." );
            }

            // Validate the types of gates.
            if (gateTypes == null)
            {
                throw new ArgumentNullException( "gateTypes" );
            }

            //
            // Validate the name of the nested gate.
            //
            string nestedGateName = nestedGate[ 0 ];

            // Validate the legality of the name of the nested gate.
            if (!Program.IsLegalName( nestedGateName ))
            {
                throw new Exception( "Syntax error (" + nestedGateName + ")." );
            }
            // Validate the uniqueness of the name of the nested gate.
            if (nestedGateTypes.ContainsKey(nestedGateName))
            {
                throw new Exception( "Duplicate (" + nestedGateName + ")." );
            }

            //
            // Validate the type of the nested gate.
            //
            string nestedGateTypeString = nestedGate[ 1 ];

            // Retrieve the type of the nested gate.
            GateType nestedGateType;
            try
            {
                nestedGateType = gateTypes[ nestedGateTypeString ];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception( "Syntax error (" + nestedGateTypeString + ")." );
            }

            // Store the nested gate.
            nestedGateTypes.Add( nestedGateName, nestedGateType );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.CONNECTIONS;
        }

        /// <summary>
        /// Adds a connection.
        /// </summary>
        /// 
        /// <param name="connection">The connection.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>connection</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>connection</c> contains an illegally defined connection.
        /// Condition 3: <c>connection</c> contains a duplicit endpoint of a connection.
        /// </exception>
        public void AddConnection( string[] connection )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the connection.
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (connection.Length != 2)
            {
                throw new Exception( "Syntax error (" + connection + ")." );
            }
            
            //
            // Validate the endpoint of the connection.
            //
            string connectionTo = connection[ 0 ];

            if (connections.ContainsKey( connectionTo ))
            {
                throw new Exception( "Duplicate (" + connection + ")." );
            }

            //
            // Validate the startpoint of the connection.
            //
            string connectionFrom = connection[ 1 ];

            // Store the connection.
            connections.Add( connectionTo, connectionFrom );
        }

        /// <summary>
        /// Validates the connections.
        /// </summary>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: An output plug of a type of a composite gate remained unconnected.
        /// </exception>
        public virtual void ValidateConnections()
        {
            foreach (string outputPlugName in OutputPlugNames)
            {
                if (!connections.ContainsKey( outputPlugName ))
                {
                    throw new Exception( "Binding rule broken" );
                }
            }
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
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the connections.
            ValidateConnections();

            // Advance the phase of construciton.
            constructionPhase = CompositeGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the type of a composite gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate.</param>
        /// 
        /// <returns>
        /// The composite gate (as an (abstract) gate).
        /// </returns>
        public override Gate Instantiate( string name)
        {
            return new CompositeGate( name, this );
        }

        #endregion // Public instance methods
    }

    /// <summary>
    /// The phase of construction of a type of a composite gate.
    /// </summary>
    enum CompositeGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        NESTED_GATES,
        CONNECTIONS,
        END
    }
}
