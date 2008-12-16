using System.Collections.Generic;

using GateNetworkDotNet.Gates.Connections;

namespace GateNetworkDotNet.Gates.Plugs
{
    /// <summary>
    /// An plug.
    /// </summary>
    public class Plug
        : IPlug
    {
        #region Private insatance fields

        /// <summary>
        /// The value of the plug.
        /// </summary>
        private string value;

        /// <summary>
        /// The input connection plugged into the plug.
        /// </summary>
        private List< Connection > sourceConnections;

        /// <summary>
        /// The output connections plugged into the plug.
        /// </summary>
        private List< Connection > targetConnections;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets or sets the value of the plug.
        /// </summary>
        /// 
        /// <value>
        /// The value of the plug.
        /// </value>
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors
        
        /// <summary>
        /// Creates a new plug.
        /// </summary>
        public Plug()
        {
            sourceConnections = new List< Connection >();
            targetConnections = new List< Connection >();
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Plugs a source connection into the plug.
        /// </summary>
        /// 
        /// <param name="connection">The source connection.</param>
        public void PlugSourceConnection( Connection connection )
        {
            sourceConnections.Add( connection );
            connection.TargetPlug = this;
        }

        /// <summary>
        /// Plugs a target connecion into the plug.
        /// </summary>
        /// 
        /// <param name="connection">The target connection.</param>
        public void PlugTargetConnection( Connection connection )
        {
            targetConnections.Add( connection );
            connection.SourcePlug = this;
        }

        #endregion // Public instance methods
    }
}
