using System.Collections.Generic;

using GateNetworkDotNet.GateTypes;
using GateNetworkDotNet.Gates.Connections;

namespace GateNetworkDotNet.Gates
{
    class CompositeGate
        : Gate
    {
        #region Private instance fields

        /// <summary>
        /// The nested gates.
        /// </summary>
        private Dictionary< string, Gate > nestedGates;

        /// <summary>
        /// The connections;
        /// </summary>
        private Connection[] connections; 

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The nested gates.
        /// </value>
        public Dictionary< string, Gate > NestedGates
        {
            get
            {
                return nestedGates;
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
                return (Type as CompositeGateType).NestedGateCount;
            }
        }

        /// <summary>
        /// Gets the connections
        /// </summary>
        /// 
        /// <value>
        /// The connections.
        /// </value>
        public Connection[] Connections
        {
            get
            {
                return connections;
            }
        }

        /// <summary>
        /// Gets the number of connections.
        /// </summary>
        /// 
        /// <value>
        /// The number of connections.
        /// </value>
        public int ConnectionCount
        {
            get
            {
                return (Type as CompositeGateType).ConnectionCount;
            }
        }

        #endregion // Public instance properties

        #region Public instance contructors

        /// <summary>
        /// Creates a new composite gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate.</param>
        /// <param name="type">The type of the composite gate.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal composite gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        public CompositeGate( string name, CompositeGateType type )
            : base( name, type )
        {
            //
            // Initialize the output plugs.
            // TODO: Provide implementation.
            
            //
            // Construct the nested gates.
            //
            nestedGates = new Dictionary< string, Gate >();
            foreach (KeyValuePair< string, GateType > kvp in (Type as CompositeGateType).NestedGateTypes)
            {
                string nestedGateName = kvp.Key;
                GateType nestedGateType = kvp.Value;
                Gate nestedGate = nestedGateType.Instantiate( nestedGateName );

                nestedGates.Add( nestedGateName, nestedGate ); 
            }

            //
            // Construct the connections.
            //
            connections = new Connection[ ConnectionCount ];
            int connectionIndex = 0;
            foreach (KeyValuePair< string, string > kvp in (Type as CompositeGateType).Connections)
            {
                string connectionTo = kvp.Key;
                string connectionFrom = kvp.Value;
            }
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate.
        /// </summary>
        public override void Evaluate()
        {
            throw new System.NotImplementedException();
        }

        #endregion // Public instance methods
    }
}
