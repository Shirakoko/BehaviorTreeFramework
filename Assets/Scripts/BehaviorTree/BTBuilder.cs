using System;
using System.Collections.Generic;

public class BTBuilder
{
    private BTNode root;
    private Stack<ControlNode> nodeStack = new Stack<ControlNode>();

    public BTBuilder()
    {
        nodeStack.Push(null); // 哨兵节点
    }

    public BTBuilder Sequence()
    {
        var node = new Sequence();
        AttachNode(node);
        return this;
    }

    public BTBuilder Selector()
    {
        var node = new Selector();
        AttachNode(node);
        return this;
    }

    public BTBuilder Parallel(Parallel.Policy successPolicy, Parallel.Policy failurePolicy)
    {
        var node = new Parallel(successPolicy, failurePolicy);
        AttachNode(node);
        return this;
    }

    public BTBuilder Action(Func<NodeStatus> action)
    {
        var node = new Action(action);
        AttachNode(node);
        return this;
    }

    public BTBuilder Condition(Func<bool> condition)
    {
        var node = new Condition(condition);
        AttachNode(node);
        return this;
    }

    public BTBuilder Wait(float seconds)
    {
        var node = new Wait(seconds);
        AttachNode(node);
        return this;
    }

    public BTBuilder Invert()
    {
        var node = new Invert();
        AttachNode(node);
        return this;
    }

    public BTBuilder Repeat(int count)
    {
        var node = new Repeat(count);
        AttachNode(node);
        return this;
    }

    public BTBuilder AlwaysSuccess()
    {
        var node = new AlwaysSuccess();
        AttachNode(node);
        return this;
    }

    public BTBuilder AlwaysFailure()
    {
        var node = new AlwaysFailure();
        AttachNode(node);
        return this;
    }

    public BTBuilder End()
    {
        if (nodeStack.Count > 1)
        {
            var node = nodeStack.Pop();
            if (nodeStack.Peek() == null)
            {
                root = node;
            }
        }
        return this;
    }

    public BTPlannerRunner Build()
    {
        return new BTPlannerRunner(root);
    }

    private void AttachNode(BTNode node)
    {
        var parent = nodeStack.Peek();
        if (parent != null)
        {
            parent.AddChild(node);
        }

        if (node is ControlNode controlNode)
        {
            nodeStack.Push(controlNode);
        }
    }
}