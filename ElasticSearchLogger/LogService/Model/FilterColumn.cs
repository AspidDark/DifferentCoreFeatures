namespace LogService.Model
{
    /// <summary>
    /// Фильтр записи
    /// </summary>
    public class FilterColumn
    {
        /// <summary>
        /// Тип значения сравниваемого значения
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Сравниваемое значение
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Сравнимое
        /// </summary>
        public string property { get; set; }

        /// <summary>
        /// Оператор стравнения. 
        /// `eq`: =
        /// `gte`: =>
        /// `lte`: <=
        /// `ne`: !=
        /// </summary>
        public string @operator { get; set; }
    }
}
