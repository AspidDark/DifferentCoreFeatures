using MongoDataAccess.DataAccess;
using MongoDB.Driver;
using MongoDBDemo;
using MongoDataAccess.Models;

//string connectionString = "mongodb://127.0.0.1:27017";
//string databaseName = "simple_db";
//string collectionName = "people";

//var client = new MongoClient(connectionString);
//var db = client.GetDatabase(databaseName);
//var collection = db.GetCollection<PersonModel>(collectionName);

//var person = new PersonModel { FirstName = "Tim", LastName = "Corey" };

//await collection.InsertOneAsync(person);

//var results = await collection.FindAsync(_ => true);

//foreach (var result in results.ToList())
//{
//    Console.WriteLine($"{result.Id}: {result.FirstName} {result.LastName}");
//}

ChoreDataAccess db = new ChoreDataAccess();

await db.CreateUser(new UserModel() { FirstName = "Tim", LastName = "Corey" });

var users = await db.GetAllUsers();

var chore = new ChoreModel() 
{ 
    AssignedTo = users.First(), 
    ChoreText = "Mow the Lawn", 
    FrequencyInDays = 7 
};

await db.CreateChore(chore);

var chores = await db.GetAllChores();

var newChore = chores.First();
newChore.LastCompleted = DateTime.UtcNow;

await db.CompleteChore(newChore);