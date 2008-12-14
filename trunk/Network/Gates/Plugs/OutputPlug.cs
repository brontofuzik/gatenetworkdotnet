using System.Collections.Generic;

namespace Network.Gates.Plugs
{
    /// <summary>
    /// An output plug.
    /// </summary>
    public class OutputPlug
        : Plug
    {
        #region Private instance fields

        /// <summary>
        /// The output connections plugged into the plug.
        /// </summary>
        private List< Connection > outputConnections;

        #endregion // Private instance fields
    }
}
