using Nest;

namespace LogService.DB
{
    /// <summary>
    /// Описывает ссылку на лог связанной сущности
    /// </summary>
    public class ElasticLogLinkValue
    {
        /// <summary>
        /// Ссылка на оригинальную сущность
        /// </summary>
        [Number(NumberType.Long, Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// Ссылка на залогируемую сущность. 
        /// 0 ~ если значение не ссылоное
        /// </summary>
        [Keyword(Name = "log_id")]
        public string LogId { get; set; }

        /// <summary>
        /// Имя ссылки если есть Id, или значение свойства
        /// </summary>
        [Text(Name = "value", Analyzer = "russian", Fielddata = true)]
        public string Name { get; set; }
    }
}
