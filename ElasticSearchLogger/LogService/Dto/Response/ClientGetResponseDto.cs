using System.Text.Json.Serialization;

namespace LogService.Dto
{
    /// <summary>
    /// Запрошенный с клиента объект
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientGetResponseDto<T> : ClientResponseDto
    {
        /// <summary>
        /// Возвращаемый на клиент объект данных
        /// </summary>
        [JsonPropertyName("Data")]
        public virtual T Data { get; set; }
    }
}
