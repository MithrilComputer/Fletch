using Fletch.Engine.Systems;

namespace Fletch.Engine.Model
{
    internal sealed class SystemEntry
    {
        public SceneSubsystem System { get; }
        public ExecutionOrderInfo ExecutionOrder { get; }

        public SystemEntry(SceneSubsystem system, ExecutionOrderInfo executionOrder)
        {
            System = system;
            ExecutionOrder = executionOrder;
        }
    }
}
