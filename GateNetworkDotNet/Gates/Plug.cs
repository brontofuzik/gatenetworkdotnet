using System;
using System.Collections.Generic;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// An plug.
    /// </summary>
    public class Plug
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

        /// <summary>
        /// The parent gate of the plug.
        /// </summary>
        private Gate parentGate;

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
                // If the new value of the plug is different from the old one, ...
                if (this.value != value)
                {
                    // ... update the value of the plug, ...
                    this.value = value;
                    // ... and trasmit the new value through the target connections into the target plugs.
                    foreach (Connection targetConnection in targetConnections)
                    {
                        targetConnection.Transmit();
                    }

                    // If the parent gate of the plug is a basic gate, ...
                    if (parentGate is BasicGate)
                    {
                        // ... it needs to be re-evaluated.
                        (parentGate as BasicGate).IsEvaluated = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of source connections plugged into the plug.
        /// </summary>
        /// 
        /// <value>
        /// The number of source connections plugged into the plug.
        /// </value>
        public int SourceConnectionCount
        {
            get
            {
                return sourceConnections.Count;
            }
        }

        /// <summary>
        /// Gets the number of target connections plugged into the plug.
        /// </summary>
        /// 
        /// <value>
        /// The number of target connections plugged into the plug.
        /// </value>
        public int TargetConnectionCount
        {
            get
            {
                return targetConnections.Count;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors
        
        /// <summary>
        /// Creates a new plug.
        /// </summary>
        public Plug( Gate parentGate )
        {
            value = "?";

            sourceConnections = new List< Connection >();
            targetConnections = new List< Connection >();

            if (parentGate == null)
            {
                throw new ArgumentNullException();
            }
            this.parentGate = parentGate;
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
            if (SourceConnectionCount == 1)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }
            sourceConnections.Add( connection );
            connection.TargetPlug = this;
        }

        /// <summary>
        /// Plugs a target connecion into the plug.
        /// </summary>
        /// 
        /// <param name="connection">The target connection.</param>
        public void PlugTargetConnection(Connection connection)
        {
            targetConnections.Add( connection );
            connection.SourcePlug = this;
        }

        #endregion // Public instance methods
    }
}
