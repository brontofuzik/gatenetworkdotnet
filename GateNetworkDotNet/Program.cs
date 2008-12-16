using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
//                // Open the network configuration file.
//                streamReader = new StreamReader( args[ 0 ] );
                
                //
                // Basic gate types.
                //

                // The dictionary of all the basic gate types.
                Dictionary< string, GateType > gateTypes = new Dictionary< string, GateType >();

                string name;
                List< string > inputPlugNames;
                List< string > outputPlugNames;
                List< string > transitions;
                GateType gateType;

                // Implicitly defined basic gate type: "0".
                name = "0";
                inputPlugNames = new List< string >( new string[ 0 ] );
                outputPlugNames = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "0" } );
                gateType = new BasicGateType( name, inputPlugNames, outputPlugNames, transitions );
                gateTypes.Add( name, gateType );

                // Implicitly defined basic gate type: "1".
                name = "1";
                inputPlugNames = new List< string >( new string[ 0 ] );
                outputPlugNames = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1" } );
                gateType = new BasicGateType( name, inputPlugNames, outputPlugNames, transitions );
                gateTypes.Add( name, gateType );

                // Basic gate type: "and".
                name = "and";
                inputPlugNames = new List< string >( new string[] { "i0", "i1" } );
                outputPlugNames = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1 1 1" } );
                gateType = new BasicGateType( name, inputPlugNames, outputPlugNames, transitions );
                gateTypes.Add( name, gateType );

                // basic gate type: "or".
                name = "or";
                inputPlugNames = new List< string >( new string[] { "i0", "i1" } );
                outputPlugNames = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1 1 1", "0 1 1", "1 0 1" } );
                gateType = new BasicGateType( name, inputPlugNames, outputPlugNames, transitions );
                gateTypes.Add( name, gateType );

                // Basic gate type: "not".
                name = "not";
                inputPlugNames = new List< string >( new string[] { "i" } );
                outputPlugNames = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "0 1" } );
                gateType = new BasicGateType( name, inputPlugNames, outputPlugNames, transitions );
                gateTypes.Add( name, gateType );

                //
                // Basic gates.
                //

                string type;
                BasicGate basicGate;

                // Basic gate "a and".
                name = "a";
                type = "and";
                gateType = gateTypes[ type ];
                basicGate = (BasicGate)gateType.Instantiate( name );
                basicGate.InputPlugs[ 0 ].Value = "1";
                basicGate.InputPlugs[ 1 ].Value = "1";
                basicGate.Evaluate();
                Console.WriteLine( basicGate.OutputPlugs[ 0 ].Value );

                // Basic gate "n not".
                name = "n";
                type = "not";
                gateType = gateTypes[ type ];
                basicGate = (BasicGate)gateType.Instantiate( name );
                basicGate.InputPlugs[ 0 ].Value = "1";
                basicGate.Evaluate();
                Console.WriteLine( basicGate.OutputPlugs[ 0 ].Value );

                // basic gate "o or".
                name = "o";
                type = "or";
                gateType = gateTypes[ type ];
                basicGate = (BasicGate)gateType.Instantiate( name );
                basicGate.InputPlugs[ 0 ].Value = "1";
                basicGate.InputPlugs[ 1 ].Value = "1";
                basicGate.Evaluate();
                Console.WriteLine( basicGate.OutputPlugs[ 0 ].Value );

                //
                // Composite gate types.
                //

                List< string > nestedGates;
                List< string > connections;

                // Composite gate type: "nand".
                name = "nand";
                inputPlugNames = new List< string >( new string[] { "i0", "i1" } );
                outputPlugNames = new List< string >( new string[] { "o" } );
                nestedGates = new List< string >( new string[] { "a and", "n not" } );
                connections = new List< string >( new string[] { "a.i0->i0", "a.i1->i1", "n.i->a.o", "o->n.o" } );
                gateType = new CompositeGateType( name, inputPlugNames, outputPlugNames, nestedGates, connections, gateTypes );
                gateTypes.Add( name, gateType );
                
                //
                // Composite gates.
                //

                CompositeGate compositeGate;

                // Composite gate "na nand".
                name = "na";
                type = "nand";
                gateType = gateTypes[ type ];
                //compositeGate = (CompositeGate)gateType.Instantiate( name );
                //compositeGate.InputPlugs[ 0 ].Value = "1";
                //compositeGate.InputPlugs[ 1 ].Value = "1";
                //compositeGate.Evaluate();
                //Console.WriteLine( compositeGate.OutputPlugs[ 0 ].Value);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine( e.Message );
            }
            catch (ArgumentException e)
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
