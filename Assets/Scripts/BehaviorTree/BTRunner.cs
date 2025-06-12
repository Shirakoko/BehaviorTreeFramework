using UnityEngine;

public class BTRunner
{
    private BTNode root;

    public BTRunner(BTNode root)
    {
        this.root = root;
    }

    public NodeStatus ExecuteBT()
    {
        if (this.root != null)
        {
            return root.Execute();
        }
        
        Debug.Log("BTRunner: root is null!");
        return NodeStatus.Failure;
    }
}