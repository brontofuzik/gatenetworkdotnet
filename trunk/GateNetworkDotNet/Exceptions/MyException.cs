using System;

namespace GateNetworkDotNet.Exceptions
{
    public class MyException
        : Exception
    {
        #region Private instance fields

        /// <summary>
        /// The number of the line where the syntax error occured.
        /// </summary>
        private readonly int lineNumber;

        /// <summary>
        /// The error message.
        /// </summary>
        private readonly string message;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// 
        /// <value>
        /// The error message.
        /// </value>
        public override string Message
        {
            get
            {
                return ((lineNumber != 0) ? ("Line " + lineNumber + ": ") : "") + message;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new syntax error.
        /// </summary>
        /// 
        /// <param name="lineNumber">The number of the line where the syntax error occured.</param>
        /// <param name="message">The error message.</param>
        public MyException( int lineNumber, string message )
        {
            this.lineNumber = lineNumber;
            this.message = message;
        }

        public MyException(string message)
            : this( 0, message )
        {
        }

        #endregion // Public instance constructors
    }
}
