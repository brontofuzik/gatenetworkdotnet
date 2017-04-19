using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LogicCircuit.Gates;

namespace LogicCircuit.GateTypes
{
    public class CompositeGateType : GateType
    {
        private static string[] implicitInputPlugNames = new string[] {"0","1"};

        private Dictionary<string, GateType> nestedGateTypes;

        private Dictionary<string, string> connections;

        private CompositeGateTypeConstructionPhase constructionPhase;

        public CompositeGateType()
        {
            nestedGateTypes = new Dictionary<string, GateType>();
            connections = new Dictionary<string, string>();
            constructionPhase = CompositeGateTypeConstructionPhase.NAME;
        }

        public Dictionary< string, GateType > NestedGateTypes
        {
            get
            {
                return nestedGateTypes;
            }
        }

        public int NestedGateCount
        {
            get
            {
                return nestedGateTypes.Count;
            }
        }

        public Dictionary<string, string> Connections
        {
            get
            {
                return connections;
            }
        }

        public int ConnectionCount
        {
            get
            {
                return connections.Count;
            }
        }

        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == CompositeGateTypeConstructionPhase.END);
            }
        }

        public override void Configure(string line, Dictionary<string, GateType> gateTypes)
        {
            string keyword = ParseKeyword(line);

            if (keyword.Equals("inputs") || keyword.Equals("outputs") || keyword.Equals("end"))
            {
                base.Configure(line, gateTypes);
            }
            else if (keyword.Equals("gate"))
            {
                // Set the nested gates.
                string[] nestedGate = ParseNestedGate(line);
                AddNestedGate(nestedGate, gateTypes);
            }
            else
            {
                // Set the connections.
                string[] connection = ParseConnection(line);
                AddConnection(connection);
            }
        }

        public string[] ParseNestedGate(string line)
        {
            // Split the line into words.
            string[] words = line.Split(' ');

            // Validate the number of words (3).
            if (words.Length != 3)
            {
                throw new Exception("Syntax error.");
            }

            // Return only the second and the third word.
            string[] nestedGate = new string[2];
            for (int i = 0; i < 2; i++)
            {
                nestedGate[i] = words[i + 1];
            }
            return nestedGate;
        }

        public string[] ParseConnection(string line)
        {
            // Split the line into words.
            string[] words = Regex.Split(line, "->");

            // Validate the number of words (2).
            if (words.Length != 2)
            {
                throw new Exception("Syntax error.");
            }

            // Return all the words.
            return words;
        }

        protected override void SetName(string name)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NAME)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetName(name);

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        public override void SetInputPlugNames(string[] inputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception("Missing keyword.");
            }

            string[] extendedInputPlugNames = new string[inputPlugNames.Length + implicitInputPlugNames.Length];
            for (int i = 0; i < inputPlugNames.Length; i++)
            {
                extendedInputPlugNames[i] = inputPlugNames[i];
            }
            for (int i = 0; i < implicitInputPlugNames.Length; i++)
            {
                extendedInputPlugNames[inputPlugNames.Length + i] = implicitInputPlugNames[i];
            }
            base.SetInputPlugNames(extendedInputPlugNames);

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        public override void SetOutputPlugNames(string[] outputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetOutputPlugNames(outputPlugNames);

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.NESTED_GATES;
        }
   
        public void AddNestedGate(string[] nestedGate, Dictionary< string, GateType > gateTypes)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NESTED_GATES && constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the nested gate.
            if (nestedGate == null)
            {
                throw new ArgumentNullException("nestedGate");
            }
            if (nestedGate.Length != 2)
            {
                throw new Exception("Syntax error (" + nestedGate + ").");
            }

            // Validate the types of gates.
            if (gateTypes == null)
            {
                throw new ArgumentNullException("gateTypes");
            }

            //
            // Validate the name of the nested gate.
            //
            string nestedGateName = nestedGate[0];

            // Validate the legality of the name of the nested gate.
            if (!Program.IsLegalName(nestedGateName))
            {
                throw new Exception("Syntax error (" + nestedGateName + ").");
            }
            // Validate the uniqueness of the name of the nested gate.
            if (nestedGateTypes.ContainsKey(nestedGateName))
            {
                throw new Exception("Duplicate (" + nestedGateName + ").");
            }

            //
            // Validate the type of the nested gate.
            //
            string nestedGateTypeString = nestedGate[1];

            // Retrieve the type of the nested gate.
            GateType nestedGateType;
            try
            {
                nestedGateType = gateTypes[nestedGateTypeString];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Syntax error (" + nestedGateTypeString + ").");
            }

            // Store the nested gate.
            nestedGateTypes.Add(nestedGateName, nestedGateType);

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.CONNECTIONS;
        }

        public void AddConnection(string[] connection)
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the connection.
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (connection.Length != 2)
            {
                throw new Exception("Syntax error (" + connection + ").");
            }
            
            //
            // Validate the endpoint of the connection.
            //
            string connectionTo = connection[0];

            if (connections.ContainsKey(connectionTo))
            {
                throw new Exception("Duplicate (" + connection + ").");
            }

            //
            // Validate the startpoint of the connection.
            //
            string connectionFrom = connection[1];

            // Store the connection.
            connections.Add(connectionTo, connectionFrom);
        }

        public virtual void ValidateConnections()
        {
            foreach (string outputPlugName in OutputPlugNames)
            {
                if (!connections.ContainsKey(outputPlugName))
                {
                    throw new Exception("Binding rule broken");
                }
            }
        }

        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the connections.
            ValidateConnections();

            // Advance the phase of construciton.
            constructionPhase = CompositeGateTypeConstructionPhase.END;
        }

        public override Gate Instantiate(string name)
        {
            return new CompositeGate(name, this);
        }
    }

    enum CompositeGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        NESTED_GATES,
        CONNECTIONS,
        END
    }
}
