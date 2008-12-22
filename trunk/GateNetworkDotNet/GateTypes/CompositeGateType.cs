using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Connections;
using GateNetworkDotNet.Gates.Plugs;

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
        /// Sets the name of the composite gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate type.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>name</c> is not a legal identifier.
        /// </exception>
        public override void SetName( string name )
        {
            if (constructionPhase != CompositeGateTypeConstructionPhase.NAME)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }

            base.SetName( name );

            constructionPhase = CompositeGateTypeConstructionPhase.INPUTS;
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
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>inputPlugNames</c> contains an illegal input plug name.
        /// </exception>
        public override void SetInputPlugNames( string inputPlugNamesString )
        {
            if (constructionPhase != CompositeGateTypeConstructionPhase.INPUTS)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }

            base.SetInputPlugNames( inputPlugNamesString + " " + implicitInputPlugNames );

            constructionPhase = CompositeGateTypeConstructionPhase.OUTPUTS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputPlugNamesString"></param>
        public override void SetOutputPlugNames(string outputPlugNamesString)
        {
            if (constructionPhase != CompositeGateTypeConstructionPhase.OUTPUTS)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }

            base.SetOutputPlugNames( outputPlugNamesString );

            constructionPhase = CompositeGateTypeConstructionPhase.GATES;
        }

        /// <summary>
        /// Adds a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The nested gate.</param>
        public void AddNestedGate( string nestedGate, Dictionary< string, GateType > gateTypes )
        {
            // Validate the construction phase.
            if (constructionPhase != CompositeGateTypeConstructionPhase.GATES)
            {
                // TODO: Provide more specifix exception.
                throw new Exception();
            }

            // Validate the nested gate string.
            if (nestedGate == null)
            {
                throw new ArgumentNullException( "nestedGate" );
            }

            string[] nestedGateNameAndType = nestedGate.Split( ' ' );

            if (nestedGateNameAndType.Length != 2)
            {
                // TODO: Provide more specific exception.
                throw new ArgumentException( "nestedGate" );
            }

            // Validate the name of the nested gate.
            string nestedGateName = nestedGateNameAndType[ 0 ];
            // Validate the legality of the name of the nested gate.
            if (!Program.IsLegalIdentifier( nestedGateName ))
            {
                throw new ArgumentException( "nestedGate" );
            }
            // Validate the uniqueness of the name of the nested gate.
            if (nestedGateTypes.ContainsKey( nestedGateName ))
            {
                throw new ArgumentException( "nestedGate" );
            }

            // Validate the type of the nested gate.
            string nestedGateTypeString = nestedGateNameAndType[ 1 ];
            // Validate the availability of the type of the nested gate.
            if (!gateTypes.ContainsKey( nestedGateTypeString ))
            {
                throw new ArgumentException( "nestedGate" );
            }

            // Retrieve the type of the nested gate.
            GateType nestedGateType = gateTypes[ nestedGateTypeString ];

            // Store the nested gate.
            nestedGateTypes.Add( nestedGateName, nestedGateType );
        }

        /// <summary>
        /// Adds a connection.
        /// </summary>
        /// 
        /// <param name="connection">The connection.</param>
        public void AddConnection( string connection )
        {
            if (constructionPhase == CompositeGateTypeConstructionPhase.GATES)
            {
                constructionPhase = CompositeGateTypeConstructionPhase.CONNECTIONS;
            }

            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }

            string[] connectionToAndFrom = Regex.Split( connection, "->" );

            if (connectionToAndFrom.Length != 2)
            {
                // TODO: Provide more specific exception.
                throw new ArgumentException( "connection" );
            }

            // TODO: Validate the connection.
            string connectionTo = connectionToAndFrom[ 0 ];
            string connectionFrom = connectionToAndFrom[1];

            // Store the connection.
            connections.Add( connectionTo, connectionFrom );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void EndConstruction()
        {
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
        INPUTS,
        OUTPUTS,
        GATES,
        CONNECTIONS,
        END
    }
}
