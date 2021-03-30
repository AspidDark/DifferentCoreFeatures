using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDB
{
    public class MongoCRUD
    {
        //Connnection
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            //initialize and connect
            //ctrl+shift+space
            var client = new MongoClient();
            db = client.GetDatabase(database);

        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }


        //select * from table
        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }


        //select with filter
        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find<T>(filter).First();
        }


        //Insert or Update
        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);
            //var result = collection.ReplaceOne(new BsonDocument("_id", id), record, new UpdateOptions { IsUpsert=true});
            ReplaceOneResult result = collection.ReplaceOne(new BsonDocument("_id", id), record, new UpdateOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            var tmp= collection.DeleteOne(filter);
        }



    }
}
