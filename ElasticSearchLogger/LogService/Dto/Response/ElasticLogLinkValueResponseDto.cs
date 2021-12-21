using System.Text.Json.Serialization;

namespace LogService.Dto.Response
{
    /// <summary>
    /// Объект ответа на запрос, который описывает ссылку на другой лог
    /// </summary>
    public class ElasticLogLinkValueResponseDto
    {
        /// <summary>
        /// Ссылка на залогируемую сущность. 
        /// 0 ~ если значение не ссылоное
        /// </summary>
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Имя ссылки если есть ссылка (Id), или значение свойства
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; set; }
    }
}
