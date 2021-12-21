using System.Text.Json.Serialization;

namespace LogService.Dto
{
    /// <summary>
    /// Ответ клиенту на успешное сохранение объекта
    /// </summary>
    public class ClientSaveResponseDto : ClientResponseDto
    {
        /// <summary>
        /// id сохраненной сущности
        /// </summary>
        [JsonPropertyName("Id")]
        public long Id { get; set; }
    }
}
