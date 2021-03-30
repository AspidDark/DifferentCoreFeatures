using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB
{
    public class Person
    {
        [BsonId] //id
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Adresses PrimaryAddress { get; set; }
        [BsonElement("dob")] //database nameing change
        public DateTime DateOfBirth { get; set; }
    }
}
