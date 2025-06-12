/** Selector (FallBack): 按顺序执行子节点，任一成功立即返回成功，全部失败才返回失败 */
public class Selector : ControlNode
{
    public override NodeStatus Execute()
    {
        foreach (var child in children)
        {
            var status = child.Execute();
            if (status != NodeStatus.Failure)
                return status;
        }
        return NodeStatus.Failure;
    }
}