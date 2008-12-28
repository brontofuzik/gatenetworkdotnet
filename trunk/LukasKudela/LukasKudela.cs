using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet
{
    /// <summary>
    /// An entry point of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// An entry point of the application.
        /// </summary>
        /// 
        /// <param name="args">The command line arguments.</param>
        public static void Main( string[] args )
        {
            StreamReader streamReader = null;

            try
            {
                //
                // 1. Read the network configuration file.
                //

                // Open the network configuration file.
                if (args.Length != 1)
                {
                    throw new Exception( "Usage: GateNetworkDotNet.exe NetworkConfigurationFile" );
                }
                streamReader = new StreamReader( args[ 0 ] );

                // The dictionary of (known) gate types.
                Dictionary< string, GateType > gateTypes = new Dictionary< string, GateType >();

                // Loop invariant:
                // "line" contains the most recently read line.
                // "lineNumber" is the number of already read lines.
                string line;
                int lineNumber = 0;
                try
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lineNumber++;

                        // If the line can be ignored, skip it.
                        if (IsIgnorableLine( line ))
                        {
                            continue;
                        }

                        GateType gateType = GateType.ParseGateType( line );
                        if (gateTypes.ContainsKey( gateType.Name ))
                        {
                            throw new Exception( "Duplicate (" + gateType.Name + ")." );
                        }

                        // TODO: Handle EOF eventuality.
                        while (!gateType.IsConstructed && ((line = streamReader.ReadLine()) != null))
                        {
                            lineNumber++;

                            // If the line can be ignored, skip it.
                            if (IsIgnorableLine( line ))
                            {
                                continue;
                            }

                            gateType.Configure( line, gateTypes );
                        }

                        if (gateType.IsConstructed)
                        {
                            gateTypes.Add( gateType.Name, gateType );
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new MyException( lineNumber, e.Message );
                }

                //
                // 2. Construct the network.
                //


                NetworkGateType networkGateType;
                try
                {
                    networkGateType = (NetworkGateType)gateTypes[ "networkGateType" ];
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception( "Missing keyword (network)" );
                }
                CompositeGate networkGate = (CompositeGate)networkGateType.Instantiate( "networkGate" );
                networkGate.Initialize();

                //
                // 3. Use the network.
                //

                while (true)
                {
                    line = Console.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (line.Equals( "end" ))
                    {
                        break;
                    }

                    try
                    {
                        Console.WriteLine( networkGate.Evaluate( line ) );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine( e.Message );
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message );
            }
            finally
            {
                // Close the network configuration file.
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
        }

        /// <summary>
        /// Determines whether a line can be ignored.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// <c>True</c> if the line can be ignored, <c>false</c> otherwise.
        /// </returns>
        public static bool IsIgnorableLine( string line )
        {
            // A line can be ignored if and only if it
            // * is empty,
            // * contains only whitespace characters,
            // * begins with a semicolon.
            string ignorableLinePattern = @"^(|\s*|;.*)$";
            Regex ignorableLineRegex = new Regex( ignorableLinePattern );

            return ignorableLineRegex.IsMatch( line );
        }

        /// <summary>
        /// Determines whether a name is legal.
        /// </summary>
        /// 
        /// <param name="name">The name (of a gate type, gate instance, gate input or output plug, etc.).</param>
        /// 
        /// <returns>
        /// <c>True</c> if the name is legal, <c>false</c> otherwise.
        /// </returns>
        public static bool IsLegalName( string name )
        {
            // If the name is of zero length, it is not legal.
            if (name.Length == 0)
            {
                return false;
            }

            // If the name contains a whitespace character, it is not legal.
            if ((name.IndexOf( ' ' ) != -1) || (name.IndexOf( '\t' ) != -1) || (name.IndexOf( '\v' ) != -1) ||
                (name.IndexOf( '\n' ) != -1) || (name.IndexOf( '\r' ) != -1) || (name.IndexOf( '\f' ) != -1))
            {
                return false;
            }

            // If the name contains any of the following characters, it is not legal.
            if ((name.IndexOf( '.' ) != -1) || (name.IndexOf( ';' ) != -1))
            {
                return false;
            }

            // If the name contains any of the following words, it is not legal.
            if (name.Contains( "->" ))
            {
                return false;
            }

            // If the name starts with the word "end", it is not legal.
            if (name.StartsWith( "end" ))
            {
                return false;
            }

            return true;
        }
    }
}

namespace GateNetworkDotNet.Exceptions
{
    /// <summary>
    /// My exception.
    /// </summary>
    public class MyException
        : Exception
    {
        #region Private instance fields

        /// <summary>
        /// The number of the line where the exception has been thrown.
        /// </summary>
        private readonly int lineNumber;

        /// <summary>
        /// The message of the exception.
        /// </summary>
        private readonly string message;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the message of the exception.
        /// </summary>
        /// 
        /// <value>
        /// The message of the exception.
        /// </value>
        public override string Message
        {
            get
            {
                return "Line " + lineNumber + ": " + message;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// 
        /// <param name="lineNumber">The number of the line where the exception has been thrown.</param>
        /// <param name="message">The message of the exception.</param>
        public MyException( int lineNumber, string message )
        {
            this.lineNumber = lineNumber;
            this.message = message;
        }

        #endregion // Public instance constructors
    }
}

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
                    outputPlugValuesSB.Append( "?" + " " );
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

            string outputPlugValues = type.Evaluate( inputPlugValues );

            SetOutputPlugValues( outputPlugValues );
        }

        #endregion // Public instance methods
    }
    
    /// <summary>
    /// A composite gate.
    /// </summary>
    public class CompositeGate
        : Gate
    {
        #region Private instance fields

        /// <summary>
        /// The type of the composite gate.
        /// </summary>
        private CompositeGateType type;

        /// <summary>
        /// The nested gates.
        /// </summary>
        private Dictionary< string, Gate > nestedGates;

        /// <summary>
        /// The connections;
        /// </summary>
        private Connection[] connections; 

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The nested gates.
        /// </value>
        public Dictionary< string, Gate > NestedGates
        {
            get
            {
                return nestedGates;
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
                return type.NestedGateCount;
            }
        }

        /// <summary>
        /// Gets the connections
        /// </summary>
        /// 
        /// <value>
        /// The connections.
        /// </value>
        public Connection[] Connections
        {
            get
            {
                return connections;
            }
        }

        /// <summary>
        /// Gets the number of connections.
        /// </summary>
        /// 
        /// <value>
        /// The number of connections.
        /// </value>
        public int ConnectionCount
        {
            get
            {
                return type.ConnectionCount;
            }
        }

        #endregion // Public instance properties

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
        public CompositeGate( string name, CompositeGateType type )
            : base( name, type )
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
            nestedGates = new Dictionary< string, Gate >();
            foreach (KeyValuePair< string, GateType > kvp in type.NestedGateTypes)
            {
                string nestedGateName = kvp.Key;
                GateType nestedGateType = kvp.Value;
                Gate nestedGate = nestedGateType.Instantiate( nestedGateName );

                nestedGates.Add( nestedGateName, nestedGate ); 
            }

            //
            // Construct the connections.
            //
            connections = new Connection[ ConnectionCount ];
            int connectionIndex = 0;
            foreach (KeyValuePair< string, string > kvp in type.Connections)
            {
                //
                // Get the target plug.
                //
                string connectionTarget = kvp.Key;
                string[] targetPlugName = connectionTarget.Split( '.' );
                bool nestedTargetPlug = (targetPlugName.Length != 1);
                
                // Depending on whether the target plug is a nested plug, the target gate is ...
                Gate targetGate = nestedTargetPlug ?
                    // ... a nested gate.
                    nestedGates[ targetPlugName[ 0 ] ] :
                    // ... this composite gate.
                    this;
                
                // Depending on whether the target plug is a nested plug, the target plug is ...
                Plug targetPlug = nestedTargetPlug ?
                    // ... an input plug of a nested gate.
                    targetGate.GetInputPlugByName( targetPlugName[ 1 ] ) :
                    // ... an output plug of this composite gate.
                    targetGate.GetOutputPlugByName( targetPlugName[ 0 ] );
                    
                //
                // Get the source plug.
                //
                string connectionSource = kvp.Value;
                string[] sourcePlugName = connectionSource.Split( '.' );
                bool nestedSourcePlug = (sourcePlugName.Length != 1);

                // Depending on whether the source plug is a nested plug, the source gate is ...
                Gate sourceGate = nestedSourcePlug ?
                    // ... a nested gate.
                    nestedGates[ sourcePlugName[ 0 ] ] :
                    // ... this composite gate.
                    this;

                // Depending on whether the source plug is a nested plug, the source plug is ...
                Plug sourcePlug = nestedSourcePlug ?
                    // ... an output plug of a nested gate.
                    sourceGate.GetOutputPlugByName( sourcePlugName[ 1 ] ) :
                    // ... an input plug of this composite gate.
                    sourceGate.GetInputPlugByName( sourcePlugName[ 0 ] );

                // Construct the connection.
                Connection connection = new Connection();
                sourcePlug.PlugTargetConnection( connection );
                targetPlug.PlugSourceConnection( connection );

                connections[ connectionIndex++ ] = connection;
            }

            //
            //
            //
            GetInputPlugByName( "0" ).Value = "0";
            GetInputPlugByName( "1" ).Value = "1";
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValuesString">The values of the input plugs.</param>
        public override void SetInputPlugValues( string inputPlugValuesString )
        {
            string[] inputPlugValues = inputPlugValuesString.Split( ' ' );
            if (inputPlugValues.Length != InputPlugCount - 2)
            {
                throw new Exception( "Syntax error." );
            }

            for (int i = 0; i < InputPlugCount - 2; i++)
            {
                InputPlugs[ i ].Value = inputPlugValues[ i ];
            }
        }

        /// <summary>
        /// Initializes the composite gate.
        /// </summary>
        public override void Initialize()
        {
            foreach (KeyValuePair< string, Gate > kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.Initialize();
            }
        }

        /// <summary>
        /// Updates the values of the input plugs of the composite gate.
        /// </summary>
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

        /// <summary>
        /// Evaluates the composite gate.
        /// </summary>
        public override void UpdateOutputPlugValues()
        {
            foreach (KeyValuePair< string, Gate > kvp in nestedGates)
            {
                Gate nestedGate = kvp.Value;
                nestedGate.UpdateOutputPlugValues();
            }
            foreach (Plug outputPlug in OutputPlugs)
            {
                outputPlug.UpdatePlugValue();
            }
        }

        /// <summary>
        /// Evaluates the (abstract) gate.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        /// 
        /// <returns>
        /// The computation time (in cycles) and the values of the output plugs.
        /// </returns>
        public string Evaluate( string inputPlugValues )
        {
            SetInputPlugValues( inputPlugValues );
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

        #endregion // Public instance methods
    }


    /// <summary>
    /// A connection.
    /// </summary>
    public class Connection
    {
        #region Private instance fields

        /// <summary>
        /// The source plug.
        /// </summary>
        private Plug sourcePlug;

        /// <summary>
        /// The target plug.
        /// </summary>
        private Plug targetPlug;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets or sets the source plug.
        /// </summary>
        /// 
        /// <value>
        /// The source plug.
        /// </value>
        public Plug SourcePlug
        {
            get
            {
                return sourcePlug;
            }
            set
            {
                sourcePlug = value;
            }
        }

        /// <summary>
        /// Gets or sets the target plug.
        /// </summary>
        /// 
        /// <value>
        /// The target plug.
        /// </value>
        public Plug TargetPlug
        {
            get
            {
                return targetPlug;
            }
            set
            {
                targetPlug = value;
            }
        }

        #endregion // Public instance properties
    }

    /// <summary>
    /// An abstract gate.
    /// </summary>
    public abstract class Gate
    {
        #region Private instance fields

        /// <summary>
        /// The name of the (abstract) gate.
        /// </summary>
        private string name;

        /// <summary>
        /// The type of the (abstract) gate.
        /// </summary>
        private GateType type;

        /// <summary>
        /// The input plugs.
        /// </summary>
        private Plug[] inputPlugs;

        /// <summary>
        /// The output plugs.
        /// </summary>
        private Plug[] outputPlugs;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the name of the gate.
        /// </summary>
        /// 
        /// <value>
        /// The name of the gate type.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The input plugs.
        /// </value>
        public Plug[] InputPlugs
        {
            get
            {
                return inputPlugs;
            }
        }

        /// <summary>
        /// Gets the number of the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the input plugs.
        /// </value>
        public int InputPlugCount
        {
            get
            {
                return type.InputPlugCount;
            }
        }

        /// <summary>
        /// Gets the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The output plugs.
        /// </value>
        public Plug[] OutputPlugs
        {
            get
            {
                return outputPlugs;
            }
        }

        /// <summary>
        /// Gets the number of the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the output plugs.
        /// </value>
        public int OutputPlugCount
        {
            get
            {
                return type.OutputPlugCount;
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate.</param>
        /// <param name="type">The type of the gate.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>name</c> is <c>null</c>.
        /// Condition 2: <c>type</c> is <c>null</c>.
        /// </exception>
        protected Gate( string name, GateType type )
        {
            // Validate the name.
            if (!Program.IsLegalName( name ))
            {
                throw new ArgumentException( name );
            }
            this.name = name;

            // Validate the (abstract) gate type.
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            this.type = type;

            // Create the input plugs.
            inputPlugs = new Plug[ InputPlugCount ];
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugs[ i ] = new Plug( this );
            }

            // Create the output plugs.
            outputPlugs = new Plug[ OutputPlugCount ];
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugs[ i ] = new Plug( this );
            }
        }

        #endregion // Protected instance constructors

        #region Public instance methods

        /// <summary>
        /// Gets an input plug specified by its name.
        /// </summary>
        /// 
        /// <param name="inputPlugName">The name of the input plug.</param>
        /// 
        /// <returns>
        /// The input plug (or <c>null</c> if such input plug does not exist).
        /// </returns>
        public Plug GetInputPlugByName( string inputPlugName )
        {
            int inputPlugIndex = type.GetInputPlugIndex( inputPlugName );
            return (inputPlugIndex != -1) ? InputPlugs[ inputPlugIndex ] : null;
        }

        /// <summary>
        /// Gets an output plug specified by its name.
        /// </summary>
        /// 
        /// <param name="outputPlugName">The name of the output plug.</param>
        /// 
        /// <returns>
        /// The output plug (or <c>null</c> if such output plug does not exist).
        /// </returns>
        public Plug GetOutputPlugByName( string outputPlugName )
        {
            int outputPlugIndex = type.GetOutputPlugIndex( outputPlugName );
            return (outputPlugIndex != -1) ? OutputPlugs[ outputPlugIndex ] : null;
        }

        /// <summary>
        /// Gets the values of the input plugs.
        /// </summary>
        /// 
        /// <returns>
        /// The values of the input plugs.
        /// </returns>
        public string GetInputPlugValues()
        {
            // Build the string representation of the values of the input plugs.
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append( InputPlugs[ i ].Value + " " );
            }
            // Remove the trailing space character if necessary.
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove( inputPlugValuesSB.Length - 1, 1 );
            }
            return inputPlugValuesSB.ToString();
        }

        /// <summary>
        /// Sets the values of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugValuesString">The values of the input plugs.</param>
        public abstract void SetInputPlugValues( string inputPlugValuesString );

        /// <summary>
        /// Gets the values of the output plugs.
        /// </summary>
        /// 
        /// <returns>
        /// The values of the output plugs.
        /// </returns>
        public string GetOutputPlugValues()
        {
            // Build the string representation of the values of the output plugs.
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < OutputPlugCount; i++)
            {
                outputPlugValuesSB.Append( OutputPlugs[ i ].Value + " " );
            }
            // Remove the trailing space character if necessary.
            if (outputPlugValuesSB.Length != 0)
            {
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
            }
            return outputPlugValuesSB.ToString();
        }

        /// <summary>
        /// Sets the values of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugValues">The values of the output plugs.</param>
        public void SetOutputPlugValues( string outputPlugValuesString )
        {
            string[] outputPlugValues = outputPlugValuesString.Split( ' ' );
            for (int i = 0; i < OutputPlugCount; i++)
            {
                OutputPlugs[ i ].Value = outputPlugValues[ i ];
            }
        }

        /// <summary>
        /// Initializes the (abstract) gate.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Updates the values of the input plugs of the (abstract) gate.
        /// </summary>
        public abstract bool UpdateInputPlugValues();

        /// <summary>
        /// Evaluates the (abstract) gate.
        /// </summary>
        public abstract void UpdateOutputPlugValues();

        #endregion // Public instance methods
    }

    /// <summary>
    /// A plug.
    /// </summary>
    public class Plug
    {
        #region Private insatance fields

        /// <summary>
        /// The value of the plug.
        /// </summary>
        private string value;

        /// <summary>
        /// The input connection plugged into the plug.
        /// </summary>
        private Connection sourceConnection;

        /// <summary>
        /// The output connections plugged into the plug.
        /// </summary>
        private List< Connection > targetConnections;

        /// <summary>
        /// The parent gate of the plug.
        /// </summary>
        private Gate parentGate;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets or sets the value of the plug.
        /// </summary>
        /// 
        /// <value>
        /// The value of the plug.
        /// </value>
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!IsLegalPlugValue( value ))
                {
                    throw new Exception( "Syntax error." );
                }
                this.value = value;
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors
        
        /// <summary>
        /// Creates a new plug.
        /// </summary>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>parentGate</c> is <c>null</c>.
        /// </exception>
        public Plug( Gate parentGate )
        {
            value = "?";

            sourceConnection = null;
            targetConnections = new List< Connection >();

            if (parentGate == null)
            {
                throw new ArgumentNullException();
            }
            this.parentGate = parentGate;
        }

        #endregion // Public instance constructors

        #region Public static methods

        /// <summary>
        /// Determines whether a value of a (input or output) plug is a legal.
        /// </summary>
        /// 
        /// <param name="plugValue">The value of a (input or output) plug.</param>
        /// 
        /// <returns>
        /// <c>True</c> if the value of a plug is legal, <c>false</c> otherwise.
        /// </returns>
        public static bool IsLegalPlugValue( string plugValue )
        {
            return (plugValue.Equals( "0" ) || plugValue.Equals( "1" ) || plugValue.Equals( "?" ));
        }

        #endregion // Public static methods

        #region Public instance methods

        /// <summary>
        /// Plugs a source connection into the plug.
        /// </summary>
        /// 
        /// <param name="sourceConnection">The source connection.</param>
        public void PlugSourceConnection( Connection sourceConnection )
        {
            if (this.sourceConnection != null)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }
            this.sourceConnection = sourceConnection;
            
            sourceConnection.TargetPlug = this;
        }

        /// <summary>
        /// Plugs a target connecion into the plug.
        /// </summary>
        /// 
        /// <param name="targetConnection">The target connection.</param>
        public void PlugTargetConnection( Connection targetConnection )
        {
            targetConnections.Add( targetConnection );
            
            targetConnection.SourcePlug = this;
        }

        /// <summary>
        /// Updates the value of the (input or output) plug.
        /// </summary>
        public bool UpdatePlugValue()
        {
            if (sourceConnection != null)
            {
                if (value != sourceConnection.SourcePlug.Value)
                {
                    value = sourceConnection.SourcePlug.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion // Public instance methods
    }
}

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// The type of an (abstract) gate.
    /// </summary>
    public abstract class GateType
    {
        #region Private instance fields

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The names of the input plugs.
        /// </summary>
        private string[] inputPlugNames;

        /// <summary>
        /// The names of the output plugs.
        /// </summary>
        private string[] outputPlugNames;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// 
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the names of the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The names of the input plugs.
        /// </value>
        public string[] InputPlugNames
        {
            get
            {
                return inputPlugNames;
            }
        }

        /// <summary>
        /// The number of the input plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the input plugs.
        /// </value>
        public int InputPlugCount
        {
            get
            {
                return inputPlugNames.Length;
            }
        }

        /// <summary>
        /// Gets the names of the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The names of the output plugs.
        /// </value>
        public string[] OutputPlugNames
        {
            get
            {
                return outputPlugNames;
            }
        }

        /// <summary>
        /// Gets the number of the output plugs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the output plugs.
        /// </value>
        public int OutputPlugCount
        {
            get
            {
                return outputPlugNames.Length;
            }
        }

        /// <summary>
        /// Determines whether the type of an (abstract) gate is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the type of an (abstract) gate is constructed, <c>false</c> otherwise.
        public abstract bool IsConstructed
        {
            get;
        }

        #endregion // Public instance properties

        #region Public static methods

        /// <summary>
        /// Parses a line for a type of a gate.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The type of the gate.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>line</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition 1: <c>line</c> contains one word and it is not "network".
        /// Condition 2: <c>line</c> contains two words and the first one is neither "gate" nor "composite".
        /// Condition 3: <c>line</c> contains more than two words.
        /// </exception>
        public static GateType ParseGateType( string line )
        {
            // Validate the line.
            if (line == null)
            {
                throw new ArgumentNullException( line );
            }

            GateType gateType;

            string[] gateTypeClassAndName = line.Split( ' ' );
            switch (gateTypeClassAndName.Length)
            {
                case 1:

                    // Network construction.
                    if (gateTypeClassAndName[ 0 ].Equals( "network" ))
                    {
                        gateType = new NetworkGateType();
                    }
                    else
                    {
                        throw new Exception( "Syntax error." );
                    }
                    gateType.SetName( "networkGateType" );
                    break;

                case 2:

                    // Basic or composite gate construction.
                    if (gateTypeClassAndName[ 0 ].Equals( "gate" ))
                    {
                        // Basic gate construction.
                        gateType = new BasicGateType();
                    }
                    else if (gateTypeClassAndName[ 0 ].Equals( "composite" ))
                    {
                        // Composite gate construction.
                        gateType = new CompositeGateType();
                    }
                    else
                    {
                        // Syntax error.
                        throw new Exception( "Syntax error." );
                    }
                    gateType.SetName( gateTypeClassAndName[ 1 ] );
                    break;

                default:

                    // Syntax error.
                    throw new Exception( "Syntax error." );
            }
            return gateType;
        }

        #endregion // Public static methods

        #region Public instance mehods

        /// <summary>
        /// Configures the type of an (abstract) gate.
        /// </summary>
        /// 
        /// <param name="line">The line (from the configuration file).</param>
        /// <param name="gateTypes">The (already defined) types of gates.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>line</c> contains an unknown keyword.
        /// </exception>
        public virtual void Configure( string line, Dictionary< string, GateType > gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ))
            {
                // Set the names of the input plugs.
                string[] inputPlugNames = ParsePlugNames( line );
                SetInputPlugNames( inputPlugNames );
            }
            else if (keyword.Equals( "outputs" ))
            {
                // Set the names of the output plugs.
                string[] outputPlugNames = ParsePlugNames( line );
                SetOutputPlugNames( outputPlugNames );
            }
            else if (keyword.Equals( "end" ))
            {
                // End the construction process.
                EndConstruction();
            }
            else
            {
                throw new Exception( "Syntax error." );
            }
        }

        /// <summary>
        /// Parses a line for a keyword.
        /// Keyword is the first word in a line, i.e. the sequence of characters before the first space character,
        /// or the entire line if the line contains no space character.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The keyword.
        /// </returns>
        protected string ParseKeyword( string line )
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );

            // Return only the first word.
            return words[ 0 ];
        }


        /// <summary>
        /// Parse a line for names of (input or output) plugs.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The names of the (input or output) plugs.
        /// </returns>
        private string[] ParsePlugNames( string line )
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );

            // Return all the words except for the first one.
            string[] plugNames = new string[ words.Length - 1 ];
            for (int i = 0; i < plugNames.Length; i++)
            {
                plugNames[ i ] = words[ i + 1 ];
            }
            return plugNames;
        }

        /// <summary>
        /// Sets the name of the (abstract) type of a gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) type of a gate.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition: <c>name</c> is not a legal name.
        /// </exception>
        protected virtual void SetName( string name )
        {
            // Validate the name.
            if (name == null)
            {
                throw new ArgumentNullException( "name" );
            }
            if (!Program.IsLegalName( name ))
            {
                throw new Exception( "Syntax error (" + name + ")." );
            }

            this.name = name;
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
        /// <exception cref="System.Exception">
        /// Condition 1: <c>inputPlugNames</c> contains an illegal name of an input plug.
        /// Condition 2: <c>inputPlugNames</c> contains a duplcit name of an input plug.
        /// </exception>
        public virtual void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the names of the input plugs.
            if (inputPlugNames == null)
            {
                throw new ArgumentNullException( "inputPlugNames" );
            }
            StringCollection inputPlugNamesCollection = new StringCollection();
            foreach (string inputPlugName in inputPlugNames)
            {
                if (!Program.IsLegalName( inputPlugName ))
                {
                    throw new Exception( "Syntax error (" + inputPlugName + ")." );
                }
                if (inputPlugNamesCollection.Contains( inputPlugName ))
                {
                    throw new Exception( "Duplicate (" + inputPlugName + ")." );
                }
                inputPlugNamesCollection.Add( inputPlugName );
            }

            this.inputPlugNames = inputPlugNames;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>outputputPlugNames</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition 1: <c>outputPlugNames</c> contains no names of output plugs.
        /// Condition 2: <c>outputPlugNames</c> contains an illegal name of an output plug.
        /// Condition 3: <c>outputPlugNames</c> contains a duplcit name of an output plug.
        /// </exception>
        public virtual void SetOutputPlugNames( string[] outputPlugNames )
        {
            // Validate the names of the output plugs.
            if (outputPlugNames == null)
            {
                throw new ArgumentNullException( "outputPlugNames" );
            }
            if (outputPlugNames.Length == 0)
            {
                throw new Exception( "Syntax error." );
            }
            StringCollection outputPlugNamesCollection = new StringCollection();
            foreach (string outputPlugName in outputPlugNames)
            {
                if (!Program.IsLegalName( outputPlugName ))
                {
                    throw new Exception( "Syntax error (" + outputPlugName + ")." );
                }
                if (outputPlugNamesCollection.Contains( outputPlugName ))
                {
                    throw new Exception( "Duplicate (" + outputPlugName + ")." );
                }
                outputPlugNamesCollection.Add( outputPlugName );
            }

            this.outputPlugNames = outputPlugNames;
        }

        /// <summary>
        /// Ends the construction process.
        /// </summary>
        public abstract void EndConstruction();

        /// <summary>
        /// Instantiates the type of an (abstract) gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) gate.</param>
        /// 
        /// <returns>
        /// The (abstract) gate.
        /// </returns>
        public abstract Gate Instantiate( string name );

        /// <summary>
        /// Gets the index of an input plug specified by its name.
        /// </summary>
        /// 
        /// <param name="inputPlugName">The name of the input plug.</param>
        /// 
        /// <returns>
        /// The index of the input plug.
        /// </returns>
        public int GetInputPlugIndex( string inputPlugName )
        {
            for (int i = 0; i < InputPlugCount; i++)
            {
                if (inputPlugNames[ i ].Equals( inputPlugName ))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of an output plug specified by its name.
        /// </summary>
        /// 
        /// <param name="outputPlugName">The name of the output plug.</param>
        /// 
        /// <returns>
        /// The index of the output plug.
        /// </returns>
        public int GetOutputPlugIndex( string outputPlugName )
        {
            for (int i = 0; i < OutputPlugCount; i++)
            {
                if (outputPlugNames[ i ].Equals( outputPlugName ))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion // Public instance methods       
    }
    
    /// <summary>
    /// The type of a basic gate.
    /// </summary>
    public class BasicGateType
        : GateType
    {
        #region Private instance fields

        /// <summary>
        /// The transitions.
        /// </summary>
        private Dictionary< string, string > transitions;

        /// <summary>
        /// The phase of construction.
        /// </summary>
        private BasicGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the transitions.
        /// </summary>
        /// 
        /// <value>
        /// The transitions.
        /// </value>
        public Dictionary< string, string > Transitions
        {
            get
            {
                return transitions;
            }
        }

        /// <summary>
        /// Gets the number of transitions.
        /// </summary>
        /// 
        /// <value>
        /// The number of transitions.
        /// </value>
        public int TransitionCount
        {
            get
            {
                return transitions.Count;
            }
        }

        /// <summary>
        /// Gets the length of a transition.
        /// </summary>
        /// 
        /// <value>
        /// The length of a transiiton.
        /// </value>
        public int TransitionLength
        {
            get
            {
                return InputPlugCount + OutputPlugCount;
            }
        }

        /// <summary>
        /// Determines whether the type of a basic gate is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the type of a basic gate is constructed, <c>false</c> otherwise.
        /// </value>
        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == BasicGateTypeConstructionPhase.END);
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new type of a basic gate.
        /// </summary>
        public BasicGateType()
        {
            transitions = new Dictionary< string, string >();

            constructionPhase = BasicGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public instance methods

        /// <summary>
        /// Configures the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="line">The line (from the configuration file).</param>
        /// <param name="gateTypes">The (already defined) types of gates.</param>
        public override void Configure( string line, Dictionary< string, GateType > gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ) || keyword.Equals( "outputs" ) || keyword.Equals( "end" ))
            {
                base.Configure( line, gateTypes );
            }
            else
            {
                // Set the transitions.
                string[] transition = ParseTransition( line );
                AddTransition( transition );
            }
        }

        /// <summary>
        /// Parses a line for a transition.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The transition.
        /// </returns>
        public string[] ParseTransition( string line)
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );
            
            // Return all the words.
            return words;
        }

        /// <summary>
        /// Sets the name of the basic type of a gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic type of a gate.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        protected override void SetName( string name )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.NAME)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetName( name );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetInputPlugNames( inputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetOutputPlugNames( string[] outputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetOutputPlugNames( outputPlugNames );

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.TRANSITIONS;
        }

        /// <summary>
        /// Adds a transition to the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="transition">The transition.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>transition</c> is null.
        /// </exception>
        /// 
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>transition</c> contains a transition of incorrect length.
        /// Condition 3: <c>transition</c> contains an illegal value of an (input or output) plug.
        /// Condition 4: <c>transition</c> contains a transiiton that has already been added to the transitions.
        /// </exception>
        public void AddTransition( string[] transition )
        {
            // Validate the phase of construciton.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the transition.
            if (transition == null)
            {
                throw new ArgumentNullException( "transition" );
            }
            if (transition.Length != TransitionLength)
            {
                throw new Exception( "Syntax error (" + transition + ")." );
            }
            foreach (string transitionValue in transition)
            {
                if (!Plug.IsLegalPlugValue( transitionValue ))
                {
                    throw new Exception( "Syntax error (" + transition + ")." );
                }
            }

            //
            // Build the string containing the values of the input strings.
            //
            StringBuilder inputPlugValuesSB = new StringBuilder();
            for (int i = 0; i < InputPlugCount; i++)
            {
                inputPlugValuesSB.Append( transition[ i ] + " " );
            }
            if (inputPlugValuesSB.Length != 0)
            {
                inputPlugValuesSB.Remove( inputPlugValuesSB.Length - 1, 1 );
            }
            string inputPlugValues = inputPlugValuesSB.ToString();

            if (transitions.ContainsKey( inputPlugValues ))
            {
                throw new Exception( "Duplicate (" + transition + ")." );
            }

            //
            // Build the string containing the values of the output strings.
            //
            StringBuilder outputPlugValuesSB = new StringBuilder();
            for (int i = InputPlugCount; i < TransitionLength; i++)
            {
                outputPlugValuesSB.Append( transition[ i ] + " " );
            }
            if (outputPlugValuesSB.Length != 0)
            {
                outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
            }
            string outputPlugValues = outputPlugValuesSB.ToString();

            // Add the (inputPlugValues, outputPlugValues) key-value-pair into the transitions.
            transitions.Add( inputPlugValues, outputPlugValues );
        }

        /// <summary>
        /// Ends the construction process.
        /// </summary>
        /// 
        /// <exception cref="Syste.Exception">
        /// Condition: The method has been called in the wrong phase of construction.
        /// </exception>
        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != BasicGateTypeConstructionPhase.TRANSITIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Advance the phase of construction.
            constructionPhase = BasicGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the basic gate.</param>
        /// 
        /// <returns>
        /// The basic gate (as an (abstract) gate).
        /// </returns>
        public override Gate Instantiate( string name )
        {
            return new BasicGate( name, this );
        }

        /// <summary>
        /// Evaluates the transiiotn function of the type of a basic gate.
        /// </summary>
        /// 
        /// <param name="inputPlugValues">The values of the input plugs.</param>
        /// 
        /// <returns>
        /// The values of the output plugs.
        /// </returns>
        public string Evaluate( string inputPlugValues )
        {
            // TODO: Think about replacing the following piece of code with TryGetMethod.
            string outputPlugValues;
            try
            {
                // The transition function contains the mapping for the inputs.
                outputPlugValues = transitions[ inputPlugValues ];
            }
            catch (KeyNotFoundException)
            {
                // The transition fucntion does not contain the mapping for the inputs, hence implicit mapping is used.
                string outputPlugValue = inputPlugValues.Contains( "?" ) ? "?" : "0";
                
                StringBuilder outputPlugValuesSB = new StringBuilder();
                for (int i = 0; i < OutputPlugCount; i++)
                {
                    outputPlugValuesSB.Append( outputPlugValue + " " );
                }
                if (outputPlugValuesSB.Length != 0)
                {
                    outputPlugValuesSB.Remove( outputPlugValuesSB.Length - 1, 1 );
                }
                outputPlugValues = outputPlugValuesSB.ToString();
            }

            // Return the values of the output plugs.
            return outputPlugValues;
        }
        #endregion // Public instance methods
    }

    /// <summary>
    /// The phase of construction of a type of a basic gate.
    /// </summary>
    enum BasicGateTypeConstructionPhase
    {
        BEGINNING,
        NAME,
        INPUT_PLUG_NAMES,
        OUTPUT_PLUG_NAMES,
        TRANSITIONS,
        END
    }
    
    /// <summary>
    /// A type of a composite gate.
    /// </summary>
    public class CompositeGateType
        : GateType
    {
        #region Private static fields

        /// <summary>
        /// The names of the implicit input plugs.
        /// </summary>
        private static string[] implicitInputPlugNames = new string[] {"0","1"};

        #endregion // Private static fields

        #region Private instance fields

        /// <summary>
        /// The nested gates.
        /// </summary>
        private Dictionary< string, GateType > nestedGateTypes;

        /// <summary>
        /// The connections.
        /// </summary>
        private Dictionary< string, string > connections;

        /// <summary>
        /// The phase of construction.
        /// </summary>
        private CompositeGateTypeConstructionPhase constructionPhase;

        #endregion // Private instance fields

        #region Public instance properties

        /// <summary>
        /// Gets the nested gates.
        /// </summary>
        /// 
        /// <value>
        /// The nested gates.
        /// </value>
        public Dictionary< string, GateType > NestedGateTypes
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
        /// Gets the connections.
        /// </summary>
        /// 
        /// <value>
        /// The connections.
        /// </value>
        public Dictionary< string, string > Connections
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
        /// Determines whether the type of a composite gate is constructed.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the type of a composite gate is constructed, <c>false</c> otherwise.
        /// </value>
        public override bool IsConstructed
        {
            get
            {
                return (constructionPhase == CompositeGateTypeConstructionPhase.END);
            }
        }

        #endregion // Public instance properties

        #region Public instance constructors

        /// <summary>
        /// Creates a new type of a composite gate.
        /// </summary>
        public CompositeGateType()
        {
            nestedGateTypes = new Dictionary< string, GateType >();
            connections = new Dictionary< string, string >();

            constructionPhase = CompositeGateTypeConstructionPhase.NAME;
        }

        #endregion // Public instance constructors

        #region Public insatnce methods

        /// <summary>
        /// Configures the type of a composite gate.
        /// </summary>
        /// 
        /// <param name="line">The line (from the configuration file).</param>
        /// <param name="gateTypes">The (already defined) types of gates.</param>
        public override void Configure( string line, Dictionary< string, GateType > gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals( "inputs" ) || keyword.Equals( "outputs" ) || keyword.Equals( "end" ))
            {
                base.Configure( line, gateTypes );
            }
            else if (keyword.Equals( "gate" ))
            {
                // Set the nested gates.
                string[] nestedGate = ParseNestedGate( line );
                AddNestedGate( nestedGate, gateTypes );
            }
            else
            {
                // Set the connections.
                string[] connection = ParseConnection( line );
                AddConnection( connection );
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
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>line</c> contains an illegal definition of a nested gate.
        /// </exception>
        public string[] ParseNestedGate( string line )
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );

            // Validate the number of words (3).
            if (words.Length != 3)
            {
                throw new Exception( "Syntax error." );
            }

            // Return only the second and the third word.
            string[] nestedGate = new string[ 2 ];
            for (int i = 0; i < 2; i++)
            {
                nestedGate[ i ] = words[ i + 1 ];
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
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>line</c> contains an illegal definition of a connection.
        /// </exception>
        public string[] ParseConnection( string line )
        {
            // Split the line into words.
            string[] words = Regex.Split( line, "->" );

            // Validate the number of words (2).
            if (words.Length != 2)
            {
                throw new Exception( "Syntax error." );
            }

            // Return all the words.
            return words;
        }

        /// <summary>
        /// Sets the name of the composite type of a gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite type of a gate.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        protected override void SetName( string name )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NAME)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetName( name );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.INPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            string[] extendedInputPlugNames = new string[ inputPlugNames.Length + implicitInputPlugNames.Length ];
            for (int i = 0; i < inputPlugNames.Length; i++)
            {
                extendedInputPlugNames[ i ] = inputPlugNames[ i ];
            }
            for (int i = 0; i < implicitInputPlugNames.Length; i++)
            {
                extendedInputPlugNames[ inputPlugNames.Length + i ] = implicitInputPlugNames[ i ];
            }
            base.SetInputPlugNames( extendedInputPlugNames );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES;
        }

        /// <summary>
        /// Sets the names of the output plugs.
        /// </summary>
        /// 
        /// <param name="outputPlugNames">The names of the output plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: The method is called in the wrong phase of construction.
        /// </exception>
        public override void SetOutputPlugNames( string[] outputPlugNames )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.OUTPUT_PLUG_NAMES)
            {
                throw new Exception( "Missing keyword." );
            }

            base.SetOutputPlugNames( outputPlugNames );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.NESTED_GATES;
        }

        /// <summary>
        /// Adds a nested gate.
        /// </summary>
        /// 
        /// <param name="line">The nested gate.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>nestedGate</c> is <c>null</c>.
        /// Condition 2: <c>gateTypes</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>nestedGate</c> contains an illegaly defined nested gate.
        /// Condition 3: <c>nestedGate</c> contains an illegal name of a nested gate.
        /// Condition 4: <c>nestedGate</c> contains a duplicit name of a nested gate.
        /// Condition 5: <c>nestedGate</c> contains an undefined type of a nested gate.
        /// </exception>      
        public void AddNestedGate( string[] nestedGate, Dictionary< string, GateType > gateTypes )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.NESTED_GATES && constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the nested gate.
            if (nestedGate == null)
            {
                throw new ArgumentNullException( "nestedGate" );
            }
            if (nestedGate.Length != 2)
            {
                throw new Exception( "Syntax error (" + nestedGate + ")." );
            }

            // Validate the types of gates.
            if (gateTypes == null)
            {
                throw new ArgumentNullException( "gateTypes" );
            }

            //
            // Validate the name of the nested gate.
            //
            string nestedGateName = nestedGate[ 0 ];

            // Validate the legality of the name of the nested gate.
            if (!Program.IsLegalName( nestedGateName ))
            {
                throw new Exception( "Syntax error (" + nestedGateName + ")." );
            }
            // Validate the uniqueness of the name of the nested gate.
            if (nestedGateTypes.ContainsKey( nestedGateName ))
            {
                throw new Exception( "Duplicate (" + nestedGateName + ")." );
            }

            //
            // Validate the type of the nested gate.
            //
            string nestedGateTypeString = nestedGate[ 1 ];

            // Retrieve the type of the nested gate.
            GateType nestedGateType;
            try
            {
                nestedGateType = gateTypes[ nestedGateTypeString ];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception( "Syntax error (" + nestedGateTypeString + ")." );
            }

            // Store the nested gate.
            nestedGateTypes.Add( nestedGateName, nestedGateType );

            // Advance the phase of construction.
            constructionPhase = CompositeGateTypeConstructionPhase.CONNECTIONS;
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
        /// <exception cref="System.Exception">
        /// Condition 1: The method has been called in the wrong phase of construction.
        /// Condition 2: <c>connection</c> contains an illegally defined connection.
        /// Condition 3: <c>connection</c> contains a duplicit endpoint of a connection.
        /// </exception>
        public void AddConnection( string[] connection )
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the connection.
            if (connection == null)
            {
                throw new ArgumentNullException( "connection" );
            }
            if (connection.Length != 2)
            {
                throw new Exception( "Syntax error (" + connection + ")." );
            }
            
            //
            // Validate the endpoint of the connection.
            //
            string connectionTo = connection[ 0 ];

            if (connections.ContainsKey( connectionTo ))
            {
                throw new Exception( "Duplicate (" + connection + ")." );
            }

            //
            // Validate the startpoint of the connection.
            //
            string connectionFrom = connection[ 1 ];

            // Store the connection.
            connections.Add( connectionTo, connectionFrom );
        }

        /// <summary>
        /// Validates the connections.
        /// </summary>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: An output plug of a type of a composite gate remained unconnected.
        /// </exception>
        public virtual void ValidateConnections()
        {
            foreach (string outputPlugName in OutputPlugNames)
            {
                if (!connections.ContainsKey( outputPlugName ))
                {
                    throw new Exception( "Binding rule broken" );
                }
            }
        }


        /// <summary>
        /// Ends the construction process.
        /// </summary>
        /// 
        /// <exception cref="Syste.Exception">
        /// Condition: The method has been called in the wrong phase of construction.
        /// </exception>
        public override void EndConstruction()
        {
            // Validate the phase of construction.
            if (constructionPhase != CompositeGateTypeConstructionPhase.CONNECTIONS)
            {
                throw new Exception( "Missing keyword." );
            }

            // Validate the connections.
            ValidateConnections();

            // Advance the phase of construciton.
            constructionPhase = CompositeGateTypeConstructionPhase.END;
        }

        /// <summary>
        /// Instantiates the type of a composite gate.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate.</param>
        /// 
        /// <returns>
        /// The composite gate (as an (abstract) gate).
        /// </returns>
        public override Gate Instantiate( string name)
        {
            return new CompositeGate( name, this );
        }

        #endregion // Public instance methods
    }

    /// <summary>
    /// The phase of construction of a type of a composite gate.
    /// </summary>
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
    
    /// <summary>
    /// A type of a network gate.
    /// </summary>
    public class NetworkGateType
        : CompositeGateType
    {
        #region Public instance methods

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: <c>inputPlugNames</c> contains no explicitly defined input plug.
        /// </exception>
        public override void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the names of the input plugs.
            if (inputPlugNames.Length == 0)
            {
                throw new Exception( "Syntax error (Network has to have at least one input)." );
            }

            base.SetInputPlugNames( inputPlugNames );
        }

        /// <summary>
        /// Validates the connections.
        /// </summary>
        /// 
        /// <exception cref="System.Exception">
        /// Condition: An (explicitly defined) input plug of a type of a composite gate remained unconnected.
        /// </exception>
        public override void ValidateConnections()
        {
            base.ValidateConnections();

            foreach (string inputPlugName in InputPlugNames)
            {
                if (inputPlugName.Equals( "0" ) || inputPlugName.Equals( "1" ))
                {
                    continue;
                }
                if (!Connections.ContainsValue( inputPlugName ))
                {
                    throw new Exception( "Binding rule broken" );
                }
            }
        }

        #endregion // Public instance methods
    }
}