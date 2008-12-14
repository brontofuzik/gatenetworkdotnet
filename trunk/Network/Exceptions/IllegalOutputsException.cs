using System;

namespace Network.Exceptions
{
    /// <summary>
    /// An illegal outputs exception.
    /// </summary>
    public class IllegalOutputsException
        : Exception
    {
        #region Private static fields

        /// <summary>
        /// 
        /// </summary>
        private static string message = "Illegal outputs";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The illegal outputs.
        /// </summary>
        private string outputs;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// 
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                return message + ": " + outputs;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new illegal outputs exception.
        /// </summary>
        /// 
        /// <param name="inputs">The outputs inputs.</param>
        public IllegalOutputsException( string outputs )
        {
            this.outputs = outputs;
        }

        #endregion // Public instance constructors
    }
}
