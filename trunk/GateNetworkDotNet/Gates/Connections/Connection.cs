using System;

using GateNetworkDotNet.Gates.Plugs;

namespace GateNetworkDotNet.Gates.Connections
{
    /// <summary>
    /// A connection.
    /// </summary>
    public class Connection
        : IConnection
    {
        #region Private instance fields

        /// <summary>
        /// The source plug.
        /// </summary>
        private Plug sourcePlug;

        /// <summary>
        /// The target plug.
        /// </summary>
        private Plug targetPlug;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets or sets the source plug.
        /// </summary>
        /// 
        /// <value>
        /// The source plug.
        /// </value>
        public Plug SourcePlug
        {
            get
            {
                return sourcePlug;
            }
            set
            {
                sourcePlug = value;
            }
        }

        /// <summary>
        /// Gets or sets the target plug.
        /// </summary>
        /// 
        /// <value>
        /// The target plug.
        /// </value>
        public Plug TargetPlug
        {
            get
            {
                return targetPlug;
            }
            set
            {
                targetPlug = value;
            }
        }

        #endregion // Public instance properties
    }
}
