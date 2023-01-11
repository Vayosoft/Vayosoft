namespace Vayosoft.Commons.Enums
{
    public enum OperationStatus : byte
    {
        NotStarted = 0,
        Busy = 1,
        Complete = 2,
        Paused = 3,
        Failed = 5,
        Cancelled = 6,
        Retry = 7,
        PartiallyComplete = 8
    }
}
