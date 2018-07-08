using MongoDB.Bson;
using System;

namespace Leo.Core.Id.Bson
{
    public class ObjectIdProvider : IdProvider
    {
        public string Create(DateTime? timestamp = null)
        {
            if (timestamp.HasValue)
            {
                return ObjectId.GenerateNewId(timestamp.Value).ToString();
            }
            else
            {
                return ObjectId.GenerateNewId().ToString();
            }
        }

        public long Hash(string id)
        {
            ObjectId objectid = new ObjectId();
            if(ObjectId.TryParse(id, out objectid))
            {
                return objectid.GetHashCode();
            }
            else
            {
                throw new Exception("Id is not a valid ObjectId. Unable to generate hash code");
            }
        }
    }
}