using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CascadEntity.Extensions
{
    public static class StoreDataExtension
    {
        #region Методы расширения типа IQueryable
        /// <summary> Условная сортировка последовательности. </summary>
        /// <param name="query"> </param>
        /// <param name="condition"> Проводить или нет сортировку.  </param>
        /// <param name="acs"> Сортировать по возрастанию или убыванию.  </param>
        /// <param name="keySelector"> Выражение для вычисления ключа.  </param>
        /// <typeparam name="T"> Тип элементов последовательности. </typeparam>
        /// <typeparam name="TKey"> Тип ключей. </typeparam>
        public static IQueryable<T> OrderIf<T, TKey>(this IQueryable<T> query, bool condition, bool acs, Expression<Func<T, TKey>> keySelector)
        {
            if (condition == false)
            {
                return query;
            }

            return acs ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
        }

        /// <summary> Дополнительная условная сортировка последовательности. </summary>
        /// <param name="query"> </param>
        /// <param name="condition"> Проводить или нет сортировку.  </param>
        /// <param name="acs"> Сортировать по возрастанию или убыванию.  </param>
        /// <param name="keySelector"> Выражение для вычисления ключа.  </param>
        /// <typeparam name="T"> Тип элементов последовательности. </typeparam>
        /// <typeparam name="TKey"> Тип ключей. </typeparam>
        public static IQueryable<T> OrderThenIf<T, TKey>(this IQueryable<T> query, bool condition, bool acs, Expression<Func<T, TKey>> keySelector)
        {
            if (condition == false)
            {
                return query;
            }

            return acs ? ((IOrderedQueryable<T>)query).ThenBy(keySelector) : ((IOrderedQueryable<T>)query).ThenByDescending(keySelector);
        }


        /// <summary> Условная фильтрация последовательности. </summary>
        /// <param name="query"> </param>
        /// <param name="condition"> Проводить или нет фильтрацию.  </param>
        /// <param name="predicate"> Предикат для отбора элементов.  </param>
        /// <typeparam name="T"> Тип элементов последовательности. </typeparam>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary> Условная фильтрация последовательности. </summary>
        /// <param name="query"> </param>
        /// <param name="condition"> Проводить или нет фильтрацию.  </param>
        /// <param name="predicate"> Предикат для отбора элементов.  </param>
        /// <typeparam name="T"> Тип элементов последовательности. </typeparam>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>Условная фильтрация последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="query">Запрос</param>
        /// <param name="condition">Проводить или нет фильтрацию</param>
        /// <param name="predicateIfTrue">Предикат для отбора элементов true</param>
        /// <param name="predicateIfFalse">Предикат для отбора элементов false</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIfElse<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicateIfTrue, Expression<Func<T, bool>> predicateIfFalse)
        {
            return condition ? query.Where(predicateIfTrue) : query.Where(predicateIfFalse);
        }

        /// <summary> Принадлежит ли элемент массиву. Если элемент равен null, возвращается false. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool In<T>(this T @value, params T[] values)
        {
            return values != null && values.Contains(@value);
        }

        /// <summary> Принадлежит ли элемент последовательности. Если элемент равен null, возвращается false. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool In<T>(this T @value, IQueryable<T> values)
        {
            return values != null && values.Contains(@value);
        }

        /// <summary> Принадлежит ли элемент последовательности. Если элемент равен null, возвращается false. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool In<T>(this T @value, IEnumerable<T> values)
        {
            return values != null && values.Contains(@value);
        }

        /// <summary> Проверяет не принадлежит ли элемент массиву. Если элемент или коллекция равны null, возвращается true. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool NotIn<T>(this T @value, params T[] values)
        {
            return values == null || !values.Contains(@value);
        }

        /// <summary> Проверяет не принадлежит ли элемент последовательности. Если элемент или коллекция равны null, возвращается true. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool NotIn<T>(this T @value, IQueryable<T> values)
        {
            return values == null || !values.Contains(@value);
        }

        /// <summary> Проверяет не принадлежит ли элемент последовательности. Если элемент или коллекция равны null, возвращается true. </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        public static bool NotIn<T>(this T @value, IEnumerable<T> values)
        {
            return values == null || !values.Contains(@value);
        }
        #endregion
    }
}
