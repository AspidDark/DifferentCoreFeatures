using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CascadEntity.Extensions
{
    public static class ObjectParseExtention
    {
        /// <summary>
        /// Приводит <paramref name="obj"/> к типу <typeparamref name="T"/>.
        /// Если не удалось, то возвращается default(<typeparamref name="T"/>).
        /// </summary>
        /// <typeparam name="T">
        /// Тип, к которому приводится объект.
        /// </typeparam>
        /// <param name="obj">
        /// Объект.
        /// </param>
        /// <returns>
        /// Приведенное значение.
        /// </returns>
        public static T To<T>(this object obj)
        {
            return To(obj, default(T));
        }

        /// <summary>
        /// Приводит <paramref name="obj"/> к типу <typeparamref name="T"/>.
        /// Если не удалось, то возвращается <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Тип, к которому приводится объект.
        /// </typeparam>
        /// <param name="obj">
        /// Объект.
        /// </param>
        /// <param name="defaultValue">
        /// Дефолтное значение.
        /// </param>
        /// <returns>
        /// Приведенное значение.
        /// </returns>
        public static T To<T>(this object obj, T defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            if (obj is T)
            {
                return (T)obj;
            }

            Type type = typeof(T);

            if (type == typeof(string))
            {
                return (T)(object)obj.ToString();
            }

            // Далее работаем с типом, лежащим в основе (например, int для int?).
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType.IsEnum)
            {
                return ToEnum(obj, defaultValue, underlyingType);
            }

            try
            {
                return (T)Convert.ChangeType(obj, underlyingType);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Приводит объект к типу bool.
        /// Возвращает true для строки "true".
        /// Возвращает true для целого числа (int, long) отличного от нуля.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <returns>
        /// Приведенное значение. 
        /// </returns>
        public static bool ToBool(this object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool)
            {
                return (bool)obj;
            }

            bool boolValue;

            if (bool.TryParse(obj.ToString(), out boolValue))
            {
                return boolValue;
            }

            int intValue;

            if (int.TryParse(obj.ToString(), out intValue))
            {
                return intValue != 0;
            }

            return false;
        }

        /// <summary>
        /// Приводит объект к типу DateTime.
        /// Если не удалось, возвращает DateTime.MinValue.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <returns>
        /// </returns>
        public static DateTime ToDateTime(this object obj)
        {
            if (obj == null)
            {
                return DateTime.MinValue;
            }

            if (obj is DateTime)
            {
                return (DateTime)obj;
            }

            DateTime dateTimeValue;

            if (DateTime.TryParse(obj.ToString(), CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out dateTimeValue))
            {
                return dateTimeValue;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Приводит объект к типу DateTime.
        /// Если не удалось, возвращает DateTime.MinValue.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <param name="defaultValue">
        /// Значение, возвращаемое, если не удалось выполнить преобразование.
        /// </param>
        /// <returns>
        /// </returns>
        public static DateTime? ToDateTimeNullable(this object obj, DateTime? defaultValue = null)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            if (obj is DateTime)
            {
                return (DateTime)obj;
            }

            DateTime dateTimeValue;

            if (DateTime.TryParse(obj.ToString(), CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out dateTimeValue))
            {
                return dateTimeValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Приводит объект к типу decimal.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <returns>
        /// </returns>
        public static decimal ToDecimal(this object obj)
        {
            if (obj == null)
            {
                return 0M;
            }

            if (obj is decimal)
            {
                return (decimal)obj;
            }

            string decimalString = obj.ToString();

            if (decimalString == string.Empty)
            {
                return 0M;
            }

            string value = decimalString.Replace(
                CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator,
                CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

            decimal decValue;

            if (decimal.TryParse(value, out decValue))
            {
                return decValue;
            }

            return 0M;
        }

        /// <summary>
        /// Приводит объект к типу double.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <returns>
        /// </returns>
        public static double ToDouble(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj is double)
            {
                return (double)obj;
            }

            double dbValue;

            string value = obj.ToString().Replace(
                CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator,
                CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

            if (double.TryParse(value, out dbValue))
            {
                return dbValue;
            }

            return 0;
        }

        /// <summary>
        /// Приводит объект к типу long.
        /// Для true возвращает defaultValue.
        /// </summary>
        /// <param name="obj">
        /// Объект.
        /// </param>
        /// <param name="defaultValue">
        /// Значение, возвращаемое, если не удалось выполнить преобразование. 
        /// </param>
        /// <returns></returns>
        public static long ToLong(this object obj, long defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }

            if (obj is long)
            {
                return (long)obj;
            }

            long longValue;

            if (long.TryParse(obj.ToString(), out longValue))
            {
                return longValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Приводит объект к типу long.
        /// Для true возвращает 1.
        /// </summary>
        /// <param name="obj">
        /// Объект.
        /// </param>
        /// <returns></returns>
        public static long ToLong(this object obj)
        {
            if (obj is bool)
            {
                return (bool)obj ? 1 : 0;
            }

            return obj.ToLong(0);
        }

        /// <summary>
        /// Приводит объект к типу long?.
        /// Для true возвращает 1.
        /// </summary>
        /// <param name="obj">
        /// Объект.
        /// </param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns></returns>
        public static long? ToLongNullable(this object obj, long? defaultValue = null)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }

            if (obj is long)
            {
                return (long)obj;
            }

            long longValue;

            if (long.TryParse(obj.ToString(), out longValue))
            {
                return longValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Приводит объект к типу int.
        /// Для true возвращает defaultValue.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <param name="defaultValue">
        /// Значение, возвращаемое, если не удалось выполнить преобразование.
        /// </param>
        /// <returns>
        /// </returns>
        public static int ToInt(this object obj, int defaultValue)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }

            if (obj is int)
            {
                return (int)obj;
            }

            int intValue;

            if (int.TryParse(obj.ToString(), out intValue))
            {
                return intValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Приводит объект к типу int.
        /// Для true возвращает 1.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <returns>
        /// </returns>
        public static int ToInt(this object obj)
        {
            if (obj is bool)
            {
                return (bool)obj ? 1 : 0;
            }

            return obj.ToInt(0);
        }

        /// <summary>
        /// Приводит объект к типу int?.
        /// Для true возвращает defaultValue.
        /// </summary>
        /// <param name="obj">
        /// Объект. 
        /// </param>
        /// <param name="defaultValue">
        /// Значение, возвращаемое, если не удалось выполнить преобразование.
        /// </param>
        /// <returns>
        /// </returns>
        public static int? ToIntNullable(this object obj, int? defaultValue = null)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return defaultValue;
            }

            if (obj is int)
            {
                return (int)obj;
            }

            int intValue;

            if (int.TryParse(obj.ToString(), out intValue))
            {
                return intValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Приводит объект к типу string.
        /// Если объект равен null, то возвращает пустую строку.
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public static string ToStr(this object obj)
        {
            return obj?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Приведение объекта к типу перечисления.
        /// Работает, если базовый тип перечисления int
        /// и объект имеет тип int, long, string или decimal,
        /// иначе InvalidOperationException.
        /// </summary>
        /// <typeparam name="T">Тип, представляющий собой перречисление.</typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultValue">Значение, возвращаемое, если преобразование не удалось.</param>
        /// <param name="type">Тип, лежащий в основе перечисления.
        /// Если перечисление не Nullable, то тип самого перечисления. 
        /// </param>
        /// <returns></returns>
        private static T ToEnum<T>(object obj, T defaultValue, Type type)
        {
            if (obj is decimal)
            {
                return (T)Enum.Parse(type, obj.ToString());
            }

            if (obj is string)
            {
                return (T)Enum.Parse(type, (string)obj);
            }

            if (obj is long)
            {
                return (T)Enum.Parse(type, obj.ToString());
            }

            if (Enum.IsDefined(type, obj))
            {
                return (T)Enum.Parse(type, obj.ToString());
            }

            return defaultValue;
        }

        /// <summary> Парсинг объекта JSON в массив long элементов </summary>
        public static long[] ToLongArray(this object obj)
        {
#warning NON IMPLEMENT
            // TODO Implement
            //JsonNe js = new JavaScriptSerializer();
            return null;//js.Deserialize<long[]>(obj.ToStr());
        }

        /// <summary> Парсинг строки в список long элементов </summary>
        public static List<long> ToListLong(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new List<long>();

            List<long> listLong = str.Split(',', ';').Select(long.Parse).Where(x => x > 0L).ToList();
            return listLong;
        }

        /// <summary> Количество месяцев между датами </summary>
        /// <param name="date1">Первая дата (из которой вычитается)</param>
        /// <param name="date2">Вторая дата (которая вычитается)</param>
        /// <returns>Количество месяцев между датами. (Вернётся ноль если первая дата меньше или равна второй даты)</returns>  
        public static int TotalMonthDifference(this DateTime date1, DateTime date2)
        {
            if (date1.Date <= date2.Date)
                return 0;
            DateTimeSpan dateTimeSpan = DateTimeSpan.CompareDates(date1, date2);
            int months = dateTimeSpan.Years * 12 + dateTimeSpan.Months;
            return months + 1;
        }
    }
}
