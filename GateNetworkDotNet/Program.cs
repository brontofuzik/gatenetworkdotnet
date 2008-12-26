using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.GateTypes;

namespace GateNetworkDotNet
{
    class Program
    {
        static void Main( string[] args )
        {
            StreamReader streamReader = null;
            
            try
            {
                // Open the network configuration file.
                streamReader = new StreamReader( args[ 0 ] );

                // The dictionary of (known) gate types.
                Dictionary< string, GateType > gateTypes = new Dictionary< string, GateType >();

                // Loop invariant:
                // "line" contains the most recently read line.
                // "lineNumber" is the number of already read lines.
                string line;
                int lineNumber = 0;
                while (true)
                {
                    try
                    {
                        line = streamReader.ReadLine();
                        lineNumber++;
                        if (line == null)
                        {
                            break;
                        }
                        if (IsIgnorableLine( line ))
                        {
                            continue;
                        }
                        //line = line.Trim();

                        string gateTypeClass = ParseKeyword( line );

                        GateType gateType;
                        if (gateTypeClass.Equals( "gate" ))
                        {
                            gateType = new BasicGateType();
                        }
                        else if (gateTypeClass.Equals( "composite" ) || gateTypeClass.Equals( "network" ))
                        {
                            gateType = new CompositeGateType();
                        }
                        else
                        {
                            throw new MyException( "Syntax error (" + gateTypeClass + ")." );
                        }

                        // Set the name of the gate type.
                        string gateTypeName = ParseName( line );
                        if (gateTypes.ContainsKey( gateTypeName ))
                        {
                            throw new MyException( "Duplicate (" + gateTypeName + ")." );
                        }
                        gateType.SetName( gateTypeName );

                        // TODO: Handle EOF eventuality.
                        while (!gateType.IsConstructed)
                        {
                            line = streamReader.ReadLine();
                            lineNumber++;
                            if (line == null)
                            {
                                break;
                            }
                            if (IsIgnorableLine( line ))
                            {
                                continue;
                            }
                            //line = line.Trim();

                            string keyword = ParseKeyword( line );

                            if (keyword.Equals( "inputs" ))
                            {
                                // Set the names of the input plugs.
                                string inputPlugNames = ParsePlugNames( line );
                                gateType.SetInputPlugNames( inputPlugNames );
                            }
                            else if (keyword.Equals( "outputs" ))
                            {
                                // Set the names of the output plugs.
                                string outputPlugNames = ParsePlugNames( line );
                                gateType.SetOutputPlugNames( outputPlugNames );
                            }
                            else if (keyword.Equals( "gate" ))
                            {
                                // Set the nested gates.
                                string nestedGate = ParseNestedGate( line );
                                (gateType as CompositeGateType).AddNestedGate( nestedGate, gateTypes );
                            }
                            else if (keyword.Equals( "end" ))
                            {
                                // End the construction process.
                                gateType.EndConstruction();
                            }
                            else
                            {
                                if (gateTypeClass.Equals( "gate" ))
                                {
                                    // Set the transitions.
                                    string transition = ParseTransition( line );
                                    (gateType as BasicGateType).AddTransition( transition );
                                }
                                else
                                {
                                    // Set the connections.
                                    string connection = ParseConnection( line );
                                    (gateType as CompositeGateType).AddConnection( connection );
                                }
                            }
                        }

                        gateTypes.Add( gateTypeName, gateType );
                    }
                    catch (MyException e)
                    {
                        throw new MyException( lineNumber, e.Message );
                    }
                }

                // Network.
                CompositeGate network = (CompositeGate)gateTypes["network"].Instantiate("network");
                network.Initialize();
                Console.WriteLine(network.Evaluate("1"));
                Console.WriteLine(network.Evaluate("1"));
            }
            catch (MyException e)
            {
                Console.WriteLine( e.Message );
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine( e.Message );
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine( e.Message );
            }
            catch (IOException e)
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
        /// Parses the line for a keyword.
        /// Keyword must be the first word of a line.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The keyword.
        /// </returns>
        public static string ParseKeyword( string line )
        {
            return (line.IndexOf( ' ' ) != -1) ? line.Substring( 0, line.IndexOf( ' ' ) ) : line;
        }

        /// <summary>
        /// Parse the line for a name of a (basic or composite) gate.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        ///
        /// <returns>
        /// The name of the (basic or composite) gate.
        /// </returns>
        public static string ParseName( string line )
        {
            if (line.IndexOf( ' ' ) == -1)
            {
                throw new MyException( "Syntax error." );
            }
            return line.Substring( line.IndexOf( ' ' ) + 1 );
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
        public static string ParsePlugNames( string line )
        {
            return (line.IndexOf( ' ' ) != -1) ? line.Substring( line.IndexOf( ' ' ) + 1 ) : "";
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
        public static string ParseTransition( string line )
        {
            return line;
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
        public static string ParseNestedGate( string line )
        {
            return line.Substring( line.IndexOf( ' ' ) + 1 ); 
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
        public static string ParseConnection( string line )
        {
            return line;
        }

        /// <summary>
        /// Determines whether an identifier is legal.
        /// </summary>
        /// 
        /// <param name="name">The identifier (i.e. name of a gate type, gate instance, gate input or output, network).</param>
        /// 
        /// <returns>
        /// <c>True</c> if the identifier is legal, <c>false</c> otherwise.
        /// </returns>
        public static bool IsLegalIdentifier( string identifier )
        {
            // TODO: Provide implementation.
            string legalIdentifierPattern = @"^(.*)$";
            //string illegalIdentifierPattern = @"^(|end.*)$";
            Regex legalIdentifierRegex = new Regex(legalIdentifierPattern);

            return legalIdentifierRegex.IsMatch(identifier);
        }

        /// <summary>
        /// Determines whether a line is ignorable.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// <c>True</c> if the line is ignorable, <c>false</c> otherwise.
        /// </returns>
        public static bool IsIgnorableLine(string line)
        {
            // TODO: Is a line beginning with some whitespace characters and a semicolon ignorable?
            string ignorableLinePattern = @"^(|\s*|;.*)$";
            Regex ignorableLineRegex = new Regex(ignorableLinePattern);

            return ignorableLineRegex.IsMatch(line);
        }
    }
}
