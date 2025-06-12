using UnityEngine;

/** Wait: 等待指定时间 */
public class Wait : ExecutionNode
{
    private readonly float waitTime;
    private float elapsedTime;

    public Wait(float time)
    {
        waitTime = time;
    }

    public override NodeStatus Execute()
    {
        elapsedTime += Time.deltaTime;
        return elapsedTime >= waitTime ? NodeStatus.Success : NodeStatus.Running;
    }
}