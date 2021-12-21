using System.Text.Json.Serialization;

namespace LogService.Dto
{
    /// <summary> Общий тип возвращаемого ответа клиенту. </summary>
    public class ClientResponseDto
    {
        /// <summary>
        /// Успех обработки запроса
        /// </summary>
        [JsonPropertyName("Success")]
        public bool Success { get; set; } = true;

        /// <summary>
        /// Дополнительное сообщение клиенту
        /// </summary>
        [JsonPropertyName("Message")]
        public string Message { get; set; }
    }
}
