namespace LogicCircuit.Gates
{
    public class Connection
    {
        private Plug sourcePlug;

        private Plug targetPlug;

        public Plug SourcePlug
        {
            get
            {
                return sourcePlug;
            }
            set
            {
                sourcePlug = value;
            }
        }

        public Plug TargetPlug
        {
            get
            {
                return targetPlug;
            }
            set
            {
                targetPlug = value;
            }
        }
    }
}
