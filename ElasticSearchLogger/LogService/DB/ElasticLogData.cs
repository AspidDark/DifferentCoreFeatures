using Nest;

namespace LogService.DB
{
    /// <summary>
    /// Индекс с сылкой на обобщенный лог
    /// </summary>
    public class ElasticLogData<T>: ElasticIndex
    {
        /// <summary>
        /// Ссылка на обобщенный лог
        /// </summary>
        [Keyword(Name = "log_id")]
        public string LogId { get; set; }

        /// <summary> Данные лога. Даты в объекте должны иметь формат yyyy-MM-dd'T'HH:mm:ss.fff</summary>
        [Nested(Name = "json_data")]
        public T JsonData { get; set; }
    }
}
