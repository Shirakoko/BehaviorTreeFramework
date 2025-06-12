using System.Collections.Generic;

public class BTPlannerRunner
{
    private BTPlanner planner;
    private Stack<BTNode> executionStack;
    private NodeStatus lastStatus = NodeStatus.Failure;

    public BTPlannerRunner(BTNode root)
    {
        planner = new BTPlanner(root);
        executionStack = planner.Plan();
    }

    public NodeStatus ExecuteBT()
    {
        // 如果执行栈为空，重新规划
        if (executionStack.Count == 0)
        {
            executionStack = planner.Plan();
            if (executionStack.Count == 0)
            {
                lastStatus = NodeStatus.Failure;
                return lastStatus;
            }
        }

        // 执行当前节点
        var currentNode = executionStack.Pop();
        lastStatus = currentNode.Execute();

        // 如果当前节点返回Running，保持执行栈不变
        // 如果返回Success或Failure，清空执行栈，下次调用会重新规划
        if (lastStatus != NodeStatus.Running)
        {
            executionStack.Clear();
        }

        return lastStatus;
    }
}