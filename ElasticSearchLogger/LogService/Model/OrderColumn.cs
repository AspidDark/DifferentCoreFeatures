namespace LogService.Model
{
    /// <summary>
    /// Сортировка записи
    /// </summary>
    public class OrderColumn
    {
        /// <summary>
        /// Сортируемое свойство
        /// </summary>
        public string property { get; set; }

        /// <summary>
        /// Направление сортировки
        /// `ASC` по возростанию
        /// `DESC` по убыванию
        /// </summary>
        public string direction { get; set; }
    }
}
