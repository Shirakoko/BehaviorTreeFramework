using System;

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