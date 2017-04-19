using System;
using System.Collections.Generic;
using LogicCircuit.GateTypes;

namespace LogicCircuit.Gates
{
    public class CompositeGate : Gate
    {
        private CompositeGateType type;

        private Dictionary< string, Gate > nestedGates;

        private Connection[] connections;

        public CompositeGate(string name, CompositeGateType type)
            : base(name, type)
        {
            // Validate the composite gate type.
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            this.type = type;

            //
            // Construct the nested gates.
            //
            nestedGates = new Dictionary<string, Gate>();
            foreach (KeyValuePair<string, GateType> kvp in type.NestedGateTypes)
            {
                string nestedGateName = kvp.Key;
                GateType nestedGateType = kvp.Value;
                Gate nestedGate = nestedGateType.Instantiate(nestedGateName);

                nestedGates.Add(nestedGateName, nestedGate);
            }

            //
            // Construct the connections.
            //
            connections = new Connection[ConnectionCount];
            int connectionIndex = 0;
            foreach (KeyValuePair<string, string> kvp in type.Connections)
            {
                //
                // Get the target plug.
                //
                string connectionTarget = kvp.Key;
                string[] targetPlugName = connectionTarget.Split('.');
                bool nestedTargetPlug = (targetPlugName.Length != 1);

                // Depending on whether the target plug is a nested plug, the target gate is ...
                Gate targetGate = nestedTargetPlug ?
                    // ... a nested gate.
                    nestedGates[targetPlugName[0]] :
                    // ... this composite gate.
                    this;

                // Depending on whether the target plug is a nested plug, the target plug is ...
                Plug targetPlug = nestedTargetPlug ?
                    // ... an input plug of a nested gate.
                    targetGate.GetInputPlugByName(targetPlugName[1]) :
                    // ... an output plug of this composite gate.
                    targetGate.GetOutputPlugByName(targetPlugName[0]);

                //
                // Get the source plug.
                //
                string connectionSource = kvp.Value;
                string[] sourcePlugName = connectionSource.Split('.');
                bool nestedSourcePlug = (sourcePlugName.Length != 1);

                // Depending on whether the source plug is a nested plug, the source gate is ...
                Gate sourceGate = nestedSourcePlug ?
                    // ... a nested gate.
                    nestedGates[sourcePlugName[0]] :
                    // ... this composite gate.
                    this;

                // Depending on whether the source plug is a nested plug, the source plug is ...
                Plug sourcePlug = nestedSourcePlug ?
                    // ... an output plug of a nested gate.
                    sourceGate.GetOutputPlugByName(sourcePlugName[1]) :
                    // ... an input plug of this composite gate.
                    sourceGate.GetInputPlugByName(sourcePlugName[0]);

                // Construct the connection.
                Connection connection = new Connection();
                sourcePlug.PlugTargetConnection(connection);
                targetPlug.PlugSourceConnection(connection);

                connections[connectionIndex++] = connection;
            }

            //
            //
            //
            GetInputPlugByName("0").Value = "0";
            GetInputPlugByName("1").Value = "1";
        }

        public Dictionary< string, Gate > NestedGates
        {
            get
            {
                return nestedGates;
            }
        }

        public int NestedGateCount
        {
            get
            {
                return type.NestedGateCount;
            }
        }

        public Connection[] Connections
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
                return type.ConnectionCount;
            }
        }

        public override void SetInputPlugValues(string inputPlugValuesString)
        {
            string[] inputPlugValues = inputPlugValuesString.Split(' ');
            if (inputPlugValues.Length != InputPlugCount - 2)
            {
                throw new Exception("Syntax error.");
            }

            for (int i = 0; i < InputPlugCount - 2; i++)
            {
                InputPlugs[i].Value = inputPlugValues[i];
            }
        }

        public override void Initialize()
        {
            foreach (KeyValuePair<string, Gate> kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.Initialize();
            }
        }

        public override bool UpdateInputPlugValues()
        {
            foreach (Plug inputPlug in InputPlugs)
            {
                inputPlug.UpdatePlugValue();
            }

            bool updatePerformed = false;

            foreach (KeyValuePair< string, Gate > kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                updatePerformed = nestedGate.UpdateInputPlugValues() || updatePerformed;
            }

            return updatePerformed;
        }

        public override void UpdateOutputPlugValues()
        {
            foreach (KeyValuePair<string, Gate> kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.UpdateOutputPlugValues();
            }
            foreach (Plug outputPlug in OutputPlugs)
            {
                outputPlug.UpdatePlugValue();
            }
        }

        public string Evaluate(string inputPlugValues)
        {
            SetInputPlugValues(inputPlugValues);
            bool updatePerformed = true;

            int cycles = 0;
            while (cycles < 1000000)
            {
                // Update the values of the input plugs.
                updatePerformed = UpdateInputPlugValues();

                if (!updatePerformed)
                {
                    break;
                }

                // Update the values of the output plugs.
                UpdateOutputPlugValues();

                cycles++;
            }

            return cycles + " " + GetOutputPlugValues();
        }
    }
}
