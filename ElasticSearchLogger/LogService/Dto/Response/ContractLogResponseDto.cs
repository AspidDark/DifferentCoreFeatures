using LogService.DB;
using LogService.Model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LogService.Dto.Response
{
    /// <summary>
    /// Ответ на запрос истории договора
    /// </summary>
    public class ContractLogResponseDto
    {
        /// <summary>
        /// Id лога
        /// </summary>
        [JsonPropertyName("LogId")]
        public string LogId { get; set; }

        /// <summary>
        /// Текущий статус
        /// </summary>
        [JsonPropertyName("ContractStatus")]
        public string ContractStatus { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        [JsonPropertyName("SaveDateTime")]
        public DateTime SaveDateTime { get; set; }

        /// <summary>
        /// Ответственный
        /// </summary>
        [JsonPropertyName("Responsible")]
        public string Responsible { get; set; }

        /// <summary>
        /// Коментарий системы
        /// </summary>
        [JsonPropertyName("BuisnesComment")]
        public string BuisnesComment { get; set; }

        /// <summary>
        /// Тип изменения
        /// </summary>
        [JsonPropertyName("ActionType")]
        public ElasticLogBuilder.ActionType ActionType { get; set; }

        /// <summary>
        /// Измененные поля
        /// </summary>
        [JsonPropertyName("ChangeFields")]
        public List<ContractLogMutation> ChangeFields { get; set; }

        /// <summary>
        /// id запроса логирования
        /// </summary>
        [JsonIgnore]
        public string RequestId { get; set; }
    }
}
