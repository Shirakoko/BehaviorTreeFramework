/** Sequence: 按顺序执行子节点，全部成功才返回成功，任一失败立即返回失败 */
public class Sequence : ControlNode
{
    public override NodeStatus Execute()
    {
        foreach (var child in children)
        {
            var status = child.Execute();
            if (status != NodeStatus.Success)
                return status;
        }
        return NodeStatus.Success;
    }
}