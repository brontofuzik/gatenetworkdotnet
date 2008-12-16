using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Connections;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A composite gate type.
    /// </summary>
    public class CompositeGateType
        : GateType
    {
        #region Private instance fields

        /// <summary>
        /// The gates comprising the composite gate type.
        /// </summary>
        private Dictionary< string, GateType > nestedGateTypes;

        /// <summary>
        /// The connections within the composite gate type.
        /// </summary>
        private Dictionary< string, string > connections;

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

        #endregion // Public instance properties

        #region Public instance contructors

        /// <summary>
        /// Creates a new gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// <param name="nestedGates">The nested gates.</param>
        /// <param name="connections">The connections.</param>
        /// <param name="gateTypes">The dictionary of known gate types.</param>
        /// 
        /// <exception cref="GateNetworkDorNet.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputPlugNames</c> contains an illegal name.
        /// Condition 3: <c>outputPlugNames</c> contains an illegal name.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Consition 1: <c>inputPlugNames</c> contains less than zero plug name. 
        /// Condition 2: <c>outputPlugNames</c> contains less than one plug name. 
        /// </exception>
        public CompositeGateType(string name, string inputPlugNames, string outputPlugNames, List< string > nestedGates, List< string > connections, Dictionary< string, GateType > gateTypes )
            : base( name, inputPlugNames, outputPlugNames )
        {
            //
            // Validate and construct the nested gates.
            //
            this.nestedGateTypes = new Dictionary< string, GateType >();
            foreach (string nestedGate in nestedGates)
            {
                string[] nestedGateNameAndType = nestedGate.Split( ' ' );

                if (nestedGateNameAndType.Length != 2)
                {
                    // TODO: Provide more specific exception.
                    throw new ArgumentException( "nestedGates" );
                }

                // Validate the name of the nested gate.
                string nestedGateName = nestedGateNameAndType[ 0 ];
                // Validate the legality of the name of the nested gate.
                if (!Program.IsLegalIdentifier( nestedGateName ))
                {
                    throw new ArgumentException( "nestedGates" );
                }
                // Validate the uniqueness of the name of the nested gate.
                if (this.nestedGateTypes.ContainsKey( nestedGateName ))
                {
                    throw new ArgumentException( "nestedGates" );
                }

                // Validate the type of the nested gate.
                string nestedGateTypeStr = nestedGateNameAndType[ 1 ];
                // Validate the availability of the type of the nested gate.
                if (!gateTypes.ContainsKey( nestedGateTypeStr ))
                {
                    throw new ArgumentException( "nestedGates" );
                }

                // Retrieve the type of the nested gate.
                GateType nestedGateType = gateTypes[ nestedGateTypeStr ];

                // Store the nested gate.
                this.nestedGateTypes.Add( nestedGateName, nestedGateType );
            }

            //
            // Validate and construct the connections.
            //
            this.connections = new Dictionary< string, string >();
            foreach (string connection in connections)
            {
                string[] connectionToAndFrom = Regex.Split( connection, "->" );

                if (connectionToAndFrom.Length != 2)
                {
                    // TODO: Provide more specific exception.
                    throw new ArgumentException( connection );
                }

                // TODO: Validate the connection.
                string connectionTo = connectionToAndFrom[ 0 ];
                string connectionFrom = connectionToAndFrom[ 1 ];

                // Store the connection.
                this.connections.Add( connectionTo, connectionFrom );
            }
        }

        #endregion // Public instance contructors

        #region Public instance methods

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

        /// <summary>
        /// Evaluates the transition function of the composite gate type.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        /// 
        /// <returns>
        /// The values of the output plugs.
        /// </returns>
        public override string[] Evaluate( string[] inputPlugValues )
        {
            throw new NotImplementedException();
        }

        #endregion // Public instance methods
    }
}
