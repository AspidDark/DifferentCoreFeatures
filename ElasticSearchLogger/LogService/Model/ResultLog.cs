using LogService.DB;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LogService.Model
{
    /// <summary>
    /// Результат получения данных лога
    /// </summary>
    public class ResultLog<T> where T: LogMutation
    {
        /// <summary>
        /// Общая информация о логе
        /// </summary>
        public ElasticLog CommonLog { get; set; }

        /// <summary>
        /// История лога
        /// </summary>
        [JsonPropertyName("LogHistory")]
        public List<T> LogHistory { get; set; }
    }
}
