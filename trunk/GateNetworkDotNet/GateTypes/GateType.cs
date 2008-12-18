using System;
using System.Collections.Generic;

using GateNetworkDotNet.Exceptions;
using GateNetworkDotNet.Gates;
using GateNetworkDotNet.Gates.Plugs;

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

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new (abstract) gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the (abstract) gate type.</param>
        ///
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>name</c> is not a legal identifier.
        /// </exception>
        public GateType( string name )
        {
            // Validate the name.
            if (!Program.IsLegalIdentifier( name ))
            {
                throw new ArgumentException( "name" );
            }
            this.name = name;
        }

        #endregion // Protected instance constructors

        #region Public instance mehods

        /// <summary>
        /// Sets the names of the input plugs.
        /// </summary>
        /// 
        /// <param name="inputPlugNames">The names of the input plugs.</param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// Condition: <c>inputPlugNames</c> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>inputPlugNames</c> contains an illegal input plug name.
        /// </exception>
        public void SetInputPlugNames( string[] inputPlugNames )
        {
            // Validate the names of the input plugs.
            if (inputPlugNames == null)
            {
                throw new ArgumentNullException( "inputPlugNames" );
            }
            foreach (string inputPlugName in inputPlugNames)
            {
                if (!Program.IsLegalIdentifier( inputPlugName ))
                {
                    throw new ArgumentException( "inputPlugNames" );
                }
            }
            this.inputPlugNames = inputPlugNames;
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
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>outputPlugNames</c> contains less than one output plug name.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Condition: <c>outputPlugNames</c> contains an illegal output plug name.
        /// </exception>
        public void SetOutputPlugNames( string[] outputPlugNames )
        {
            // Validate the names of the output plugs.
            if (outputPlugNames == null)
            {
                throw new ArgumentNullException( "outputPlugNames" );
            }
            if (outputPlugNames.Length < 1)
            {
                throw new ArgumentException( "outputPlugNames" );
            }
            foreach (string outputPlugName in outputPlugNames)
            {
                if (!Program.IsLegalIdentifier( outputPlugName ))
                {
                    throw new ArgumentException( "outputPlugNames" );
                }
            }
            this.outputPlugNames = outputPlugNames;
        }

        /// <summary>
        /// Gets the index of an input plug specified by its name.
        /// </summary>
        /// 
        /// <param name="name">The name of the input plug.</param>
        /// 
        /// <returns>
        /// The index of the input plug.
        /// </returns>
        public int GetInputPlugIndex( string name )
        {
            for (int i = 0; i < InputPlugCount; i++)
            {
                if (inputPlugNames[ i ].Equals( name ))
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
        /// <param name="name">The name of the output plug.</param>
        /// 
        /// <returns>
        /// The index of the output plug.
        /// </returns>
        public int GetOutputPlugIndex( string name )
        {
            for (int i = 0; i < OutputPlugCount; i++)
            {
                if (OutputPlugNames[ i ].Equals( name ))
                {
                    return i;
                }
            }
            return -1;
        }

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

        #endregion // Public instance methods
    }
}
