using LogService.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LogService.Model
{
    /// <summary>
    /// Параметры чтения записей
    /// </summary>
    public class StoreLoadParams
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
        public List<FilterColumn> filter { get; set; }

        /// <summary>
        /// Сортировка записей
        /// </summary>
        public List<OrderColumn> sort { get; set; }

        public StoreLoadParams(List<FilterColumn> filter)
        {
            this.filter = filter;
        }

        public StoreLoadParams(int start, int limit, string filter, string sort)
        {
            this.start = start;
            this.limit = limit;
            this.filter = string.IsNullOrEmpty(filter) ? new List<FilterColumn>() : JsonConvert.DeserializeObject<List<FilterColumn>>(filter);
            this.sort = string.IsNullOrEmpty(sort) ? new List<OrderColumn>() : JsonConvert.DeserializeObject<List<OrderColumn>>(sort);
        }

        public StoreLoadParams(int start, int limit, List<FilterColumn> filter, List<OrderColumn> sort)
        {
            this.start = start;
            this.limit = limit;
            this.filter = filter;
            this.sort = sort;
        }
    }
}
