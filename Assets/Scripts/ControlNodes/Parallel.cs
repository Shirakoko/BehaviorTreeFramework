/** Parallel: 并行执行所有子节点 */
public class Parallel : ControlNode 
{
    /// <summary>
    /// 成功所需的最小成功子节点数
    /// -1 表示即使全部子节点成功仍然算作失败
    /// 正数表示至少需要N个子节点成功
    /// </summary>
    private readonly int successThreshold;

    public Parallel(int successThreshold = 1) 
    {
        this.successThreshold = successThreshold == -1 ? int.MaxValue : successThreshold;
    }

    public override NodeStatus Execute() 
    {
        int successCount = 0;
        bool anyRunning = false;

        foreach (var child in children) 
        {
            var status = child.Execute();
            if (status == NodeStatus.Success) successCount++;
            if (status == NodeStatus.Running) anyRunning = true;
        }

        // 成功条件
        if (successCount >= successThreshold) 
            return NodeStatus.Success;

        // 仍有节点在执行中
        if (anyRunning) 
            return NodeStatus.Running;

        // 默认失败（未达到成功阈值且没有运行中的节点）
        return NodeStatus.Failure;
    }
}