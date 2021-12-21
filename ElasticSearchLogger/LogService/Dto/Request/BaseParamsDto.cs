using LogService.Model;
using System;

namespace LogService.Dto
{
    /// <summary>
    /// Параметры запроса для получения списка
    /// </summary>
    [Serializable]
    public class BaseParamsDto
    {
        /// <summary>
        /// С какой записи будет чтение
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// Максимальное кол-во записей
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// Фильтрация записей
        /// </summary>
        public string filter { get; set; }

        /// <summary>
        /// Сортировка записей
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// id региона
        /// </summary>
        public long subdivisionId { get; set; }

        /// <summary>
        /// id записи. Если 0, то не будет учавствовать в фильтре
        /// </summary>
        public long entityid { get; set; }

        public StoreLoadParams GetStoreLoadParams()
        {
            return new StoreLoadParams(start, limit, filter, sort);
        }
    }
}
