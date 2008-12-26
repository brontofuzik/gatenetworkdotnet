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
                // If the number of the line has been specified, include it in the exception message.
                if (lineNumber != 0)
                {
                    return "Line " + lineNumber + ": " + message;
                }
                else
                {
                    return message;
                }
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

        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// 
        /// <param name="message">The message of the exception.</param>
        public MyException( string message )
            : this( 0, message )
        {
        }

        #endregion // Public instance constructors
    }
}
