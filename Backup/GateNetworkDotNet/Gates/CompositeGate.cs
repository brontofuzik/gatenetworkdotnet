using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    class CompositeGate
        : Gate
    {
        #region Public instance contructors

        /// <summary>
        /// Creates a new composite gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate.</param>
        /// <param name="type">The type of the composite gate.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal composite gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        protected CompositeGate( string name, GateType type )
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
            throw new System.NotImplementedException();
        }

        #endregion // Public instance methods
    }
}
