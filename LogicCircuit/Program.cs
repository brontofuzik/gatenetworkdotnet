using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LogicCircuit.Exceptions;
using LogicCircuit.Gates;
using LogicCircuit.GateTypes;

namespace LogicCircuit
{
    /// <summary>
    /// An entry point of the application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// An entry point of the application.
        /// </summary>
        /// 
        /// <param name="args">The command line arguments.</param>
        static void Main(string[] args)
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
                    throw new Exception("Usage: GateNetworkDotNet.exe NetworkConfigurationFile");
                }
                streamReader = new StreamReader(args[0]);

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
                        if (IsIgnorableLine(line))
                        {
                            continue;
                        }

                        GateType gateType = GateType.ParseGateType(line);
                        if (gateTypes.ContainsKey(gateType.Name))
                        {
                            throw new Exception("Duplicate (" + gateType.Name + ").");
                        }

                        // TODO: Handle EOF eventuality.
                        while (!gateType.IsConstructed && ((line = streamReader.ReadLine()) != null))
                        {
                            lineNumber++;

                            // If the line can be ignored, skip it.
                            if (IsIgnorableLine(line))
                            {
                                continue;
                            }

                            gateType.Configure(line, gateTypes);
                        }

                        if (gateType.IsConstructed)
                        {
                            gateTypes.Add(gateType.Name, gateType);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new MyException(lineNumber, e.Message);
                }

                //
                // 2. Construct the network.
                //


                NetworkGateType networkGateType;
                try
                {
                    networkGateType = (NetworkGateType)gateTypes["networkGateType"];
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception("Missing keyword (network)");
                }
                CompositeGate networkGate = (CompositeGate)networkGateType.Instantiate("networkGate");
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

                    if (line.Equals("end"))
                    {
                        break;
                    }

                    try
                    {
                        Console.WriteLine(networkGate.Evaluate(line));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
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
        /// Determines whether a line can be ignored.
        /// </summary>
        /// 
        /// <param name="line">The line.</param>
        /// 
        /// <returns>
        /// <c>True</c> if the line can be ignored, <c>false</c> otherwise.
        /// </returns>
        public static bool IsIgnorableLine(string line)
        {
            // A line can be ignored if and only if it
            // * is empty,
            // * contains only whitespace characters,
            // * begins with a semicolon.
            string ignorableLinePattern = @"^(|\s*|;.*)$";
            Regex ignorableLineRegex = new Regex(ignorableLinePattern);

            return ignorableLineRegex.IsMatch(line);
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
        public static bool IsLegalName(string name)
        {
            // If the name is of zero length, it is not legal.
            if (name.Length == 0)
            {
                return false;
            }

            // If the name contains a whitespace character, it is not legal.
            if ((name.IndexOf(' ') != -1) || (name.IndexOf('\t') != -1) || (name.IndexOf('\v') != -1) ||
                (name.IndexOf('\n') != -1) || (name.IndexOf('\r') != -1) || (name.IndexOf('\f') != -1))
            {
                return false;
            }

            // If the name contains any of the following characters, it is not legal.
            if ((name.IndexOf('.') != -1) || (name.IndexOf(';') != -1))
            {
                return false;
            }

            // If the name contains any of the following words, it is not legal.
            if (name.Contains("->"))
            {
                return false;
            }

            // If the name starts with the word "end", it is not legal.
            if (name.StartsWith("end"))
            {
                return false;
            }

            return true;
        }
    }
}
