using System;

namespace Network.Exceptions
{
    class IllegalTransitionException
        : Exception
    {
        #region Private stativ fields

        /// <summary>
        /// The message.
        /// </summary>
        private static string message = "Illegal transition";

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// 
        /// </summary>
        private string transition;
        
        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Get the message.
        /// </summary>
        /// 
        /// <value>
        /// The message;
        /// </value>
        public override string Message
        {
            get
            {
                return message + ": " + transition;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transition"></param>
        public IllegalTransitionException( string transition )
        {
            this.transition = transition;
        }

        #endregion // Public instance constructors
    }
}
