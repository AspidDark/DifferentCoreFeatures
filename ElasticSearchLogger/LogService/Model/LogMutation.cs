using LogService.DB;
using LogService.Dto.Response;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LogService.Model
{
    /// <summary>
    /// Общий объект по измененным свойствам
    /// </summary>
    public abstract class LogMutation
    {
        /// <summary>
        /// Свойство сущности, которое менялось
        /// </summary>
        [JsonPropertyName("PropertyName")]
        public string PropertyName { get; set; }

        public override bool Equals(object obj)
        {
            return ((LogMutation)obj).PropertyName == PropertyName;
        }
    }

    /// <summary>
    /// Изменение свойство лога с данными сущности
    /// </summary>
    public class RawLogMutation : LogMutation
    {
        /// <summary>
        /// Старое значение
        /// </summary>
        [JsonPropertyName("PropertyValueOld")]
        public string PropertyValueOld { get; set; } = string.Empty;

        /// <summary>
        /// Новое значение
        /// </summary>
        [JsonPropertyName("PropertyValueNew")]
        public string PropertyValueNew { get; set; } = string.Empty;        
    }

    /// <summary>
    /// Изменение свойства лога с данными договора
    /// </summary>
    public class ContractLogMutation : LogMutation
    {
        /// <summary>
        /// Старое значение
        /// </summary>
        [JsonPropertyName("PropertyValueOld")]
        public List<ElasticLogLinkValueResponseDto> PropertyValueOld { get; set; } = new List<ElasticLogLinkValueResponseDto>();

        /// <summary>
        /// Новое значение
        /// </summary>
        [JsonPropertyName("PropertyValueNew")]
        public List<ElasticLogLinkValueResponseDto> PropertyValueNew { get; set; } = new List<ElasticLogLinkValueResponseDto>();

        /// <summary>
        /// Есть ли изменения
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return PropertyValueNew.Count > 0 || PropertyValueOld.Count > 0;
        }
    }
}
