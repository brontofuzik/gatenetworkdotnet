using System;
using System.Collections.Generic;
using System.Text;

using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet.Gates
{
    /// <summary>
    /// A basic gate.
    /// </summary>
    class BasicGate
        : Gate
    {
        #region Private instance fields

        /// <summary>
        /// The type of the basic gate.
        /// </summary>
        private BasicGateType type;

        /// <summary>
        /// Is the basic gate evaluated.
        /// </summary>
        private bool isEvaluated;
        
        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Determines whether the basic gate is evaluated.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the basic gate is evaluated, <c>false</c> otherwise.
        /// </value>
        public bool IsEvaluated
        {
            get
            {
                return isEvaluated;
            }
            set
            {
                isEvaluated = value;
            }
        }

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

            // The newly constructed basic gate is not yet evaluated.
            isEvaluated = false;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        public override void SetInputPlugValues( string inputPlugValuesString )
        {
            string[] inputPlugValues = inputPlugValuesString.Split( ' ' );
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
                Evaluate();
            }
            else
            {
                StringBuilder outputPlugValuesSB = new StringBuilder();
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    outputPlugValuesSB.Append("?" + " ");
                }
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
                string outputPlugValues = outputPlugValuesSB.ToString();

                SetOutputPlugValues( outputPlugValues );

                isEvaluated = false;
            }
        }

        /// <summary>
        /// Evaluates the basic gate.
        /// </summary>
        public override void Evaluate()
        {
            if (!isEvaluated)
            {
                string inputPlugValues = GetInputPlugValues();
                string outputPlugValues = type.Evaluate( inputPlugValues );
                SetOutputPlugValues( outputPlugValues );
                
                isEvaluated = true;
            }
        }

        #endregion // Public instance methods
    }
}
