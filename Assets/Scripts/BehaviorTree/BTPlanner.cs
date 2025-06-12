using System.Collections.Generic;
using UnityEngine;

public class BTPlanner
{
    private readonly BTNode root;
    private Stack<BTNode> executionStack = new Stack<BTNode>();

    public BTPlanner(BTNode rootNode)
    {
        root = rootNode;
    }

    public Stack<BTNode> Plan()
    {
        executionStack.Clear();
        BuildExecutionStack(root);
        return executionStack;
    }

    private void BuildExecutionStack(BTNode node)
    {
        if (node is ControlNode controlNode)
        {
            // 控制节点的子节点需要按执行顺序的反向压栈
            for (int i = controlNode.children.Count - 1; i >= 0; i--)
            {
                BuildExecutionStack(controlNode.children[i]);
            }
        }
        executionStack.Push(node);
    }
}