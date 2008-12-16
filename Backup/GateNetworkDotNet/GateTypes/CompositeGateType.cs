using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Connections;

namespace GateNetworkDotNet.GateTypes
{
    /// <summary>
    /// A composite gate type.
    /// </summary>
    public class CompositeGateType
        : GateType
    {
        #region Private instance fields

        /// <summary>
        /// The gates comprising the composite gate type.
        /// </summary>
        private Dictionary< string, Gate > nestedGates;

        /// <summary>
        /// The connections.
        /// </summary>
        private List< Connection > connections;

        #endregion // Private instance fields

        #region Public instance contructors

        /// <summary>
        /// Creates a new composite gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the composite gate type.</param>
        /// <param name="inputs">The list of the inputs' names.</param>
        /// <param name="outputs">The list of the outputs' names.</param>
        /// <param name="gates">The gates comprising the composite gate type.</param>
        /// <param name="connections">The connections.</param>
        /// <param name="gateTypes">The basic gate types.</param>
        /// 
        /// <exception cref="Network.Exceptions.IllegalNameException">
        /// Condition 1: <c>name</c> is an illegal name.
        /// Condition 2: <c>inputs</c> contains an illegal name.
        /// Condition 3: <c>outputts</c> contains an illegal name.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Condition 1: <c>inputs</c> is <c>null</c>.
        /// Condition 2: <c>outputs</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>outputs</c> is empty.
        /// </exception>
        public CompositeGateType( string name, List< string > inputs, List< string > outputs, List< string > gates, List< string > connections, Dictionary< string, BasicGateType > gateTypes )
            : base( name, inputs, outputs )
        {
            ////
            //// Validate and construct the nested gates.
            ////
            //nestedGates = new Dictionary< string, Gate >();
            //foreach (string gate in gates)
            //{
            //    string[] gateNameAndType = gate.Split( ' ' );

            //    if (gateNameAndType.Length != 2)
            //    {
            //        // TODO: Provide more specific exception.
            //        throw new IllegalIdentifierException( gate );
            //    }

            //    string gateName = gateNameAndType[ 0 ];
            //    string gateTypeStr = gateNameAndType[ 1 ];

            //    try
            //    {
            //        BasicGateType gateType = gateTypes[gateTypeStr];
            //        BasicGate basicGate = new BasicGate( gateName, gateType );
                    
            //        nestedGates.Add( gateName, basicGate );
            //    }
            //    catch (KeyNotFoundException e)
            //    {
            //        // TODO: Provide more specific exception.
            //        Console.WriteLine( e.Message );
            //    }
            //}

            ////
            //// Validate and construct the connections.
            ////
            //this.connections = new List< Connection >();
            //foreach (string connection in connections)
            //{

            //}
        }

        #endregion // Public instance contructors

        #region Public instance methods

        /// <summary>
        /// Evaluates the transition function of the gate type.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs.</param>
        /// 
        /// <returns>
        /// The outputs.
        /// </returns>
        public override string[] Evaluate( string[] inputs )
        {
            throw new NotImplementedException();
        }

        #endregion // Public instance methods
    }
}
