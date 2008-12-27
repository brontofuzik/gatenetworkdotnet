using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

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
}
