using System;
using System.Text;
using LogicCircuit.GateTypes;

namespace LogicCircuit.Gates
{
    public class BasicGate : Gate
    {
        private BasicGateType type;

        public BasicGate( string name, BasicGateType type )
            : base( name, type )
        {
            if (type == null)
            {
                throw new ArgumentNullException( "type" );
            }
            this.type = type;
        }

        public override void SetInputPlugValues( string inputPlugValuesString )
        {
            string[] inputPlugValues = inputPlugValuesString.Split( ' ' );
            if (inputPlugValues.Length != InputPlugCount)
            {
                throw new Exception( "Syntax error." );
            }

            for (int i = 0; i < InputPlugCount; i++)
            {
                InputPlugs[ i ].Value = inputPlugValues[ i ];
            }
        }

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

        public override bool UpdateInputPlugValues()
        {
            bool updatePerformed = false;

            foreach (Plug inputPlug in InputPlugs)
            {
                updatePerformed = inputPlug.UpdatePlugValue() || updatePerformed;
            }

            return updatePerformed;
        }

        public override void UpdateOutputPlugValues()
        {
            string inputPlugValues = GetInputPlugValues();

            string outputPlugValues = type.Evaluate(inputPlugValues);

            SetOutputPlugValues(outputPlugValues);
        }
    }
}
