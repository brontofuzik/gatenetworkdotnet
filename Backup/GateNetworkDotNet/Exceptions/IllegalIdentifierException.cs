using System;

namespace GateNetworkDotNet.Exceptions
{
    class IllegalIdentifierException
        : Exception
    {
        #region Private static fields

        /// <summary>
        /// The message.
        /// </summary>
        private static string message = "Illegal identifier";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The illegal identifier.
        /// </summary>
        private readonly string identifier;

        #endregion // Private instance fields

        #region Public instance properties

        public override string Message
        {
            get
            {
                return message + ": " + identifier;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        public IllegalIdentifierException( string identifier )
        {
            this.identifier = identifier;
        }

        #endregion // Public instance constructors
    }
}
