using System;

namespace Network.Exceptions
{
    class IllegalNameException
        : Exception
    {
        #region Private static fields

        /// <summary>
        /// The message.
        /// </summary>
        private static string message = "Illegal name";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The illegal name.
        /// </summary>
        private readonly string name;

        #endregion // Private instance fields

        #region Public instance properties

        public override string Message
        {
            get
            {
                return message + ": " + name;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public IllegalNameException( string name )
        {
            this.name = name;
        }

        #endregion // Public instance constructors
    }
}
