using System;
using System.Collections.Generic;
using System.Text;
using LogicCircuit.Gates;

namespace LogicCircuit.GateTypes
{
    public class BasicGateType : GateType
    {
        private Dictionary< string, string > transitions;

        private BasicGateTypeConstructionPhase constructionPhase;

        public BasicGateType()
        {
            transitions = new Dictionary<string, string>();
            constructionPhase = BasicGateTypeConstructionPhase.NAME;
        }

        public Dictionary< string, string > Transitions
        {
            get
            {
                return transitions;
            }
        }

        public int TransitionCount
        {
            get
            {
                return transitions.Count;
            }
        }

        public int TransitionLength
        {
            get
            {
                return InputPlugCount + OutputPlugCount;
            }
        }

        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == BasicGateTypeConstructionPhase.END);
            }
        }

        public override void Configure(string line, Dictionary< string, GateType > gateTypes)
        {
            string keyword = ParseKeyword(line);

            if (keyword.Equals("inputs") || keyword.Equals("outputs") || keyword.Equals("end"))
            {
                base.Configure(line, gateTypes);
            }
            else
            {
                // Set the transitions.
                string[] transition = ParseTransition(line);
                AddTransition(transition);
            }
        }

        public string[] ParseTransition(string line)
        {
            // Split the line into words.
            string[] words = line.Split(' ');
            
            // Return all the words.
            return words;
        }

        protected override void SetName(string name)
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.NAME)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetName(name);

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        public override void SetInputPlugNames(string[] inputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetInputPlugNames(inputPlugNames);

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        public override void SetOutputPlugNames(string[] outputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetOutputPlugNames(outputPlugNames);

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.TRANSITIONS;
        }

        public void AddTransition(string[] transition)
        {
            // Validate the phase of construciton.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the transition.
            if (transition == null)
            {
                throw new ArgumentNullException("transition");
            }
            if (transition.Length != TransitionLength)
            {
                throw new Exception("Syntax error (" + transition + ").");
            }
            foreach (string transitionValue in transition)
            {
                if (!Plug.IsLegalPlugValue(transitionValue))
                {
                    throw new Exception("Syntax error (" + transition + ").");
                }
            }

            //
            // Build the string containing the values of the input strings.
            //
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append(transition[i] + " ");
            }
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove(inputPlugValuesSB.Length - 1, 1);
            }
            string inputPlugValues = inputPlugValuesSB.ToString();

            if (transitions.ContainsKey(inputPlugValues))
            {
                throw new Exception("Duplicate (" + transition + ").");
            }

            //
            // Build the string containing the values of the output strings.
            //
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = InputPlugCount; i < TransitionLength; i++)
            {
                outputPlugValuesSB.Append(transition[i] + " ");
            }
            if (outputPlugValuesSB.Length != 0)
            {
                outputPlugValuesSB.Remove(outputPlugValuesSB.Length - 1, 1);
            }
            string outputPlugValues = outputPlugValuesSB.ToString();

            // Add the (inputPlugValues, outputPlugValues) key-value-pair into the transitions.
            transitions.Add(inputPlugValues, outputPlugValues);
        }

        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.END;
        }

        public override Gate Instantiate(string name)
        {
            return new BasicGate(name, this);
        }

        public string Evaluate(string inputPlugValues)
        {
            // TODO: Think about replacing the following piece of code with TryGetMethod.
            string outputPlugValues;
            try
            {
                // The transition function contains the mapping for the inputs.
                outputPlugValues = transitions[inputPlugValues];
            }
            catch (KeyNotFoundException)
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                string outputPlugValue = inputPlugValues.Contains("?") ? "?" : "0";
                
                StringBuilder outputPlugValuesSB = new StringBuilder();
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    outputPlugValuesSB.Append(outputPlugValue + " ");
                }
                if (outputPlugValuesSB.Length != 0)
                {
                    outputPlugValuesSB.Remove(outputPlugValuesSB.Length - 1, 1);
                }
                outputPlugValues = outputPlugValuesSB.ToString();
            }

            // Return the values of the output plugs.
            return outputPlugValues;
        }
    }

    enum BasicGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        TRANSITIONS,
        END
    }
}
