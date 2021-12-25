using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    if (request.Path == "/api/user")
    {
        var responseText = "Некорректные данные";   // содержание сообщения по умолчанию

        if (request.HasJsonContentType())
        {
            // определяем параметры сериализации/десериализации
            var jsonoptions = new JsonSerializerOptions();
            // добавляем конвертер кода json в объект типа Person
            jsonoptions.Converters.Add(new PersonConverter());
            // десериализуем данные с помощью конвертера PersonConverter
            var person = await request.ReadFromJsonAsync<Person>(jsonoptions);
            if (person != null)
                responseText = $"Name: {person.Name}  Age: {person.Age}";
        }
        await response.WriteAsJsonAsync(new { text = responseText });
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

public record Person(string Name, int Age);
public class PersonConverter : JsonConverter<Person>
{
    public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var personName = "Undefined";
        var personAge = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    // если свойство Age/age и оно содержит число
                    case "age" or "Age" when reader.TokenType == JsonTokenType.Number:
                        personAge = reader.GetInt32();  // считываем число из json
                        break;
                    // если свойство Age/age и оно содержит строку
                    case "age" or "Age" when reader.TokenType == JsonTokenType.String:
                        string? stringValue = reader.GetString();
                        // пытаемся конвертировать строку в число
                        if (int.TryParse(stringValue, out int value))
                        {
                            personAge = value;
                        }
                        break;
                    case "Name" or "name":  // если свойство Name/name
                        string? name = reader.GetString();
                        if (name != null)
                            personName = name;
                        break;
                }
            }
        }
        return new Person(personName, personAge);
    }
    // сериализуем объект Person в json
    public override void Write(Utf8JsonWriter writer, Person person, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("name", person.Name);
        writer.WriteNumber("age", person.Age);

        writer.WriteEndObject();
    }
}