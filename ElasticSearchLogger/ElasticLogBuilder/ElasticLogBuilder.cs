namespace ElasticLogBuilder
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using DocsByReflection;

    public class ElasticLogBuilder
    {
        private Stack<string> _buisnesComments;

        public ElasticLogBuilder()
        {
            _buisnesComments = new Stack<string>();
        }

        /// <summary>
        /// Добавить бизнес комментарии
        /// </summary>
        /// <param name="comment"></param>
        public virtual void Comment(string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                _buisnesComments.Push(comment);
            }
        }

        /// <summary>
        /// Очистить бизнес комментарии
        /// </summary>
        public virtual void ClearComments()
        {
            _buisnesComments.Clear();
        }

        /// <summary>
        /// Построить и заполнить логи в коллекцию dbLogs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="actionType"></param>
        /// <param name="createdOperator"></param>
        /// <param name="databaseName"></param>
        /// <param name="dbLogs"></param>
        public void BuildLogs<T>(T item, ActionType actionType, ElasticLogOperator createdOperator, string databaseName, List<ElasticLogRequestDto> dbLogs)
        {
            Type entityType = typeof(T);
            string entityCode = entityType.Name;
            Type ignoreLogAttributeType = typeof(ElasticIgnoreLogAttribute);
            XmlElement xmlElementEntity = DocsService.GetXmlFromType(entityType, throwError: false);

            //Если class не помечен атрибутом IgnoreLogAttribute, то логируем изменения по сущности
            if (Attribute.IsDefined(entityType, ignoreLogAttributeType) == false && xmlElementEntity != null)
            {
                string entitySummary = xmlElementEntity["summary"]?.InnerText.Trim();
                //И если class имеет summary и значение в нём
                if (string.IsNullOrEmpty(entitySummary) == false)
                {
                    //Получаем список из {SummaryValue= содержимое summary, PropertyValue = значение свойства} для свойст, которые не помечены отрибутом IgnoreLogAttribute и содержащих summary и значения в них
                    var itemProperties = entityType.GetProperties();
                    var propertiesKeyValueForLogList = itemProperties
                        .Where(propertyInfo => Attribute.IsDefined(propertyInfo, ignoreLogAttributeType) == false)
                        .Select(propertyInfo => new
                        {
                            XmlElementProperty = DocsService.GetXmlFromMember(propertyInfo, throwError: false),
                            PropertyInfo = propertyInfo
                        })
                        .Where(x => x.XmlElementProperty != null)
                        .Select(x => new
                        {
                            SummaryValue = x.XmlElementProperty["summary"]?.InnerText.Trim(),
                            PropertyInfo = x.PropertyInfo
                        })
                        .Where(x => string.IsNullOrEmpty(x.SummaryValue) == false)
                        .Select(x => new
                        {
                            SummaryValue = x.SummaryValue,
                            PropertyValue = x.PropertyInfo.GetValue(item),
                            PropertyType = x.PropertyInfo.PropertyType,
                            PropertyName = x.PropertyInfo.Name,
                            IsId = x.PropertyInfo.GetCustomAttributes(true).OfType<ElasticLogPropertyAttribute>()?.FirstOrDefault(i => i.Property == "ElasticLogId") != null
                        })
                        .ToList();
                    long itemId = 0;

                    if (propertiesKeyValueForLogList.Any())
                    {
                        var expandoDictionary = new Dictionary<string, ElasticLogFieldRequestDto>();
                        //TODO для ссылочных полей пока логируем так
                        foreach (var property in propertiesKeyValueForLogList)
                        {
                            if (property.IsId)
                            {
                                itemId = (long)property.PropertyValue;
                            }

                            // лог перечисления
                            if (property.PropertyType.IsEnum && Enum.IsDefined(property.PropertyType, (int)property.PropertyValue))
                            {
                                //todo: продумать, нужно ли id для перечисления. Пока это будет сбивать с толку, т.к. есть значение 0
                                //int enumId = (int)Enum.Parse(property.PropertyType, property.PropertyValue.ToString());
                                string displayName = ((Enum)property.PropertyValue)?.GetAttribute<DisplayAttribute>()?.Name;
                                expandoDictionary[property.PropertyName] = new ElasticLogFieldRequestDto
                                {
                                    Name = property.SummaryValue,
                                    Value = new ElasticLogLinkValueRequestDto { Name = displayName ?? property.PropertyValue?.ToString() ?? string.Empty }

                                };
                            }
                            else
                            {
                                //лог ссылки
                                //var persistentEntity = property.PropertyValue as PersistentEntity;

                                if (property.PropertyType.Assembly.GetName().Name != "mscorlib")
                                {
                                    var persistentEntityProperties = property.PropertyType.GetProperties();
                                    long id = 0;
                                    string elasticLogLinkName = string.Empty;

                                    //поиск имени описывающее ссылочный объект для создания ссылочного лога 
                                    foreach (PropertyInfo prop in persistentEntityProperties)
                                    {
                                        string elasticLogPropertyAttributeName = prop.GetCustomAttributes(true).OfType<ElasticLogPropertyAttribute>()?.FirstOrDefault()?.Property;
                                        string stringValue = string.Empty;

                                        if (elasticLogPropertyAttributeName != null && property.PropertyValue != null)
                                        {
                                            stringValue = prop.GetValue(property.PropertyValue).ToString();
                                        }

                                        switch (elasticLogPropertyAttributeName)
                                        {
                                            case "ElasticLogLinkName":
                                                elasticLogLinkName = stringValue;
                                                break;
                                            case "ElasticLogId":
                                                long.TryParse(stringValue, out id);
                                                break;
                                        }
                                    }

                                    if (id > 0)
                                    {
                                        var persistentEntityProperty = new ElasticLogFieldRequestDto
                                        {
                                            Name = property.SummaryValue,
                                            Value = new ElasticLogLinkValueRequestDto
                                            {
                                                EntityCode = property.PropertyType.Name,
                                                Name = elasticLogLinkName,
                                                Id = id
                                            }
                                        };
                                        expandoDictionary[property.PropertyName] = persistentEntityProperty;
                                    }
                                }
                                else
                                {
                                    // лог даты и времени
                                    DateTime? dateTime = property.PropertyValue as DateTime?;
                                    if (dateTime != null)
                                    {
                                        expandoDictionary[property.PropertyName] = new ElasticLogFieldRequestDto
                                        {
                                            Name = property.SummaryValue,
                                            Value = new ElasticLogLinkValueRequestDto { Name = dateTime.Value == DateTime.MinValue ? null : dateTime.Value.ToString("yyyy-MM-dd'T'HH:mm:ss.fff") }
                                        };
                                    }
                                    else
                                    {
                                        string name = property.PropertyValue?.ToString();

                                        //лог строк и чисел
                                        if (property.PropertyValue is decimal)
                                        {
                                            name = ((decimal)property.PropertyValue).ToString("G29");
                                        }

                                        expandoDictionary[property.PropertyName] = new ElasticLogFieldRequestDto
                                        {
                                            Name = property.SummaryValue,
                                            Value = new ElasticLogLinkValueRequestDto { Name = name }
                                        };
                                    }
                                }
                            }
                        }

                        ElasticLogRequestDto logRequestDto = dbLogs.FirstOrDefault(l => l.EntityTypeCode == entityCode && l.EntityId == itemId);
                        DateTime now = DateTime.Now;
                        DateTime createdDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Utc);

                        if (logRequestDto == null || logRequestDto.ActionType != actionType)
                        {
                            dbLogs.Add(new ElasticLogRequestDto
                            {
                                ActionType = actionType,
                                CreatedDate = createdDate,
                                DatabaseName = databaseName,
                                EntityType = entitySummary,
                                EntityTypeCode = entityCode,
                                EntityId = itemId,
                                UserLogin = createdOperator.Login,
                                Operator = createdOperator.FIO,
                                JsonData = expandoDictionary,
                                SubdivisionId = createdOperator.SubdivisionId,
                                BuisnesComment = string.Join(" ", _buisnesComments)
                            });
                        }
                        else
                        {
                            logRequestDto.ActionType = actionType;
                            logRequestDto.CreatedDate = createdDate;
                            logRequestDto.DatabaseName = databaseName;
                            logRequestDto.EntityType = entitySummary;
                            logRequestDto.UserLogin = createdOperator.Login;
                            logRequestDto.Operator = createdOperator.FIO;
                            logRequestDto.JsonData = expandoDictionary;
                            logRequestDto.SubdivisionId = createdOperator.SubdivisionId;
                            logRequestDto.BuisnesComment = string.Join(" ", _buisnesComments);
                        }
                    }
                }
            }
        }
    }
}
