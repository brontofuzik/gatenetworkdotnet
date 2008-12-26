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
    public abstract class GateType
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
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: <c>line</c> 
        /// </exception>
        public static GateType ParseGateType(string line)
        {
            // Validate the line.
            if (line == null)
            {
                throw new ArgumentNullException(line);
            }

            GateType gateType;
            string[] gateTypeClassAndName = line.Split(' ');
            switch (gateTypeClassAndName.Length)
            {
                case 1:

                    // Network construction.
                    if (gateTypeClassAndName[0].Equals("network"))
                    {
                        gateType = new CompositeGateType();
                    }
                    else
                    {
                        throw new MyException("Syntax error.");
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
                        throw new MyException("Syntax error.");
                    }
                    gateType.SetName(gateTypeClassAndName[1]);
                    break;

                default:

                    // Syntax error.
                    throw new MyException("Syntax error.");
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
        public abstract void Configure( string line, Dictionary< string, GateType > gateTypes );

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
            return (line.IndexOf(' ') != -1) ? line.Substring(0, line.IndexOf(' ')) : line;
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
        public string ParsePlugNames(string line)
        {
            return (line.IndexOf(' ') != -1) ? line.Substring(line.IndexOf(' ') + 1) : "";
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
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition: <c>name</c> is not a legal identifier.
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
                throw new MyException( "Syntax error (" + name + ")." );
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
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: <c>inputPlugNames</c> contains an illegal input plug name.
        /// Condition 2: <c>inputPlugNames</c> contains duplicit input plug names.
        /// </exception>
        public virtual void SetInputPlugNames( string inputPlugNamesString )
        {
            // Validate the names of the input plugs.
            if (inputPlugNamesString == null)
            {
                throw new ArgumentNullException( "inputPlugNames" );
            }

            if (inputPlugNamesString.Length != 0)
            {
                // One or more input plug names.
                StringCollection inputPlugNamesCollection = new StringCollection();
                inputPlugNames = inputPlugNamesString.Split( ' ' );
                foreach (string inputPlugName in inputPlugNames)
                {
                    if (!Program.IsLegalName( inputPlugName ))
                    {
                        throw new MyException( "Syntax error (" + inputPlugName + ")." );
                    }
                    if (inputPlugNamesCollection.Contains( inputPlugName ))
                    {
                        throw new MyException( "Duplicate (" + inputPlugName + ")." );
                    }
                    inputPlugNamesCollection.Add( inputPlugName );
                }
            }
            else
            {
                // No input plug names.
                inputPlugNames = new string[ 0 ];
            }
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
        /// <exception cref="GateNetworkDotNet.Exceptions.MyException">
        /// Condition 1: <c>outputPlugNames</c> contains an illegal output plug name.
        /// Condition 2: <c>outputPlugNames</c> contains duplicit output plug names.
        /// Condition 3: <c>outputPlugNames</c> contains less than one output plug name.
        /// </exception>
        public virtual void SetOutputPlugNames( string outputPlugNamesString )
        {
            // Validate the names of the output plugs.
            if (outputPlugNamesString == null)
            {
                throw new ArgumentNullException( "outputPlugNames" );
            }

            if (outputPlugNamesString.Length != 0)
            {
                // One or more output plug names.
                StringCollection outputPlugNamesCollection = new StringCollection();
                outputPlugNames = outputPlugNamesString.Split( ' ' );
                foreach (string outputPlugName in outputPlugNames)
                {
                    if (!Program.IsLegalName( outputPlugName ))
                    {
                        throw new MyException( "Syntax error (" + outputPlugName + ")." );
                    }
                    if (outputPlugNamesCollection.Contains( outputPlugName ))
                    {
                        throw new MyException( "Duplicate (" + outputPlugName + ")." );
                    }
                    outputPlugNamesCollection.Add( outputPlugName );
                }
            }
            else
            {
                // No output plug names.
                throw new MyException( "Syntax error." );
            }
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
