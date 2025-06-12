public class BTRunner
{
    private BTNode root;

    public BTRunner(BTNode root)
    {
        this.root = root;
    }

    public void ExecuteBT()
    {
        if (this.root != null)
        {
            root.Execute();
        }
    }
}