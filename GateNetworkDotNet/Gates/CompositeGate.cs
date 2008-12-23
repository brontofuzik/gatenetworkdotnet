using System;
using System.Collections.Generic;

using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    public class CompositeGate
        : Gate
    {
        #region Private instance fields

        /// <summary>
        /// The type of the composite gate.
        /// </summary>
        private CompositeGateType type;

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
                return type.NestedGateCount;
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
                return type.ConnectionCount;
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
            // Validate the composite gate type.
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            this.type = type;

            //
            // Construct the nested gates.
            //
            nestedGates = new Dictionary< string, Gate >();
            foreach (KeyValuePair< string, GateType > kvp in type.NestedGateTypes)
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
            foreach (KeyValuePair< string, string > kvp in type.Connections)
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

                // Initialize the implicit input plugs.
                GetInputPlugByName( "0" ).Value = "0";
                GetInputPlugByName( "1" ).Value = "1";
            }
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        public override void SetInputPlugValues( string inputPlugValuesString )
        {
            string[] inputPlugValues = inputPlugValuesString.Split( ' ' );
            for (int i = 0; i < InputPlugCount - 2; i++)
            {
                InputPlugs[ i ].Value = inputPlugValues[ i ];
            }
        }

        /// <summary>
        /// Initializes the composite gate.
        /// </summary>
        public override void Initialize()
        {
            // Initialize the nested gates.
            foreach (KeyValuePair< string, Gate > kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.Initialize();
            }
        }

        /// <summary>
        /// Evaluates the composite gate.
        /// </summary>
        public override void Evaluate()
        {
            foreach (KeyValuePair< string, Gate > kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.Evaluate();
            }
        }

        #endregion // Public instance methods
    }
}
