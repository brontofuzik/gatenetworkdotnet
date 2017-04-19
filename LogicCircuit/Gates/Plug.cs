using System;
using System.Collections.Generic;

namespace LogicCircuit.Gates
{
    public class Plug
    {
        private string value;

        private Connection sourceConnection;

        private List< Connection > targetConnections;

        private Gate parentGate;

        public Plug(Gate parentGate)
        {
            value = "?";

            sourceConnection = null;
            targetConnections = new List<Connection>();

            if (parentGate == null)
            {
                throw new ArgumentNullException();
            }
            this.parentGate = parentGate;
        }

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!IsLegalPlugValue(value))
                {
                    throw new Exception("Syntax error.");
                }
                this.value = value;
            }
        }

        public static bool IsLegalPlugValue(string plugValue)
        {
            return (plugValue.Equals("0") || plugValue.Equals("1") || plugValue.Equals("?"));
        }

        public void PlugSourceConnection(Connection sourceConnection)
        {
            if (this.sourceConnection != null)
            {
                // TODO: Provide more specific exception.
                throw new Exception();
            }
            this.sourceConnection = sourceConnection;
            
            sourceConnection.TargetPlug = this;
        }

        public void PlugTargetConnection(Connection targetConnection)
        {
            targetConnections.Add(targetConnection);
            
            targetConnection.SourcePlug = this;
        }

        public bool UpdatePlugValue()
        {
            if (sourceConnection != null)
            {
                if (value != sourceConnection.SourcePlug.Value)
                {
                    value = sourceConnection.SourcePlug.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}