using LogService.Dto;
using LogService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogService.DB
{
    public static class StoreDataExtension
    {
        #region public methods
        /// <summary> Условная фильтрация последовательности. </summary>
        /// <param name="query"> </param>
        /// <param name="condition"> Проводить или нет фильтрацию.  </param>
        /// <param name="predicate"> Предикат для отбора элементов.  </param>
        /// <typeparam name="T"> Тип элементов последовательности. </typeparam>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        public static IQueryable<T> FilterByProperty<T>(this IQueryable<T> source, StoreLoadParams storeLoadParams)
        {
            return FilterByProperty(source, storeLoadParams?.filter);
        }

        public static IQueryable<T> FilterByProperty<T>(this IQueryable<T> source, List<FilterColumn> filterColumnList)
        {
            if (filterColumnList != null)
            {
                foreach (FilterColumn filterColumn in filterColumnList)
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(filterColumn.property);
                    if (propertyInfo != null)
                    {
                        int intValue;
                        if (filterColumn.type == "int" && int.TryParse(filterColumn.value, out intValue))
                        {
                            if (filterColumn.@operator == "eq")
                            {
                                source = source.Where(PropertyEquals<T, int>(propertyInfo, intValue));
                            }
                            else if (filterColumn.@operator == "gte")
                            {
                                source = source.Where(PropertyGreaterThanOrEqual<T, int>(propertyInfo, int.Parse(filterColumn.value)));
                            }
                            else if (filterColumn.@operator == "lte")
                            {
                                source = source.Where(PropertyLessThanOrEqual<T, int>(propertyInfo, int.Parse(filterColumn.value)));
                            }
                            else if (filterColumn.@operator == "ne")
                            {
                                source = source.Where(PropertyNotEquals<T, int>(propertyInfo, int.Parse(filterColumn.value)));
                            }
                        }
                        else if (filterColumn.type == "float" || filterColumn.type == "int" && !int.TryParse(filterColumn.value, out intValue))
                        {
                            if (filterColumn.@operator == "eq")
                            {
                                source = source.Where(PropertyEquals<T, decimal>(propertyInfo, decimal.Parse(filterColumn.value)));
                            }
                            else if (filterColumn.@operator == "gte")
                            {
                                source = source.Where(PropertyGreaterThanOrEqual<T, decimal>(propertyInfo, decimal.Parse(filterColumn.value)));
                            }
                            else if (filterColumn.@operator == "lte")
                            {
                                source = source.Where(PropertyLessThanOrEqual<T, decimal>(propertyInfo, decimal.Parse(filterColumn.value)));
                            }
                            else if (filterColumn.@operator == "ne")
                            {
                                source = source.Where(PropertyNotEquals<T, decimal>(propertyInfo, decimal.Parse(filterColumn.value)));
                            }
                        }
                        else if (filterColumn.type == "date")
                        {
                            DateTime dateValue = DateTime.Parse(filterColumn.value).Date;
                            if (filterColumn.@operator == "eq")
                            {
                                source = source.Where(PropertyEquals<T, DateTime>(propertyInfo, dateValue));
                            }
                            else if (filterColumn.@operator == "gte")
                            {
                                source = source.Where(PropertyGreaterThanOrEqual<T, DateTime>(propertyInfo, dateValue));
                            }
                            else if (filterColumn.@operator == "lte")
                            {
                                source = source.Where(PropertyLessThanOrEqual<T, DateTime>(propertyInfo, dateValue));
                            }
                            else if (filterColumn.@operator == "ne")
                            {
                                source = source.Where(PropertyNotEquals<T, DateTime>(propertyInfo, dateValue));
                            }
                        }
                        else if (filterColumn.type == "list")
                        {
                            if (filterColumn.value != null && (filterColumn.value.ToLower() == "true" || filterColumn.value.ToLower() == "false"))
                            {
                                source = source.Where(PropertyEquals<T, bool>(propertyInfo, Convert.ToBoolean(filterColumn.value)));
                            }
                            else if (filterColumn.value != null)
                            {
                                source = source.Where(PropertyEquals<T, string>(propertyInfo, filterColumn.value.ToLower()));
                            }
                        }
                        else
                        {
                            source = source.Where(PropertyContains<T>(propertyInfo, filterColumn.value.ToLower()));
                        }
                    }
                }
            }

            return source;
        }

        public static IQueryable<T> Order<T>(this IQueryable<T> source, StoreLoadParams storeLoadParams)
        {
            return Order(source, storeLoadParams?.sort);
        }

        public static IQueryable<T> Order<T>(this IQueryable<T> source, List<OrderColumn> orderColumnList)
        {
            if (orderColumnList == null)
            {
                orderColumnList = new List<OrderColumn>();
            }
            if (orderColumnList.Any() == false)
            {
                orderColumnList.Add(new OrderColumn { direction = "DESC", property = "Id" }); //Entity Framework может применить Paging только для сортированных запросов
            }
            int iterIndex = 0;
            foreach (OrderColumn item in orderColumnList)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(item.property);
                if (propertyInfo != null)
                {
                    source = source.ExtOrderBy(item.property, item.direction, iterIndex == 0);
                    iterIndex++;
                }
            }
            return source;
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> source, StoreLoadParams storeLoadParams)
        {
            if (storeLoadParams == null || storeLoadParams.limit <= 0)
            {
                return source;
            }
            return source.Skip(storeLoadParams.start).Take(storeLoadParams.limit);
        }
        #endregion

        #region private methods
        private static Expression<Func<TItem, bool>> PropertyEquals<TItem, TValue>(PropertyInfo propertyInfo, TValue value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TItem));
            MethodInfo methodInfoToLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            Type propertyType = typeof(TValue);

            //TODO проверить сортировки по датам, у которых есть время
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertyInfo);

            Expression expression;
            ConstantExpression constantExpression;
            if (propertyInfo.PropertyType.IsEnum)
            {
                constantExpression = Expression.Constant(Convert.ChangeType(Enum.Parse(propertyInfo.PropertyType, value.ToString(), true), propertyInfo.PropertyType));
            }
            else
            {
                object objectWithSpecifiedType;
                Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

                if (t != null && t == typeof(DateTime))
                {
                    objectWithSpecifiedType = value as DateTime?;
                    constantExpression = Expression.Constant(objectWithSpecifiedType, propertyInfo.PropertyType);
                }
                else
                {
                    objectWithSpecifiedType = Convert.ChangeType(value, propertyInfo.PropertyType);
                    constantExpression = Expression.Constant(objectWithSpecifiedType);
                }
            }
            Expression expressionLeft = (propertyType != typeof(DateTime) && memberExpression.Expression.NodeType == ExpressionType.MemberAccess) ? memberExpression.Expression : memberExpression;
            if (propertyType == typeof(string) && !propertyInfo.PropertyType.IsEnum)
            {
                expression = Expression.Equal(Expression.Call(memberExpression, methodInfoToLower), constantExpression);
            }
            else
            {
                expression = Expression.Equal(expressionLeft, constantExpression);
            }
            return Expression.Lambda<Func<TItem, bool>>(expression, parameterExpression);
        }

        private static Expression<Func<TItem, bool>> PropertyGreaterThanOrEqual<TItem, TValue>(PropertyInfo propertyInfo, TValue value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TItem));
            MemberExpression memberExpression = typeof(TValue) != typeof(DateTime) ? Expression.Property(parameterExpression, propertyInfo) : Expression.Property(Expression.Property(parameterExpression, propertyInfo), "Date");
            Expression expressionLeft = memberExpression.Expression.NodeType == ExpressionType.MemberAccess ? memberExpression.Expression : memberExpression;
            BinaryExpression binaryExpression = Expression.GreaterThanOrEqual(expressionLeft, Expression.Constant(value));
            return Expression.Lambda<Func<TItem, bool>>(binaryExpression, parameterExpression);
        }

        private static Expression<Func<TItem, bool>> PropertyLessThanOrEqual<TItem, TValue>(PropertyInfo propertyInfo, TValue value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TItem));
            MemberExpression memberExpression = typeof(TValue) != typeof(DateTime) ? Expression.Property(parameterExpression, propertyInfo) : Expression.Property(Expression.Property(parameterExpression, propertyInfo), "Date");
            Expression expressionLeft = memberExpression.Expression.NodeType == ExpressionType.MemberAccess ? memberExpression.Expression : memberExpression;
            BinaryExpression binaryExpression = Expression.LessThanOrEqual(expressionLeft, Expression.Constant(value));
            return Expression.Lambda<Func<TItem, bool>>(binaryExpression.Conversion, parameterExpression);
        }

        private static Expression<Func<TItem, bool>> PropertyNotEquals<TItem, TValue>(PropertyInfo propertyInfo, TValue value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TItem));
            MemberExpression memberExpression = typeof(TValue) != typeof(DateTime) ? Expression.Property(parameterExpression, propertyInfo) : Expression.Property(Expression.Property(parameterExpression, propertyInfo), "Date");
            Expression expressionLeft = memberExpression.Expression.NodeType == ExpressionType.MemberAccess ? memberExpression.Expression : memberExpression;
            BinaryExpression binaryExpression = Expression.NotEqual(expressionLeft, Expression.Constant(value));
            return Expression.Lambda<Func<TItem, bool>>(binaryExpression, parameterExpression);
        }

        private static Expression<Func<T, bool>> PropertyContains<T>(PropertyInfo propertyInfo, string propertyValue)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertyInfo);
            MethodInfo methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            MethodInfo methodInfoToLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            ConstantExpression constantExpression = Expression.Constant(propertyValue, typeof(string));
            MethodCallExpression methodCallExpression = Expression.Call(Expression.Call(memberExpression, methodInfoToLower), methodInfo, constantExpression);

            return Expression.Lambda<Func<T, bool>>(methodCallExpression, parameterExpression);
        }

        private static IQueryable<T> ExtOrderBy<T>(this IQueryable<T> source, string sortProperty, string sortOrder, bool isFirst = true)
        {
            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(sortProperty);
            ParameterExpression parameterExpression = Expression.Parameter(type, "p");
            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, propertyInfo);
            LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);
            Type[] typeArguments = { type, propertyInfo.PropertyType };
            string methodName = "";
            if (isFirst) methodName = sortOrder == "ASC" ? "OrderBy" : "OrderByDescending";
            else methodName = sortOrder == "ASC" ? "ThenBy" : "ThenByDescending";
            MethodCallExpression methodCallExpression = Expression.Call(typeof(Queryable), methodName, typeArguments, source.Expression, Expression.Quote(lambdaExpression));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }
        #endregion
    }
}
