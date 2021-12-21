using LogService.DB;
using LogService.Model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LogService.Dto
{
    /// <summary>
    /// Ответ запроса, который описывает историю лога
    /// </summary>
    public class LogHistoryDto
    {
        /// <summary>
        /// id лога
        /// </summary>
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonPropertyName("ObjectCreateDate")]
        public DateTime ObjectCreateDate { get; set; }

        /// <summary>
        /// Тип изменения
        /// </summary>
        [JsonPropertyName("ActionType")]
        public ElasticLogBuilder.ActionType ActionType { get; set; }

        /// <summary>
        /// Логин пользователя, который совершил изменение
        /// </summary>
        [JsonPropertyName("UserLogin")]
        public string UserLogin { get; set; }

        /// <summary>
        /// ФИО оператора
        /// </summary>
        [JsonPropertyName("Operator")]
        public string Operator { get; set; }

        /// <summary>
        /// Название измененной сущности
        /// </summary>
        [JsonPropertyName("EntityType")]
        public string EntityType { get; set; }

        /// <summary>
        /// id измененной сущности
        /// </summary>
        [JsonPropertyName("EntityId")]
        public long EntityId { get; set; }

        /// <summary>
        /// Коментарий бизнес логики
        /// </summary>
        [JsonPropertyName("BuisnesComment")]
        public string BuisnesComment { get; set; }

        /// <summary>
        /// История лога
        /// </summary>
        [JsonPropertyName("LogHistory")]
        public List<RawLogMutation> LogHistory { get; set; }
    }
}
