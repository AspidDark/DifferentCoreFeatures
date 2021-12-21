using Newtonsoft.Json;

namespace ElasticLogBuilder
{
    /// <summary>
    /// Объект запроса, который описывает свойство сущности в логе
    /// </summary>
    public class ElasticLogFieldRequestDto
    {
        /// <summary>
        /// Русское наименование свойства
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Значение свойства
        /// </summary>
        [JsonProperty("Value")]
        public ElasticLogLinkValueRequestDto Value { get; set; }
    }
}
