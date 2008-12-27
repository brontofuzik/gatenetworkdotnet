using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// An abstract gate type.
    /// </summary>
    public abstract class AbstractGateType
    {
        #region Private instance fields

        /// <summary>
        /// The name of the gate type.
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
        /// Gets the name of the gate type.
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
        /// Determines whther the (abstract) gate is constructed or not.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if the (abstract) gate is constructed, <c>false</c> otherwise.
        /// </value>
        public abstract bool IsConstructed
        {
            get;
        }

        #endregion // Public instance properties

        #region Public static methods

        /// <summary>
        /// Parses a line for a gate type.
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
        public static AbstractGateType ParseGateType(string line)
        {
            // Validate the line.
            if (line == null)
            {
                throw new ArgumentNullException(line);
            }

            AbstractGateType gateType;

            string[] gateTypeClassAndName = line.Split(' ');
            switch (gateTypeClassAndName.Length)
            {
                case 1:

                    // Network construction.
                    if (gateTypeClassAndName[0].Equals("network"))
                    {
                        gateType = new NetworkGateType();
                    }
                    else
                    {
                        throw new Exception("Syntax error.");
                    }
                    gateType.SetName("network");
                    break;

                case 2:

                    // Basic or composite gate construction.
                    if (gateTypeClassAndName[0].Equals("gate"))
                    {
                        // Basic gate construction.
                        gateType = new BasicGateType();
                    }
                    else if (gateTypeClassAndName[0].Equals("composite"))
                    {
                        // Composite gate construction.
                        gateType = new CompositeGateType();
                    }
                    else
                    {
                        // Syntax error.
                        throw new Exception("Syntax error.");
                    }
                    gateType.SetName(gateTypeClassAndName[1]);
                    break;

                default:

                    // Syntax error.
                    throw new Exception("Syntax error.");
                    break;
            }
            return gateType;
        }

        #endregion // Public static methods

        #region Public instance mehods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="gateTypes"></param>
        public virtual void Configure( string line, Dictionary< string, AbstractGateType > gateTypes )
        {
            string keyword = ParseKeyword( line );

            if (keyword.Equals("inputs"))
            {
                // Set the names of the input plugs.
                string[] inputPlugNames = ParsePlugNames(line);
                SetInputPlugNames(inputPlugNames);
            }
            else if (keyword.Equals("outputs"))
            {
                // Set the names of the output plugs.
                string[] outputPlugNames = ParsePlugNames(line);
                SetOutputPlugNames(outputPlugNames);
            }
            else if (keyword.Equals("end"))
            {
                // End the construction process.
                EndConstruction();
            }
            else
            {
                throw new Exception("Syntax error.");
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
        public string ParseKeyword(string line)
        {
            // Split the line into words.
            string[] words = line.Split(' ');

            // Return only the first word.
            return words[0];
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
        public string[] ParsePlugNames(string line)
        {
            // Split the line into words.
            string[] words = line.Split(' ');

            // Return all the words except for the first one.
            string[] plugNames = new string[words.Length - 1];
            for (int i = 0; i < plugNames.Length; i++)
            {
                plugNames[i] = words[i + 1];
            }
            return plugNames;
        }

        /// <summary>
        /// Sets the name of the (abstract) gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) gate type.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>name</c> is <c>null</c>.
        /// </exception>
        public virtual void SetName( string name )
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
        /// Condition: <c>outputPlugNames</c> is <c>null</c>.
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
                throw new Exception("Syntax error.");
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

        public abstract void EndConstruction();

        /// <summary>
        /// Instantiates the (abstract) gate object.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) gate object.</param>
        /// 
        /// <returns>
        /// The (abstract) gate object.
        /// </returns>
        public abstract AbstractGate Instantiate( string name );

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
