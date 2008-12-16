using System.Collections.Generic;

using GateNetworkDotNet.Gates.Connections

namespace GateNetworkDotNet.Gates.Plugs
{
    /// <summary>
    /// An abstract plug.
    /// </summary>
    public abstract class Plug
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
        private List< Connection > inputConnections;

        /// <summary>
        /// The output connections plugged into the plug.
        /// </summary>
        private List< Connection > outputConnections;

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
    }
}
