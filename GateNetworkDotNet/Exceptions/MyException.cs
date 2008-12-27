using System;

namespace GateNetworkDotNet.Exceptions
{
    /// <summary>
    /// My exception.
    /// </summary>
    public class MyException
        : Exception
    {
        #region Private instance fields

        /// <summary>
        /// The number of the line where the exception has been thrown.
        /// </summary>
        private readonly int lineNumber;

        /// <summary>
        /// The message of the exception.
        /// </summary>
        private readonly string message;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the message of the exception.
        /// </summary>
        /// 
        /// <value>
        /// The message of the exception.
        /// </value>
        public override string Message
        {
            get
            {
                return "Line " + lineNumber + ": " + message;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// 
        /// <param name="lineNumber">The number of the line where the exception has been thrown.</param>
        /// <param name="message">The message of the exception.</param>
        public MyException( int lineNumber, string message )
        {
            this.lineNumber = lineNumber;
            this.message = message;
        }

        #endregion // Public instance constructors
    }
}
