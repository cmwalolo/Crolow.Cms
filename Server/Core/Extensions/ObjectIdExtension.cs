using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System;

namespace Crolow.Cms.Server.Core.Extensions
{
    public static class ObjectIdExtension
    {
        public static ObjectId GetDataSource(this IDataObject node)
        {
            return node.Id; /* TODO */
        }

        public static bool IsZero(this ObjectId id)
        {
            return id == ObjectId.Empty;
        }

        public static ObjectId GetId(byte id)
        {
            return new ObjectId(id, 0, 0, 0);
        }

        public static ObjectId Empty()
        {
            return new ObjectId(0, 0, 0, 0);
        }

        public static Guid GetId()
        {
            return Guid.NewGuid();
        }
    }
}
