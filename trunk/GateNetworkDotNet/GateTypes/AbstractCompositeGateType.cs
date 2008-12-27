using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    public abstract class AbstractCompositeGateType
        : AbstractGateType
    {
        #region Public static fields

        /// <summary>
        /// The names of the implicit input plugs.
        /// </summary>
        public static string[] implicitInputPlugNames = new string[] {"0","1"};

        #endregion // Public static fields

        #region Private instance fields

        /// <summary>
        /// The gates comprising the composite gate type.
        /// </summary>
        private Dictionary<string, AbstractGateType> nestedGateTypes;

        /// <summary>
        /// The connections within the composite gate type.
        /// </summary>
        private Dictionary<string, string> connections;

        /// <summary>
        /// The phase of construction.
        /// </summary>
        private AbstractCompositeGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, AbstractGateType> NestedGateTypes
        {
            get
            {
                return nestedGateTypes;
            }
        }

        /// <summary>
        /// Gets the number of the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The number of the nested gates.
        /// </value>
        public int NestedGateCount
        {
            get
            {
                return nestedGateTypes.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Connections
        {
            get
            {
                return connections;
            }
        }

        /// <summary>
        /// The number of the connections.
        /// </summary>
        /// 
        /// <value>
        /// The number of the connections.
        /// </value>
        public int ConnectionCount
        {
            get
            {
                return connections.Count;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == AbstractCompositeGateTypeConstructionPhase.END);
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// 
        /// </summary>
        protected AbstractCompositeGateType()
        {
            nestedGateTypes = new Dictionary<string, AbstractGateType>();
            connections = new Dictionary<string, string>();

            constructionPhase = AbstractCompositeGateTypeConstructionPhase.NAME;
        }

        #endregion // Protected instance constructors

        #region Public insatnce methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="gateTypes"></param>
        public override void Configure(string line, Dictionary<string, AbstractGateType> gateTypes)
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

        /// <summary>
        /// Parses a line for a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The nested gate.
        /// </returns>
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

        /// <summary>
        /// Parses a line for a connection.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The connection.
        /// </returns>
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

        /// <summary>
        /// Sets the name of the composite gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate type.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        public override void SetName(string name)
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.NAME)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetName(name);

            // Advance the phase of construction.
            constructionPhase = AbstractCompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>inputPlugNames</c> is <c>null</c>.
        /// </exception>
        public override void SetInputPlugNames(string[] inputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES)
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
            constructionPhase = AbstractCompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>outputPlugNames</c> is <c>null</c>.
        /// </exception>
        public override void SetOutputPlugNames(string[] outputPlugNames)
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception("Missing keyword.");
            }

            base.SetOutputPlugNames(outputPlugNames);

            // Advance the phase of construction.
            constructionPhase = AbstractCompositeGateTypeConstructionPhase.NESTED_GATES;
        }

        /// <summary>
        /// Adds a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The nested gate.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>nestedGate</c> is <c>null</c>.
        /// </exception>
        public void AddNestedGate(string[] nestedGate, Dictionary<string, AbstractGateType> gateTypes)
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.NESTED_GATES && constructionPhase != AbstractCompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the nested gate string.
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
            AbstractGateType nestedGateType;
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
            constructionPhase = AbstractCompositeGateTypeConstructionPhase.CONNECTIONS;
        }

        /// <summary>
        /// Adds a connection.
        /// </summary>
        /// 
        /// <param name="connection">The connection.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>connection</c> is <c>null</c>.
        /// </exception>
        public void AddConnection(string[] connection)
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.CONNECTIONS)
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
            
            // The endpoint of the connection.
            string connectionTo = connection[0];

            if (connections.ContainsKey(connectionTo))
            {
                throw new Exception("Duplicate (" + connection + ").");
            }

            // The startpoint of the connection.
            string connectionFrom = connection[1];

            // Store the connection.
            connections.Add(connectionTo, connectionFrom);
        }

        /// <summary>
        /// 
        /// </summary>
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


        /// <summary>
        /// Ends the construction process.
        /// </summary>
        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != AbstractCompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception("Missing keyword.");
            }

            // Validate the connections.
            ValidateConnections();

            // Advance the phase of construciton.
            constructionPhase = AbstractCompositeGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the composite gate object.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate object.</param>
        /// 
        /// <returns>
        /// The composite gate object.
        /// </returns>
        public override AbstractGate Instantiate(string name)
        {
            return new CompositeGate(name, this);
        }

        #endregion // Public instance methods
    }

    enum AbstractCompositeGateTypeConstructionPhase
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
