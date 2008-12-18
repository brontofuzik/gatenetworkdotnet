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
                Dictionary<string, GateType> gateTypes = new Dictionary<string, GateType>();

                // Open the network configuration file.
                streamReader = new StreamReader(args[0]);

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

                    string keyword = line.Substring( 0, line.IndexOf(' ') );

                    if (keyword.Equals( "gate" ))
                    {
                        //
                        // The basic gate type definition.
                        //
                        string basicGateName = line.Substring( line.IndexOf( ' ' ) );
                        if (gateTypes.ContainsKey( basicGateName ))
                        {
                            throw new MyException( lineNumber, "Duplicate" );
                        }
                        try
                        {
                            BasicGateType basicGateType = new BasicGateType( basicGateName );
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

                                string[] inputPlugNames;
                                basicGateType.InputPlugNames = inputPlugNames;

                                string[] outputPlugNames;
                                basicGateType.OutputPlugNames = outputPlugNames;

                                string[] transitions;
                                foreach (string transition in transitions)
                                {
                                    basicGateType.AddTransition( transition );
                                }
                            }
                            gateTypes.Add( basicGateName, basicGateType );
                        }
                        catch (ArgumentException e)
                        {
                            throw new MyException( lineNumber, "Illegal identifier (" + e.ParamName + ")" );
                        }
                    }
                    else if (keyword.Equals( "composite" ))
                    {
                        //
                        // The composite gate type definition.
                        //
                    }
                    else if (keyword.Equals( "network" ))
                    {
                        //
                        // The network definition.
                        //
                    }
                    else
                    {
                        throw new MyException( lineNumber, "Syntax error" );
                    }
                }

                // 
                // Basic gate types.
                //

                string name;
                string inputPlugNames;
                string outputPlugNames;
                List<string> transitions;
                GateType gateType;

                // Basic gate type: "and".
                name = "and";
                inputPlugNames = "i0 i1";
                outputPlugNames = "o";
                transitions = new List<string>(new string[] { "1 1 1" });
                gateType = new BasicGateType(name, inputPlugNames, outputPlugNames, transitions);
                gateTypes.Add(name, gateType);

                // basic gate type: "or".
                name = "or";
                inputPlugNames = "i0 i1";
                outputPlugNames = "o";
                transitions = new List<string>(new string[] { "1 1 1", "0 1 1", "1 0 1" });
                gateType = new BasicGateType(name, inputPlugNames, outputPlugNames, transitions);
                gateTypes.Add(name, gateType);

                // Basic gate type: "not".
                name = "not";
                inputPlugNames = "i";
                outputPlugNames = "o";
                transitions = new List<string>(new string[] { "0 1" });
                gateType = new BasicGateType(name, inputPlugNames, outputPlugNames, transitions);
                gateTypes.Add(name, gateType);

                //
                // Composite gate types.
                //

                List<string> nestedGates;
                List<string> connections;

                //// Composite gate type: "nand".
                name = "nand";
                inputPlugNames = "i0 i1";
                outputPlugNames = "o";
                nestedGates = new List<string>(new string[] { "a and", "n not" });
                connections = new List<string>(new string[] { "a.i0->i0", "a.i1->i1", "n.i->a.o", "o->n.o" });
                gateType = new CompositeGateType(name, inputPlugNames, outputPlugNames, nestedGates, connections, gateTypes);
                gateTypes.Add(name, gateType);

                // Composite gate type "and_wrapper".
                name = "and_wrapper";
                inputPlugNames = "i0 i1";
                outputPlugNames = "o";
                nestedGates = new List<string>(new string[] { "a and" });
                connections = new List<string>(new string[] { "a.i0->i0", "a.i1->i1", "o->a.o" });
                gateType = new CompositeGateType(name, inputPlugNames, outputPlugNames, nestedGates, connections, gateTypes);
                gateTypes.Add(name, gateType);

                //
                // Network type.
                //

                // Network type "network".
                name = "network";
                inputPlugNames = "a b";
                outputPlugNames = "a&b a|b !a&b";
                nestedGates = new List<string>(new string[] { "a1 and", "o1 or", "n1 nand" });
                connections = new List<string>(new string[] { "a1.i0->a", "a1.i1->b", "o1.i0->a", "o1.i1->b", "n1.i0->a", "n1.i1->b", "a&b->a1.o", "a|b->o1.o", "!a&b->n1.o" });
                gateType = new CompositeGateType(name, inputPlugNames, outputPlugNames, nestedGates, connections, gateTypes);
                gateTypes.Add(name, gateType);

                // The dictionary of all the gates.
                Dictionary<string, Gate> gates = new Dictionary<string, Gate>();

                //
                // Basic gates.
                //

                string type;
                Gate gate;

                // Basic gate "a and".
                name = "a";
                type = "and";
                gateType = gateTypes[type];
                gate = gateType.Instantiate(name);
                gate.InputPlugs[0].Value = "1";
                gate.InputPlugs[1].Value = "1";
                gate.Evaluate();
                //Console.WriteLine( gate.OutputPlugs[ 0 ].Value );

                // Basic gate "n not".
                name = "n";
                type = "not";
                gateType = gateTypes[type];
                gate = gateType.Instantiate(name);
                gate.InputPlugs[0].Value = "1";
                gate.Evaluate();
                //Console.WriteLine( gate.OutputPlugs[ 0 ].Value );

                // Basic gate "o or".
                name = "o";
                type = "or";
                gateType = gateTypes[type];
                gate = gateType.Instantiate(name);
                gate.InputPlugs[0].Value = "1";
                gate.InputPlugs[1].Value = "1";
                gate.Evaluate();
                //Console.WriteLine( gate.OutputPlugs[ 0 ].Value );

                //
                // Composite gates.
                //

                // Composite gate "na nand".
                name = "na";
                type = "nand";
                gateType = gateTypes[type];
                gate = gateType.Instantiate(name);
                //gate.InputPlugs[ 0 ].Value = "1";
                //gate.InputPlugs[ 1 ].Value = "1";
                //gate.Evaluate();
                //Console.WriteLine( gate.OutputPlugs[ 0 ].Value );

                //// Composite gate "aw and_wrapper".
                //name = "aw";
                //type = "and_wrapper";
                //gateType = gateTypes[ type ];
                //gate = gateType.Instantiate( name );
                //gate.InputPlugs[ 0 ].Value = "1";
                //gate.InputPlugs[ 1 ].Value = "0";
                //gate.Evaluate();
                //Console.WriteLine( gate.OutputPlugs[ 0 ].Value );

                //
                // Network.
                //

                // Network "n network".
                name = "n";
                type = "network";
                gateType = gateTypes[type];
                gate = gateType.Instantiate(name);
                gate.Initialize();

                gate.InputPlugValues = "1 1";

                int cycle = 0;
                while (true)
                {
                    Console.WriteLine(cycle + " " + gate.OutputPlugValues);
                    gate.Evaluate();
                    cycle++;
                    Console.ReadKey();
                }
            }
            catch (MyException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
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
