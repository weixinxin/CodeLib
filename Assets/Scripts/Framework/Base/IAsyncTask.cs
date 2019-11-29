namespace Framework
{
    public enum TaskStatus
    {
        Running,
        Completed,
        Canceled,
    }
    public interface IAsyncTask
    {
        TaskStatus Status
        {
            get;
        }
        bool Cancel();
    }
}
