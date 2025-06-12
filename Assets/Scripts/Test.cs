using System.Threading;
using UnityEngine;
using static Parallel;

public class Test : MonoBehaviour
{
    private Blackboard blackboard = new Blackboard();
    private BTNode rootNode;
    private bool isRunning = false;
    private BTPlannerRunner runner;
    
    void Start()
    {
        
    }

    void Update()
    {
        // runner.Execute();
        // Thread.Sleep(1000);
    }
}
