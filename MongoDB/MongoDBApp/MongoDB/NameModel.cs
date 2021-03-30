using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB
{
    public class NameModel
    {
        [BsonId] //id
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
