using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;

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
        private Connection sourceConnection;

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
                if (!IsLegalPlugValue( value ))
                {
                    throw new Exception("Syntax error.");
                }
                this.value = value;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors
        
        /// <summary>
        /// Creates a new plug.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>parentGate</c> is <c>null</c>.
        /// </exception>
        public Plug( Gate parentGate )
        {
            value = "?";

            sourceConnection = null;
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
        /// <param name="sourceConnection">The source connection.</param>
        public void PlugSourceConnection( Connection sourceConnection )
        {
            if (this.sourceConnection != null)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }
            this.sourceConnection = sourceConnection;
            
            sourceConnection.TargetPlug = this;
        }

        /// <summary>
        /// Plugs a target connecion into the plug.
        /// </summary>
        /// 
        /// <param name="targetConnection">The target connection.</param>
        public void PlugTargetConnection( Connection targetConnection )
        {
            targetConnections.Add( targetConnection );
            
            targetConnection.SourcePlug = this;
        }

        /// <summary>
        /// Updates the value of the (input or output) plug.
        /// </summary>
        public bool UpdatePlugValue()
        {
            if (sourceConnection != null)
            {
                if (value != sourceConnection.SourcePlug.Value)
                {
                    value = sourceConnection.SourcePlug.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion // Public instance methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugValue"></param>
        /// <returns></returns>
        public static bool IsLegalPlugValue( string plugValue )
        {
            return (plugValue.Equals("0") || plugValue.Equals("1") || plugValue.Equals("?"));
        }
    }
}