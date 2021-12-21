using System;
using System.Collections.Generic;

namespace ElasticLogBuilder
{
    /// <summary>
    /// Объект запроса с параметрами логируемой сущности
    /// </summary>
    public class ElasticLogRequestDto
    {
        /// <summary>
        /// Дата создания лога
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary> Действие </summary>
        public ActionType ActionType { get; set; }

        /// <summary> Имя базы данных логируемой сущности </summary>
        public string DatabaseName { get; set; }

        /// <summary> summary логируемого класса (значение является идентификатором типа логируемой сущности) </summary>
        public string EntityType { get; set; }

        /// <summary> код сущности </summary>
        public string EntityTypeCode { get; set; }

        /// <summary> Идентификатор логируемой сущности </summary>
        public long EntityId { get; set; }

        /// <summary> Логин пользователя выполнившего действие (денормализация) </summary>
        public string UserLogin { get; set; }

        /// <summary> ФМО оператора </summary>
        public string Operator { get; set; }

        /// <summary> Данные лога. Даты в объекте должны иметь формат yyyy-MM-dd'T'HH:mm:ss.fff</summary>
        public Dictionary<string, ElasticLogFieldRequestDto> JsonData { get; set; }

        /// <summary>
        /// id региона
        /// </summary>
        public long SubdivisionId { get; set; }

        /// <summary>
        /// Коментарий бизнес логики
        /// </summary>
        public string BuisnesComment { get; set; }
    }
}
