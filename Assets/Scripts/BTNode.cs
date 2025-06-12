using System.Collections.Generic;

public enum NodeStatus {
    /** 运行中 */
    Running,
    /** 成功 */
    Success,
    /** 失败*/
    Failure,
}

/** 行为树节点基类 */
public abstract class BTNode
{
    public abstract NodeStatus Execute();
}

/** 控制节点，可以有子节点 */
public abstract class ControlNode : BTNode
{
    public List<BTNode> children = new List<BTNode>();

    public void AddChild(BTNode child)
    {
        children.Add(child);
    }
}

/** 执行节点，不能有子节点 */
public abstract class ExecutionNode : BTNode { }
