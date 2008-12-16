using System;

namespace GateNetworkDotNet.Exceptions
{
    /// <summary>
    /// An illegal inputs exception.
    /// </summary>
    public class IllegalInputsException
        : Exception
    {
        #region Private static fields

        /// <summary>
        /// 
        /// </summary>
        private static string message = "Illegal inputs";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The illegal inputs.
        /// </summary>
        private string inputs;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// 
        /// <value>
        /// The message.
        /// </value>
        public override string Message
        {
            get
            {
                return message + ": " + inputs;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new illegal inputs exception.
        /// </summary>
        /// 
        /// <param name="inputs">The illegal inputs.</param>
        public IllegalInputsException( string inputs )
        {
            this.inputs = inputs;
        }

        #endregion // Public instance constructors
    }
}
