using Nest;
using System;
using System.Globalization;

namespace LogService.DB
{
    /// <summary>
    /// Модель обобщенного лога
    /// </summary>
    [ElasticsearchType(RelationName = "log")]
    public class ElasticLog: ElasticIndex
    {
        private DateTime _objectCreateDate;

        [Ignore]
        public DateTime ObjectCreateDate 
        { 
            get => _objectCreateDate; 
            set => _objectCreateDate = value; 
        }

        /// <summary>
        /// Дата создания лога
        /// </summary>
        /// https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping-date-format.html
        [Date(Name = "object_create_date", Format = "date_hour_minute_second_millis")]
        public string ObjectCreateDateString 
        { 
            get => _objectCreateDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fff"); 
            private set => _objectCreateDate = DateTime.ParseExact(value, "yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture); 
        }

        /// <summary> Действие </summary>
        [Number(NumberType.Integer, Name = "action_type")]
        public ElasticLogBuilder.ActionType ActionType { get; set; }

        /// <summary> Имя базы данных логируемой сущности </summary>
        [Text(Name = "database_name", Analyzer = "english", Fielddata = true)]
        public string DatabaseName { get; set; }

        /// <summary> Русское наименование сущности </summary>
        [Text(Name = "entity_type", Analyzer = "russian", Fielddata = true)]
        public string EntityType { get; set; }

        /// <summary> код сущности </summary>
        [Keyword(Name = "entity_type_code")]
        public string EntityTypeCode { get; set; }

        /// <summary> Идентификатор логируемой сущности </summary>
        [Number(NumberType.Long, Name = "entity_id")]
        public long EntityId { get; set; }

        /// <summary> Логин пользователя выполнившего действие (денормализация) </summary>
        [Keyword(Name = "user_login")]
        public string UserLogin { get; set; }

        /// <summary> ФИО оператора </summary>
        [Text(Name = "operator", Analyzer = "russian", Fielddata = true)]
        public string Operator { get; set; }

        /// <summary> id региона </summary>
        [Number(NumberType.Long, Name = "subdivision_id")]
        public long SubdivisionId { get; set; }

        /// <summary> Коментарий бизнес логики </summary>
        [Text(Name = "buisnes_comment", Analyzer = "russian")]
        public string BuisnesComment { get; set; }

        /// <summary>
        /// Идентификатор групповых логов для отслеживания изменений в рамках одного запроса
        /// </summary>
        [Keyword(Name = "request_id")]
        public string RequestId { get; set; }
    }
}
