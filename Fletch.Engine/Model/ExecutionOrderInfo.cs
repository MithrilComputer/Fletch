namespace Fletch.Engine.Model
{
    internal readonly struct ExecutionOrderInfo
    {
        public SystemExecutionOrder Phase { get; }
        public int Order { get; }
        public int RegistrationIndex { get; }

        public ExecutionOrderInfo(SystemExecutionOrder phase, int order, int registrationIndex)
        {
            Phase = phase;
            Order = order;
            RegistrationIndex = registrationIndex;
        }
    }
}
