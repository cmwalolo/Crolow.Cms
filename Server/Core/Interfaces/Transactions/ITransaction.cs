namespace Crolow.Cms.Server.Core.Interfaces.Transactions
{
    public interface ITransaction<T>
    {
        void Prepare(T dataContainer);
        void Execute(T dataContainer);
        void Commit(T dataContainer);
        void Rollback(T dataContainer);
    }
}
