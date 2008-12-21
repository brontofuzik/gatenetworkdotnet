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
                // The dictionary of (known) gate types.
                Dictionary< string, GateType > gateTypes = new Dictionary< string, GateType >();

                // Open the network configuration file.
                streamReader = new StreamReader( args[ 0 ] );

                // Loop invariant:
                // "line" contains the most recently read line.
                // "lineNumber" is the number of already read lines.
                string line;
                int lineNumber = 0;
                while (true)
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

                    if (keyword.Equals( "gate" ))
                    {
                        // ======================================
                        // The construction of a basic gate type.
                        // ======================================
                        
                        string basicGateName = ParseName( line );

                        // Check for basic gate type redefinition.
                        if (gateTypes.ContainsKey( basicGateName ))
                        {
                            throw new MyException( lineNumber, "Duplicate" );
                        }

                        BasicGateType basicGateType = new BasicGateType( basicGateName );

                        // TODO: Handle EOF eventuality.
                        while (!basicGateType.IsConstructed)
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

                            keyword = ParseKeyword( line );

                            if (keyword.Equals( "inputs" ))
                            {
                                string inputPlugNames = ParsePlugNames( line );
                                basicGateType.SetInputPlugNames( inputPlugNames );
                            }
                            else if (keyword.Equals( "outputs" ))
                            {
                                string outputPlugNames = ParsePlugNames( line );
                                basicGateType.SetOutputPlugNames( outputPlugNames );
                            }
                            else if (keyword.Equals( "end" ))
                            {
                                basicGateType.EndConstruction();
                            }
                            else
                            {
                                string transition = ParseTransition( line );
                                basicGateType.AddTransition( transition );
                            }
                         }
                         gateTypes.Add( basicGateName, basicGateType );
                    }
                    else if (keyword.Equals( "composite" ))
                    {
                        // ==========================================
                        // The construction of a composite gate type.
                        // ==========================================

                        string compositeGateName = ParseName( line );

                        // Check for composite gate type redefinition.
                        if (gateTypes.ContainsKey( compositeGateName ))
                        {
                            throw new MyException( lineNumber, "Duplicate" );
                        }

                        CompositeGateType compositeGateType = new CompositeGateType( compositeGateName );

                        // TODO: Handle EOF eventuality.
                        while (!compositeGateType.IsConstructed)
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

                            keyword = ParseKeyword( line );

                            if (keyword.Equals( "inputs" ))
                            {
                                string inputPlugNames = ParsePlugNames( line );
                                compositeGateType.SetInputPlugNames( inputPlugNames );
                            }
                            else if (keyword.Equals( "outputs" ))
                            {
                                string outputPlugNames = ParsePlugNames( line );
                                compositeGateType.SetOutputPlugNames( outputPlugNames );
                            }
                            else if (keyword.Equals( "gate" ))
                            {
                                string nestedGate = ParseNestedGate( line );
                                compositeGateType.AddNestedGate( nestedGate, gateTypes );
                            }
                            else if (keyword.Equals("end"))
                            {
                                compositeGateType.EndConstruction();
                            }
                            else
                            {
                                string connection = ParseConnection( line );
                                compositeGateType.AddConnection( connection );
                            }
                        }
                        gateTypes.Add( compositeGateName, compositeGateType);
                    }
                    else if (keyword.Equals( "network" ))
                    {
                        //
                        // The network construction.
                        //
                    }
                    else
                    {
                        throw new MyException( lineNumber, "Syntax error" );
                    }
                }
            }
            catch (MyException e)
            {
                Console.WriteLine( e.Message );
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
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
            return line.Substring( line.IndexOf( ' ' ) );
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
            string illegalIdentifierPattern = @"^(|end.*)$";
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
