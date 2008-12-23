using System;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// A connection.
    /// </summary>
    public class Connection
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

        #region Public insatnce methods

        /// <summary>
        /// Transmits the value from the source plug to the target plug.
        /// </summary>
        public void Transmit()
        {
            targetPlug.Value = sourcePlug.Value;
        }

        #endregion // Public instance methods
    }
}
