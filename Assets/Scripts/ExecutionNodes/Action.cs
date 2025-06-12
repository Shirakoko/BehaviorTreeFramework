using System;

/** 行为（Action）：执行行为并返回结果 */
public class Action : ExecutionNode
{
    private readonly Func<NodeStatus> action;

    public Action(Func<NodeStatus> action)
    {
        this.action = action;
    }

    public override NodeStatus Execute()
    {
        return action();
    }
}