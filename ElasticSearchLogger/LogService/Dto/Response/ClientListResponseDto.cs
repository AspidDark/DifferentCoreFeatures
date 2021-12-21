using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LogService.Dto
{
    /// <summary>
    /// Ответ клиенту списка запрашиваемых объектов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientListResponseDto<T> : ClientResponseDto
    {
        /// <summary>
        /// Общее кол-во найденых строк
        /// </summary>
        [JsonPropertyName("TotalCount")]
        public long TotalCount { get; set; }

        /// <summary>
        /// Возвращаемый на клиент массив данных
        /// </summary>
        [JsonPropertyName("Data")]
        public virtual List<T> Data { get; set; }
    }
}
