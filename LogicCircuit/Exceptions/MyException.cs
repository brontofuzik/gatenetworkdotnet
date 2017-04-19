using System;

namespace LogicCircuit.Exceptions
{
    /// <summary>
    /// My exception.
    /// </summary>
    public class MyException
        : Exception
    {
        private readonly int lineNumber;

        private readonly string message;

        public MyException(int lineNumber, string message)
        {
            this.lineNumber = lineNumber;
            this.message = message;
        }

        public override string Message
        {
            get
            {
                return "Line " + lineNumber + ": " + message;
            }
        }
    }
}
