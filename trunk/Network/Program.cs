using System;
using System.IO;
using System.Collections.Generic;

using Network.Gates;
using Network.GateTypes;

namespace Network
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
                Dictionary<string, BasicGateType> basicGateTypes = new Dictionary<string, BasicGateType>();

                string name;
                List< string > inputs;
                List< string > outputs;
                List< string > transitions;
                BasicGateType basicGateType;

                // Implicitly defined basic gate type: "0".
                name = "0";
                inputs = new List< string >( new string[ 0 ] );
                outputs = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "0" } );
                basicGateType = new BasicGateType( name, inputs, outputs, transitions );
                basicGateTypes.Add( name, basicGateType );

                // Implicitly defined basic gate type: "1".
                name = "1";
                inputs = new List< string >( new string[ 0 ] );
                outputs = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1" } );
                basicGateType = new BasicGateType( name, inputs, outputs, transitions );
                basicGateTypes.Add( name, basicGateType );

                // Basic gate type: "and".
                name = "and";
                inputs = new List< string >( new string[] { "i0", "i1" } );
                outputs = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1 1 1" } );
                basicGateType = new BasicGateType( name, inputs, outputs, transitions );
                basicGateTypes.Add( name, basicGateType );

                // basic gate type: "or".
                name = "or";
                inputs = new List< string >( new string[] { "i0", "i1" } );
                outputs = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1 1 1", "0 1 0", "1 0 0" } );
                basicGateType = new BasicGateType( name, inputs, outputs, transitions );
                basicGateTypes.Add( name, basicGateType );

                // Basic gate type: "not".
                name = "not";
                inputs = new List< string >( new string[] { "i" } );
                outputs = new List< string >( new string[] { "o" } );
                transitions = new List< string >( new string[] { "1 0" } );
                basicGateType = new BasicGateType( name, inputs, outputs, transitions );
                basicGateTypes.Add( name, basicGateType );
         
                //
                // Composite gate types.
                //

                // The dictionary of all the composite gate types.
                Dictionary< string, CompositeGateType > compositeGateTypes = new Dictionary< string, CompositeGateType >();

                List< string > gates;
                List< string > connections;
                CompositeGateType compositeGateType;

                // Composite gate type: "nand".
                name = "nand";
                inputs = new List< string >( new string[] { "i0", "i1" } );
                outputs = new List< string >( new string[] { "o" } );
                gates = new List< string >( new string[] { "a and", "n not" } );
                connections = new List< string >( new string[] { "a.i0->i0", "a.i1->i1", "n.i->a.o", "o->n.o" } );
                compositeGateType = new CompositeGateType( name, inputs, outputs, gates, connections, basicGateTypes );
                compositeGateTypes.Add( name, compositeGateType );           
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
    }
}
