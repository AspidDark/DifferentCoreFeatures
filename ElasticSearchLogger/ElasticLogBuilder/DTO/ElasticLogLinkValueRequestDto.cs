using Newtonsoft.Json;

namespace ElasticLogBuilder
{
    /// <summary>
    /// Объект запроса, который описывает ссылочное значение сущности в логе
    /// </summary>
    public class ElasticLogLinkValueRequestDto
    {
        /// <summary>
        /// Ссылка на запись оригинальной сущности. 
        /// 0 ~ если значение не ссылоное
        /// </summary>
        [JsonProperty("Id")]
        public long Id { get; set; }

        /// <summary>
        /// Имя ссылки, если есть ссылка (Id) или значение свойства
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Код сущности на которую есть ссылка
        /// </summary>
        [JsonProperty("EntityCode")]
        public string EntityCode { get; set; }
    }
}
