using System;

using Network.GateTypes;

namespace Network.Gates
{
    /// <summary>
    /// A basic gate.
    /// </summary>
    class BasicGate
        : Gate
    {
        #region Public instance contructors

        /// <summary>
        /// Creates a new basic gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate.</param>
        /// <param name="type">The type of the basic gate.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal basic gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        public BasicGate( string name, GateType type )
            : base( name, type )
        {
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate.
        /// </summary>
        public override void Evaluate()
        {
            throw new NotImplementedException();
        }

        #endregion // Public instance methods
    }
}
