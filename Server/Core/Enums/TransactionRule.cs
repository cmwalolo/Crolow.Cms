namespace Crolow.Cms.Server.Core.Enums
{
    public enum TransactionRule
    {
        Ignore = 0,
        Retry = 1,
        Reverse = 2,
        Break = 3,
        TerminateApplication = 4
    }
}