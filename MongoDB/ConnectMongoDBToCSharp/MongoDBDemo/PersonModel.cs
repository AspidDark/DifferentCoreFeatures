using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBDemo;

public class PersonModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
