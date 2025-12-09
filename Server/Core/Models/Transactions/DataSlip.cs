using Crolow.Cms.Server.Core.Enums;
using MongoDB.Bson;
using System;

namespace Kalow.Apps.Models.Transactions
{

    public class Slip
    {
        public ObjectId Id { get; set; }
        public ObjectId TransactionId { get; set; }
        public int Sequence { get; set; }
        public TransactionState TransactionState { get; set; }
        public Exception Exception { get; set; }
        public DateTime LastAccess { get; set; }
    }

    public class DataSlip<T> : Slip
    {
        public EditState EditState { get; set; }
        public T OriginalValue { get; set; }
        public T NewValue { get; set; }
    }

}
