namespace ElasticLogBuilder
{
    /// <summary>
    /// Оператор, сделавший изменение
    /// </summary>
    public class ElasticLogOperator
    {
        /// <summary>
        /// ФИО оператора
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// Логин оператора
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Район оператора. 0 - суперадмин
        /// </summary>
        public long SubdivisionId { get; set; }
    }
}
