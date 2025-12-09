using Crolow.Cms.Server.Core.Enums;
using Kalow.Apps.Models.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kalow.Apps.Models.Transactions
{
    public class DataTransaction
    {
        public ObjectId Id { get; set; }
        public TransactionState TransactionState { get; set; }
        public TransactionRule TransactionRule { get; set; }
        public Tracking Tracking { get; set; }
        public ObjectId CurrentSlip { get; set; }

        [BsonIgnore]
        public Slip[] Slips { get; set; }
    }
}
