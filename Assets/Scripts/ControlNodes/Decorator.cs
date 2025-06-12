/** 装饰节点，只能有一个子节点 */
public abstract class Decorator : ControlNode
{
    protected BTNode child;
    public bool HasChild => child != null;  // 是否有子节点

    public void SetChild(BTNode node)
    {
        children.Clear();
        AddChild(node);
        child = node;
    }
}

/** Inver: 反转子节点的执行结果 */
public class Invert : Decorator
{
    public override NodeStatus Execute()
    {
        var status = child.Execute();
        switch (status)
        {
            case NodeStatus.Success:
                return NodeStatus.Failure;
            case NodeStatus.Failure:
                return NodeStatus.Success;
            default:
                return status;
        }
    }
}

/** Repeat: 重复执行子节点指定次数 */
public class Repeat : Decorator
{
    private readonly int repeatCount;
    private int currentCount;

    public Repeat(int count)
    {
        repeatCount = count;
    }

    public override NodeStatus Execute()
    {
        while (currentCount < repeatCount)
        {
            var status = child.Execute();
            if (status == NodeStatus.Running)
                return NodeStatus.Running;
            if (status == NodeStatus.Failure)
            {
                currentCount = 0;
                return NodeStatus.Failure;
            }
            currentCount++;
        }
        currentCount = 0;
        return NodeStatus.Success;
    }
}

/** AlwaysSuccess: 无论子节点结果如何都返回成功 */
public class AlwaysSuccess : Decorator
{
    public override NodeStatus Execute()
    {
        child.Execute();
        return NodeStatus.Success;
    }
}

/** AlwaysFailure: 无论子节点结果如何都返回失败 */
public class AlwaysFailure : Decorator
{
    public override NodeStatus Execute()
    {
        child.Execute();
        return NodeStatus.Failure;
    }
}
