/** Parallel: 并行执行所有子节点 */
public class Parallel : ControlNode
{
    public enum Policy
    {
        RequireAll,
        RequireOne
    }

    private readonly Policy successPolicy;
    private readonly Policy failurePolicy;

    public Parallel(Policy successPolicy, Policy failurePolicy)
    {
        this.successPolicy = successPolicy;
        this.failurePolicy = failurePolicy;
    }

    public override NodeStatus Execute()
    {
        int successCount = 0, failureCount = 0;

        foreach (var child in children)
        {
            var status = child.Execute();
            if (status == NodeStatus.Success) successCount++;
            if (status == NodeStatus.Failure) failureCount++;
        }

        if (successPolicy == Policy.RequireAll && successCount == children.Count)
            return NodeStatus.Success;
        if (successPolicy == Policy.RequireOne && successCount > 0)
            return NodeStatus.Success;
        if (failurePolicy == Policy.RequireAll && failureCount == children.Count)
            return NodeStatus.Failure;
        if (failurePolicy == Policy.RequireOne && failureCount > 0)
            return NodeStatus.Failure;

        return NodeStatus.Running;
    }
}