using Nest;

namespace LogService.DB
{
    /// <summary>
    /// Описывает свойство сущности в логе
    /// </summary>
    public class ElasticLogField
    {
        /// <summary>
        /// Русское наименование свойства
        /// </summary>
        [Text(Name = "name", Analyzer = "russian")]
        public string Name { get; set; }

        /// <summary>
        /// Значение свойство
        /// </summary>
        [Object(Name = "value")]
        public ElasticLogLinkValue Value { get; set; }
    }
}
