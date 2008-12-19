using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.GateTypes;
using GateNetworkDotNet.GateTypes.ConstructionPhases;

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

                    // The end of the network configuration file has been reached.
                    if (line == null)
                    {
                        break;
                    }

                    // If the line can be ignored, ignore it, and continue with reading the next line.
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

                        BasicGateTypeConstructionPhase basicGateTypeConstructionPhase = BasicGateTypeConstructionPhase.NAME;
                        string basicGateName = ParseName( line );
                        if (gateTypes.ContainsKey( basicGateName ))
                        {
                            throw new MyException( lineNumber, "Duplicate" );
                        }

                        BasicGateType basicGateType = new BasicGateType( basicGateName );
                        basicGateTypeConstructionPhase = BasicGateTypeConstructionPhase.INPUTS;

                        // TODO: Handle EOF eventuality.
                        while (basicGateTypeConstructionPhase != BasicGateTypeConstructionPhase.END)
                        {
                            line = streamReader.ReadLine();
                            lineNumber++;

                            // The end of the network configuration file has been reached.
                            if (line == null)
                            {
                                break;
                            }

                            // If the line can be ignored, ignore it, and continue with reading the next line.
                            if (IsIgnorableLine( line ))
                            {
                                continue;
                            }

                            //line = line.Trim();

                            keyword = ParseKeyword( line );

                            switch (basicGateTypeConstructionPhase)
                            {
                                case BasicGateTypeConstructionPhase.INPUTS:

                                    if (!keyword.Equals( "inputs" ))
                                    {
                                        throw new MyException( lineNumber, "TODO" );
                                    }
                                    string[] inputPlugNames = ParsePlugNames( line );
                                    basicGateType.SetInputPlugNames( inputPlugNames );
                                    basicGateTypeConstructionPhase = BasicGateTypeConstructionPhase.OUTPUTS;
                                    break;

                                case BasicGateTypeConstructionPhase.OUTPUTS:

                                    if (!keyword.Equals( "outputs" ))
                                    {
                                        throw new MyException( lineNumber, "TODO" );
                                    }
                                    string[] outputPlugNames = ParsePlugNames( line );
                                    basicGateType.SetOutputPlugNames( outputPlugNames );
                                    basicGateTypeConstructionPhase = BasicGateTypeConstructionPhase.TRANSITIONS;
                                    break;

                                case BasicGateTypeConstructionPhase.TRANSITIONS:

                                    if (line.Equals( "end" ))
                                    {
                                        basicGateTypeConstructionPhase = BasicGateTypeConstructionPhase.END;
                                    }
                                    basicGateType.AddTransition( line );
                                    break;

                                default:

                                    break;
                              }
                         }
                         gateTypes.Add( basicGateName, basicGateType );
                    }
                    else if (keyword.Equals( "composite" ))
                    {
                        // ==========================================
                        // The construction of a composite gate type.
                        // ==========================================

                        CompositeGateTypeConstructionPhase compositeGateTypeConstructionPhase = CompositeGateTypeConstructionPhase.NAME;
                        string compositeGateName = ParseName( line );
                        if (gateTypes.ContainsKey( compositeGateName ))
                        {
                            throw new MyException( lineNumber, "Duplicate" );
                        }

                        CompositeGateType compositeGateType = new CompositeGateType( compositeGateName );
                        compositeGateTypeConstructionPhase = CompositeGateTypeConstructionPhase.INPUTS;

                        // TODO: Handle EOF eventuality.
                        while (compositeGateTypeConstructionPhase != CompositeGateTypeConstructionPhase.END)
                        {
                            line = streamReader.ReadLine();
                            lineNumber++;

                            // The end of the network configuration file has been reached.
                            if (line == null)
                            {
                                break;
                            }

                            // If the line can be ignored, ignore it, and continue with reading the next line.
                            if (IsIgnorableLine( line ))
                            {
                                continue;
                            }

                            //line = line.Trim();

                            keyword = ParseKeyword( line );

                            switch (compositeGateTypeConstructionPhase)
                            {
                                case CompositeGateTypeConstructionPhase.INPUTS:

                                    if (!keyword.Equals( "inputs" ))
                                    {
                                        throw new MyException( lineNumber, "TODO" );
                                    }
                                    string[] inputPlugNames = ParsePlugNames( line );
                                    compositeGateType.SetInputPlugNames( inputPlugNames );
                                    compositeGateTypeConstructionPhase = CompositeGateTypeConstructionPhase.OUTPUTS;
                                    break;

                                case CompositeGateTypeConstructionPhase.OUTPUTS:

                                    if (!keyword.Equals( "outputs" ))
                                    {
                                        throw new MyException( lineNumber, "TODO" );
                                    }
                                    string[] outputPlugNames = ParsePlugNames( line );
                                    compositeGateType.SetOutputPlugNames( outputPlugNames );
                                    compositeGateTypeConstructionPhase = CompositeGateTypeConstructionPhase.GATES;
                                    break;

                                case CompositeGateTypeConstructionPhase.GATES:

                                    if (!keyword.Equals( "gate" ))
                                    {
                                        throw new MyException( lineNumber, "TODO" );
                                    }

                                    compositeGateType.AddNestedGate( line );
                                    break;

                                case CompositeGateTypeConstructionPhase.CONNECTIONS:

                                    if (line.Equals( "end" ))
                                    {
                                        compositeGateTypeConstructionPhase = CompositeGateTypeConstructionPhase.END;
                                    }
                                    compositeGateType.AddConnection( line );
                                    break;

                                default:

                                    break;
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
            return line.Substring( 0, line.IndexOf( ' ' ) );
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
        public static string ParseName(string line)
        {
            return line.Substring( line.IndexOf( ' ' ) );
        }

        /// <summary>
        /// parses the line for names of (input or outout) plugs.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// The names of the (input or output) plugs.
        /// </returns>
        public static string[] ParsePlugNames( string line )
        {
            string plugNamesStr = line.Substring( line.IndexOf( ' ' ) );
            return plugNamesStr.Split( ' ' );
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
        public static bool IsLegalIdentifier(string identifier)
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
