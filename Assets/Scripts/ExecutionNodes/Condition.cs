using System;

public class Condition : ExecutionNode
{
    private readonly Func<bool> condition;

    public Condition(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeStatus Execute()
    {
        return condition() ? NodeStatus.Success : NodeStatus.Failure;
    }
}