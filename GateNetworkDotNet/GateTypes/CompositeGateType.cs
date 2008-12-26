using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A composite gate type.
    /// </summary>
    public class CompositeGateType
        : GateType
    {
        #region Private static fields

        /// <summary>
        /// The names of the implicit input plugs.
        /// </summary>
        private static string implicitInputPlugNames = "0 1";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The gates comprising the composite gate type.
        /// </summary>
        private Dictionary< string, GateType > nestedGateTypes;

        /// <summary>
        /// The connections within the composite gate type.
        /// </summary>
        private Dictionary< string, string > connections;

        /// <summary>
        /// The phase of construction of the composite gate.
        /// </summary>
        private CompositeGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public Dictionary< string, string > Connections
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
        /// Determines whether the composite gate is constructed or not.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the composite gate is constructed, <c>false</c> otherwise.
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
        /// Creates a new composite gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate type.</param>
        ///
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>name</c> is not a legal identifier.
        /// </exception>
        public CompositeGateType()
        {
            nestedGateTypes = new Dictionary< string, GateType >();
            connections = new Dictionary< string, string >();

            constructionPhase = CompositeGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="gateTypes"></param>
        public override void Configure(string line, Dictionary<string, GateType> gateTypes)
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ))
            {
                // Set the names of the input plugs.
                string inputPlugNames = ParsePlugNames( line );
                SetInputPlugNames( inputPlugNames );
            }
            else if (keyword.Equals( "outputs" ))
            {
                // Set the names of the output plugs.
                string outputPlugNames = ParsePlugNames( line );
                SetOutputPlugNames( outputPlugNames );
            }
            else if (keyword.Equals( "gate" ))
            {
                // Set the nested gates.
                string nestedGate = ParseNestedGate( line );
                AddNestedGate( nestedGate, gateTypes );
            }
            else if (keyword.Equals( "end" ))
            {
                // End the construction process.
                EndConstruction();
            }
            else
            {
                // Set the connections.
                string connection = ParseConnection(line);
                AddConnection(connection);
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
        public string ParseNestedGate(string line)
        {
            return line.Substring(line.IndexOf(' ') + 1);
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
        public string ParseConnection(string line)
        {
            return line;
        }

        /// <summary>
        /// Sets the name of the composite gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate type.</param>
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
            if (constructionPhase != CompositeGateTypeConstructionPhase.NAME)
            {
                throw new MyException( "Missing keyword." );
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
            if (constructionPhase != CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new MyException( "Missing keyword." );
            }

            base.SetInputPlugNames( inputPlugNames + " " + implicitInputPlugNames );

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
            if (constructionPhase != CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new MyException( "Missing keyword." );
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
        /// Condition: <c>nestedGate</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>nestedGate</c> is not a legally formed nested gate.
        /// Condition 3: <c>nestedGate</c> contains an illegal name of a nested gate.
        /// Condition 4: <c>nestedGate</c> contains a duplicit name of a nested gate.
        /// Condition 5: <c>netsedGate</c> contains an unknown type of a nested gate.
        /// </exception>
        public void AddNestedGate( string nestedGate, Dictionary< string, GateType > gateTypes )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NESTED_GATES && constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new MyException( "Missing keyword." );
            }

            // Validate the nested gate string.
            if (nestedGate == null)
            {
                throw new ArgumentNullException( "nestedGate" );
            }
            // Validate the types of gates.
            if (gateTypes == null)
            {
                throw new ArgumentNullException( "gateTypes" );
            }

            string[] nestedGateNameAndType = nestedGate.Split( ' ' );
            if (nestedGateNameAndType.Length != 2)
            {
                throw new MyException( "Syntax error (" + nestedGate + ")." );
            }
            
            //
            // Validate the name of the nested gate.
            //
            string nestedGateName = nestedGateNameAndType[ 0 ];
            
            // Validate the legality of the name of the nested gate.
            if (!Program.IsLegalName( nestedGateName ))
            {
                throw new MyException( "Syntax error (" + nestedGateName + ")." );
            }
            // Validate the uniqueness of the name of the nested gate.
            if (nestedGateTypes.ContainsKey( nestedGateName ))
            {
                throw new MyException( "Duplicate (" + nestedGateName + ")." );
            }

            //
            // Validate the type of the nested gate.
            //
            string nestedGateTypeString = nestedGateNameAndType[ 1 ];

            // Retrieve the type of the nested gate.
            GateType nestedGateType;
            try
            {
                nestedGateType = gateTypes[ nestedGateTypeString ];
            }
            catch (KeyNotFoundException)
            {
                throw new MyException( "Syntax error (" + nestedGateTypeString + ")." );
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
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: TODO
        /// Condition 2: <c>connection</c> is not a legally formed connection.
        /// </exception>
        public void AddConnection( string connection )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new MyException( "Missing keyword." );
            }

            string[] connectionToAndFrom = Regex.Split( connection, "->" );
            if (connectionToAndFrom.Length != 2)
            {
                throw new MyException( "Syntax error (" + connection + ")." );
            }

            // TODO:
            // Validate the endpoint of the connection.
            //
            string connectionTo = connectionToAndFrom[ 0 ];

            if (connections.ContainsKey(connectionTo))
            {
                throw new MyException( "Duplicate (" + connection + ")." );
            }

            // TODO:
            // Validate the startpoint of the connection.
            //
            string connectionFrom = connectionToAndFrom[ 1 ];

            // Store the connection.
            connections.Add( connectionTo, connectionFrom );
        }

        /// <summary>
        /// Ends the construction process.
        /// </summary>
        public override void EndConstruction()
        {
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new MyException( "Missing keyword." );
            }
            constructionPhase = CompositeGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the composite gate object.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate object.</param>
        /// 
        /// <returns>
        /// The composite gate object.
        /// </returns>
        public override Gate Instantiate( string name )
        {
            return new CompositeGate( name, this );
        }

        #endregion // Public instance methods
    }

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
