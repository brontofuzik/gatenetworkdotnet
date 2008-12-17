using System.Collections.Generic;

using GateNetworkDotNet.GateTypes;
using GateNetworkDotNet.Gates.Connections;
using GateNetworkDotNet.Gates.Plugs;

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
                //
                // Get the target plug.
                //
                string connectionTarget = kvp.Key;
                string[] targetPlugName = connectionTarget.Split( '.' );
                bool nestedTargetPlug = (targetPlugName.Length != 1);
                
                // Depending on whether the target plug is a nested plug, the target gate is ...
                Gate targetGate = nestedTargetPlug ?
                    // ... a nested gate.
                    nestedGates[ targetPlugName[ 0 ] ] :
                    // ... this composite gate.
                    this;
                
                // Depending on whether the target plug is a nested plug, the target plug is ...
                Plug targetPlug = nestedTargetPlug ?
                    // ... an input plug of a nested gate.
                    targetGate.GetInputPlugByName( targetPlugName[ 1 ] ) :
                    // ... an output plug of this composite gate.
                    targetGate.GetOutputPlugByName( targetPlugName[ 0 ] );
                    
                //
                // Get the source plug.
                //
                string connectionSource = kvp.Value;
                string[] sourcePlugName = connectionSource.Split( '.' );
                bool nestedSourcePlug = (sourcePlugName.Length != 1);

                // Depending on whether the source plug is a nested plug, the source gate is ...
                Gate sourceGate = nestedSourcePlug ?
                    // ... a nested gate.
                    nestedGates[ sourcePlugName[ 0 ] ] :
                    // ... this composite gate.
                    this;

                // Depending on whether the source plug is a nested plug, the source plug is ...
                Plug sourcePlug = nestedSourcePlug ?
                    // ... an output plug of a nested gate.
                    sourceGate.GetOutputPlugByName( sourcePlugName[ 1 ] ) :
                    // ... an input plug of this composite gate.
                    sourceGate.GetInputPlugByName( sourcePlugName[ 0 ] );

                // Construct the connection.
                Connection connection = new Connection();
                sourcePlug.PlugTargetConnection( connection );
                targetPlug.PlugSourceConnection( connection );

                connections[ connectionIndex++ ] = connection; 
            }
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate.
        /// </summary>
        public override void Evaluate()
        {
            Type.Evaluate( InputPlugs, OutputPlugs );
        }

        #endregion // Public instance methods
    }
}
