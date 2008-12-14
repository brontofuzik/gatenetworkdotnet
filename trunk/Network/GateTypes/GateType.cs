using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Network.Exceptions;

namespace Network.GateTypes
{
    /// <summary>
    /// An abstract gate type.
    /// </summary>
    public abstract class GateType
 //       : IGateType
    {
        #region Private instance fields

        /// <summary>
        /// The name of the gate type.
        /// </summary>
        private string name;

        /// <summary>
        /// The names of the inputs.
        /// </summary>
        private List< string > inputs;

        /// <summary>
        /// The names of the outputs.
        /// </summary>
        private List< string > outputs;

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
        /// The number of the inputs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the inputs.
        /// </value>
        public int InputCount
        {
            get
            {
                return inputs.Count;
            }
        }

        /// <summary>
        /// Gets the number of the outputs.
        /// </summary>
        /// 
        /// <value>
        /// The number of the outputs.
        /// </value>
        public int OutputCount
        {
            get
            {
                return outputs.Count;
            }
        }

        #endregion // Public instance properties

        #region Protected instance constructors

        /// <summary>
        /// Creates a new gate type.
        /// </summary>
        /// 
        /// <param name="name">The name of the gate type.</param>
        /// <param name="inputs">The list of the inputs' names.</param>
        /// <param name="outputs">The list of the outputs' names.</param>
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
        protected GateType( string name, List< string > inputs, List< string > outputs )
        {
            // Validate the name.
            // The name is valid if it is a legal name.
            if (!IsLegalName( name ))
            {
                throw new IllegalNameException( name );
            }
            this.name = name;

            // Validate the list of the inputs' names.
            // The list of the inputs' names is legal if it contains unique and legal input names.
            if (inputs == null)
            {
                throw new ArgumentNullException( "inputs" );
            }
            foreach (string input in inputs)
            {
                if (!IsLegalName( input ))
                {
                    throw new IllegalNameException( input );
                }
            }
            this.inputs = inputs;

            // Validate the list of the outputs' names.
            // The list of the outputs' names is legal if it contains (at least one) unique and legal output name.
            if (outputs == null)
            {
                throw new ArgumentNullException( "outputs" );
            }
            if (outputs.Count < 1)
            {
                throw new ArgumentException( "outputs" );
            }
            foreach (string output in outputs)
            {
                if (!IsLegalName( output ))
                {
                    throw new IllegalNameException( output );
                }
            }
            this.outputs = outputs;
        }

        #endregion // Protected instance constructors

        #region Public static methods

        /// <summary>
        /// Determines whether a name is legal.
        /// </summary>
        /// 
        /// <param name="name">The name of a gate type, gate instance, gate input or output, network.</param>
        /// 
        /// <returns>
        /// <c>True</c> if the name is legal, <c>false</c> otherwise.
        /// </returns>
        public static bool IsLegalName( string name )
        {
            string legalNamePattern = @"^.*$";
            Regex legalNameRegex = new Regex( legalNamePattern );

            return legalNameRegex.IsMatch( name );
        }

        #endregion // Public static methods

        #region Public instance mehods

        /// <summary>
        /// Evaluates the transition function of the gate type.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs.</param>
        /// 
        /// <returns>
        /// The outputs.
        /// </returns>
        public abstract string Evaluate( string inputs );

        #endregion // Public instance methods
    }
}
