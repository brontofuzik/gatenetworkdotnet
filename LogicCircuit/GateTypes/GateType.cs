using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using LogicCircuit.Gates;

namespace LogicCircuit.GateTypes
{
    public abstract class GateType
    {
        private string name;

        private string[] inputPlugNames;

        private string[] outputPlugNames;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string[] InputPlugNames
        {
            get
            {
                return inputPlugNames;
            }
        }

        public int InputPlugCount
        {
            get
            {
                return inputPlugNames.Length;
            }
        }

        public string[] OutputPlugNames
        {
            get
            {
                return outputPlugNames;
            }
        }

        public int OutputPlugCount
        {
            get
            {
                return outputPlugNames.Length;
            }
        }

        public abstract bool IsConstructed
        {
            get;
        }

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

        protected string ParseKeyword( string line )
        {
            // Split the line into words.
            string[] words = line.Split( ' ' );

            // Return only the first word.
            return words[ 0 ];
        }

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

        public abstract void EndConstruction();

        public abstract Gate Instantiate( string name );

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
    }
}
