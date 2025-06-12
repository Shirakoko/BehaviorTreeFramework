using System;
using System.Collections.Generic;
using UnityEngine;

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

    public BTBuilder Parallel(int successThreshold)
    {
        var node = new Parallel(successThreshold);
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

    private void AttachNode(BTNode node)
    {
        var parent = nodeStack.Peek();
        if (parent != null)
        {
            if (parent is Decorator decorator)
            {
                // 装饰节点使用SetChild设置单个子节点
                // 如果装饰节点已经有子节点，说明是重复调用AttachNode，忽略后续的子节点
                if (!decorator.HasChild)
                {
                    decorator.SetChild(node);
                }
                else
                {
                    Debug.LogWarning($"装饰节点 {decorator.GetType().Name} 已经有一个子节点，忽略后续子节点");
                }
            }
            else
            {
                parent.AddChild(node);
            }
        }

        // 只有控制节点需要入栈，因为：
        // 1. 控制节点（Sequence、Selector、Parallel、Decorator）可以有子节点，需要记住当前节点以便后续添加更多子节点
        // 2. 叶节点（Action、Condition）不能有子节点，不需要入栈
        if (node is ControlNode controlNode)
        {
            nodeStack.Push(controlNode);
        }
    }

    public BTBuilder End()
    {
        // 哨兵节点位于栈底，要用nodeStack.Count > 1来判断栈中的节点个数是否 ＞ 0
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

    public BTRunner Build()
    {
        return new BTRunner(root);
    }
}