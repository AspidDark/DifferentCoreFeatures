namespace ElasticLogBuilder
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class EnumExtension
    {
        /// <summary>
        /// Возвращает значение атрибута у значения перечисления
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }
}
