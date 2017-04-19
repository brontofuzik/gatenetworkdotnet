using System;
using System.Text;
using LogicCircuit.GateTypes;

namespace LogicCircuit.Gates
{
    public abstract class Gate
    {
        private string name;

        private GateType type;

        private Plug[] inputPlugs;

        private Plug[] outputPlugs;

        protected Gate(string name, GateType type)
        {
            // Validate the name.
            if (!Program.IsLegalName(name))
            {
                throw new ArgumentException(name);
            }
            this.name = name;

            // Validate the (abstract) gate type.
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            this.type = type;

            // Create the input plugs.
            inputPlugs = new Plug[InputPlugCount];
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugs[i] = new Plug(this);
            }

            // Create the output plugs.
            outputPlugs = new Plug[OutputPlugCount];
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugs[i] = new Plug(this);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Plug[] InputPlugs
        {
            get
            {
                return inputPlugs;
            }
        }

        public int InputPlugCount
        {
            get
            {
                return type.InputPlugCount;
            }
        }

        public Plug[] OutputPlugs
        {
            get
            {
                return outputPlugs;
            }
        }

        public int OutputPlugCount
        {
            get
            {
                return type.OutputPlugCount;
            }
        }

        public Plug GetInputPlugByName(string inputPlugName)
        {
            int inputPlugIndex = type.GetInputPlugIndex(inputPlugName);
            return (inputPlugIndex != -1) ? InputPlugs[inputPlugIndex] : null;
        }

        public Plug GetOutputPlugByName(string outputPlugName)
        {
            int outputPlugIndex = type.GetOutputPlugIndex(outputPlugName);
            return (outputPlugIndex != -1) ? OutputPlugs[outputPlugIndex] : null;
        }

        public string GetInputPlugValues()
        {
            // Build the string representation of the values of the input plugs.
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append(InputPlugs[i].Value + " ");
            }
            // Remove the trailing space character if necessary.
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove(inputPlugValuesSB.Length - 1, 1);
            }
            return inputPlugValuesSB.ToString();
        }

        public abstract void SetInputPlugValues(string inputPlugValuesString);

        public string GetOutputPlugValues()
        {
            // Build the string representation of the values of the output plugs.
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugValuesSB.Append(OutputPlugs[i].Value + " ");
            }
            // Remove the trailing space character if necessary.
            if (outputPlugValuesSB.Length != 0)
            {
                outputPlugValuesSB.Remove(outputPlugValuesSB.Length - 1, 1);
            }
            return outputPlugValuesSB.ToString();
        }

        public void SetOutputPlugValues(string outputPlugValuesString)
        {
            string[] outputPlugValues = outputPlugValuesString.Split(' ');
            for (int i = 0; i < OutputPlugCount; i++)
            {
                OutputPlugs[i].Value = outputPlugValues[i];
            }
        }

        public abstract void Initialize();

        public abstract bool UpdateInputPlugValues();

        public abstract void UpdateOutputPlugValues();
    }
}
