using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// A basic gate.
    /// </summary>
    public class BasicGate
        : Gate
    {
        #region Private instance fields

        /// <summary>
        /// The type of the basic gate.
        /// </summary>
        private BasicGateType type;
        
        #endregion // Private instance fields

        #region Public instance properties

        #endregion // Public instance properties

        #region Public instance contructors

        /// <summary>
        /// Creates a new basic gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate.</param>
        /// <param name="type">The type of the basic gate.</param>
        /// 
        /// <exception cref="GateNetworkDotNet.Exceptions.IllegalNameException">
        /// Condition: <c>name</c> is not a legal basic gate name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>type</c> is <c>null</c>.
        /// </exception>
        public BasicGate( string name, BasicGateType type )
            : base( name, type )
        {
            // Validate the basic gate type.
            if (type == null)
            {
                throw new ArgumentNullException( "type" );
            }
            this.type = type;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValuesString">The values of the input plugs.</param>
        public override void SetInputPlugValues(string inputPlugValuesString )
        {
            string[] inputPlugValues = inputPlugValuesString.Split( ' ' );
            if (inputPlugValues.Length != InputPlugCount)
            {
                throw new MyException( "Syntax error." );
            }

            for (int i = 0; i < InputPlugCount; i++)
            {
                InputPlugs[ i ].Value = inputPlugValues[ i ];
            }
        }

        /// <summary>
        /// Initializes the basic gate.
        /// </summary>
        public override void Initialize()
        {
            if (InputPlugCount == 0)
            {
                // If the gate has no input plugs, it is initialized using the transition function.
                UpdateOutputPlugValues();
            }
            else
            {
                // If the basic gate has at least one input plug, it is initialized using the "?" values.
                StringBuilder outputPlugValuesSB = new StringBuilder();
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    outputPlugValuesSB.Append("?" + " ");
                }
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
                string outputPlugValues = outputPlugValuesSB.ToString();

                SetOutputPlugValues( outputPlugValues );
            }
        }

        /// <summary>
        /// Updates the values of the input plugs of the basic gate.
        /// </summary>
        public override bool UpdateInputPlugValues()
        {
            bool updatePerformed = false;

            foreach (Plug inputPlug in InputPlugs)
            {
                updatePerformed = inputPlug.UpdatePlugValue() || updatePerformed;
            }

            return updatePerformed;
        }

        /// <summary>
        /// Evaluates the basic gate.
        /// </summary>
        public override void UpdateOutputPlugValues()
        {
            string inputPlugValues = GetInputPlugValues();

            string outputPlugValues = type.Evaluate(inputPlugValues);

            SetOutputPlugValues(outputPlugValues);
        }

        #endregion // Public instance methods
    }
}
