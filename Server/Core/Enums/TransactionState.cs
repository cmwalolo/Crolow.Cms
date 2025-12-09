namespace Crolow.Cms.Server.Core.Enums;

public enum TransactionState
{
    Preparing = 0,
    Ready = 1,
    Locked = 10,
    Onhold = 11,
    Executing = 12,
    Successful = 23,
    Failed = 24,
    Committed = 25,
    Reversed = 36,
    Deleted = 37
}