using ElasticLogBuilder;
using LogService.DB;
using LogService.Dto;
using LogService.Dto.Response;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogService.Model
{
    public class ElasticLogger : IDBLogger
    {
        private ElasticClient _client;
        private readonly ILogger<ElasticLogger> _logger;
        private const string LogIndexName = "log";
        private const string ContractLogIndexName = "contract";
        private const string ContractcontragentLogIndexName = "contractcontragent";
        private const string ContractdocumentownershipLogIndexName = "contractdocumentownership";
        private const string PennaltyAccrualPeriodLogIndexName = "pennaltyaccrualperiod";

        public ElasticLogger(ElasticClient client, ILogger<ElasticLogger> logger)
        {
            _client = client;
            _logger = logger;
        }

        #region public methods
        public async Task<ClientGetResponseDto<LogHistoryDto>> Get(string id)
        {
            var resultLog = new ResultLog<RawLogMutation>();

            ElasticLog log;

            if (!TryGetElasticLog(LogIndexName, id, out log))
            {
                string message = $"Главный лог {id} не найден";
                return new ClientGetResponseDto<LogHistoryDto> { Success = false, Message = message };
            }

            ElasticLogRawData logData = GetLogRawData(log);

            if (logData == null)
            {
                string message = $"Лог данных для главного лога {id} не найден";
                return new ClientGetResponseDto<LogHistoryDto> { Success = false, Message = message };
            }

            List<RawLogMutation> newLogDataMutation;

            if (log.ActionType == ElasticLogBuilder.ActionType.Delete)
            {
                newLogDataMutation = logData.JsonData.Select(x => new RawLogMutation
                {
                    PropertyName = x.Value.Name,
                    PropertyValueOld = x.Value.Value.Name
                }).ToList();
            }
            else
            {
                newLogDataMutation = logData.JsonData.Select(x => new RawLogMutation
                {
                    PropertyName = x.Value.Name,
                    PropertyValueNew = x.Value.Value.Name
                }).ToList();
            }

            resultLog.CommonLog = log;
            resultLog.LogHistory = newLogDataMutation.ToList();

            if (log.ActionType == ElasticLogBuilder.ActionType.Edit)
            {
                ElasticLog oldLog = GetOldLog(log);

                if (oldLog != null)
                {
                    ElasticLogRawData oldLogData = GetLogRawData(oldLog);

                    if (oldLogData != null)
                    {
                        List<RawLogMutation> oldLogDataMutation = oldLogData.JsonData.Select(x => new RawLogMutation
                        {
                            PropertyName = x.Value.Name,
                            PropertyValueOld = x.Value.Value.Name
                        }).ToList();

                        List<RawLogMutation> logHistory = oldLogDataMutation.Join(newLogDataMutation, x => x.PropertyName, y => y.PropertyName, (x, y) => new RawLogMutation
                        {
                            PropertyName = y.PropertyName,
                            PropertyValueOld = x.PropertyValueOld,
                            PropertyValueNew = y.PropertyValueNew
                        }).ToList();

                        foreach (RawLogMutation logN in newLogDataMutation)
                        {
                            if (!logHistory.Contains(logN))
                            {
                                logHistory.Add(new RawLogMutation
                                {
                                    PropertyName = logN.PropertyName,
                                    PropertyValueNew = logN.PropertyValueNew,
                                    PropertyValueOld = logN.PropertyValueOld
                                });
                            }
                        }

                        resultLog.LogHistory = logHistory.OrderBy(x => x.PropertyName).ToList();
                    }
                }
            }

            return new ClientGetResponseDto<LogHistoryDto>
            {
                Data = new LogHistoryDto
                {
                    Id = resultLog.CommonLog.Id,
                    ActionType = resultLog.CommonLog.ActionType,
                    EntityId = resultLog.CommonLog.EntityId,
                    EntityType = resultLog.CommonLog.EntityType,
                    ObjectCreateDate = resultLog.CommonLog.ObjectCreateDate,
                    UserLogin = resultLog.CommonLog.UserLogin,
                    Operator = resultLog.CommonLog.Operator,
                    BuisnesComment = resultLog.CommonLog.BuisnesComment,
                    LogHistory = resultLog.LogHistory
                }
            };
        }

        public async Task<ClientListResponseDto<ContractLogResponseDto>> GetContractLog(BaseParamsDto baseParams)
        {
            var contractCommoneLogs = new List<ElasticLog>();
            long count;
            var contractLogs = new ClientListResponseDto<ContractLogResponseDto>
            {
                Data = new List<ContractLogResponseDto>(),
                TotalCount = 0,
            };

            if (baseParams.entityid <= 0)
            {
                contractLogs.Success = false;
                contractLogs.Message = "Не указан id договора";
                return contractLogs;
            }

            StoreLoadParams storeParams = baseParams.GetStoreLoadParams();

            //сопоставления полей фильтрации по ContractLogResponseDto с полями в индексах log и contract
            foreach (FilterColumn filter in storeParams.filter)
            {
                switch (filter.property)
                {
                    case "ContractStatus":
                        filter.property = "Status";//contract
                        break;
                    case "ActionType":
                        filter.property = "actiontype";//log
                        break;
                    case "SaveDateTime":
                        filter.property = "objectcreatedate";//log
                        break;
                    case "Responsible":
                        filter.property = "operator";//log
                        break;
                    case "BuisnesComment":
                        filter.property = "buisnescomment";//log
                        break;
                }
            }

            //сопоставления полей сортировки по ContractLogResponseDto с полями в индексах log и contract
            foreach (OrderColumn sort in storeParams.sort)
            {
                switch (sort.property)
                {
                    case "ContractStatus":
                        sort.property = "Status";//contract
                        break;
                    case "ActionType":
                        sort.property = "actiontype";//log
                        break;
                    case "SaveDateTime":
                        sort.property = "objectcreatedate";//log
                        break;
                    case "Responsible":
                        sort.property = "operator";//log
                        break;
                    case "BuisnesComment":
                        sort.property = "buisnescomment";//log
                        break;
                }
            }
            
            storeParams.filter.Add(new FilterColumn { @operator = "ne", property = "actiontype", value = "30" });

            if (baseParams.subdivisionId > 0)
            {
                storeParams.filter.Add(new FilterColumn { @operator = "eq", property = "subdivisionid", value = baseParams.subdivisionId.ToString() });
            }

            var commonLogIdList = new List<string>();
            var tempCommonLogIdList = new List<string>();
            tempCommonLogIdList.AddRange(GetRelatedEntityCommonLogIds(ContractcontragentLogIndexName, "Contract", baseParams.entityid));
            tempCommonLogIdList.AddRange(GetRelatedEntityCommonLogIds(ContractdocumentownershipLogIndexName, "Contract", baseParams.entityid));
            tempCommonLogIdList.AddRange(GetRelatedEntityCommonLogIds(PennaltyAccrualPeriodLogIndexName, "Contract", baseParams.entityid));

            #region применяем клиентский фильтр к логам полученных id, и убираем логи актуализации
            string idsString = string.Join(',', tempCommonLogIdList);
            var idsFilters = new List<FilterColumn> { 
                new FilterColumn { @operator = "eq", property = "ids", value = idsString}
            };

            foreach (FilterColumn fc in storeParams.filter)
            {
                idsFilters.Add(fc);
            }

            Func<BoolQueryDescriptor<ElasticLog>, IBoolQuery> boolQuery = GetElasticBoolQuery(idsFilters);
            Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> querySelector = (q) => q.Bool(boolQuery);
            var searchDescriptor = new SearchDescriptor<ElasticLog>();
            searchDescriptor.Index(LogIndexName).Query(querySelector);
            ISearchResponse<ElasticLog> searchResponse = _client.Search<ElasticLog>(s => searchDescriptor);

            if (searchResponse.HitsMetadata != null)
            {
                foreach (var hit in searchResponse.HitsMetadata.Hits)
                {
                    commonLogIdList.Add(hit.Id);
                }
            }
            else
            {
                if (searchResponse.ServerError?.Error?.Reason != null)
                {
                    _logger.LogError($"Ошибка поиска связанных логов {idsString} : {searchResponse.ServerError?.Error?.Reason}. {searchResponse.ServerError?.Error?.RootCause?.FirstOrDefault()?.Reason}");
                }
            }
            #endregion

            storeParams.filter.Add(new FilterColumn { @operator = "eq", property = "entityid", value = baseParams.entityid.ToString() });
            storeParams.filter.Add(new FilterColumn { @operator = "eq", property = "entitytypecode", value = "Contract" });

            bool needAggregationCountByRequest = true;

            if (TryGetCommonLogs(storeParams, commonLogIdList, ref contractCommoneLogs, out count, needAggregationCountByRequest))
            {
                contractLogs.TotalCount += count;
                var contractLogResponseDtoList = new List<ContractLogResponseDto>();

                foreach (ElasticLog contractCommoneLog in contractCommoneLogs)
                {
                    ContractLogResponseDto contractLogResponseDto = GetContractLogResponseDto(contractCommoneLog, storeParams.filter);

                    if (contractLogResponseDto != null)
                    {
                        contractLogResponseDtoList.Add(contractLogResponseDto);
                    }
                }

                if (needAggregationCountByRequest)
                {
                    foreach (ContractLogResponseDto contractLogResponseDto in contractLogResponseDtoList)
                    {
                        ContractLogResponseDto foundContractLogResponseDto = contractLogs.Data.FirstOrDefault(f => f.RequestId == contractLogResponseDto.RequestId);

                        if (foundContractLogResponseDto == null)
                        {
                            contractLogs.Data.Add(contractLogResponseDto);
                        }
                        else
                        {
                            foreach (ContractLogMutation changeField in contractLogResponseDto.ChangeFields)
                            {
                                var foundChangeField = foundContractLogResponseDto.ChangeFields.FirstOrDefault(f => f.PropertyName == changeField.PropertyName);

                                if (foundChangeField != null)
                                {
                                    foundChangeField.PropertyValueOld.AddRange(changeField.PropertyValueOld);
                                    foundChangeField.PropertyValueNew.AddRange(changeField.PropertyValueNew);
                                }
                                else
                                {
                                    foundContractLogResponseDto.ChangeFields.Add(changeField);
                                }
                            }
                        }
                    }
                }
                else
                {
                    contractLogs.Data.AddRange(contractLogResponseDtoList);
                }
            }

            return contractLogs;
        }

        public async Task<ClientListResponseDto<LogResponseDto>> GetList(BaseParamsDto baseParams)
        {
            var commoneLogs = new List<ElasticLog>();
            long count;
            List<LogResponseDto> dblogs;
            StoreLoadParams storeParams = baseParams.GetStoreLoadParams();

            if (baseParams.subdivisionId > 0)
            {
                storeParams.filter.Add(new FilterColumn { @operator = "eq", property = "subdivisionid", value = baseParams.subdivisionId.ToString() });
            }

            if (TryGetCommonLogs(storeParams, null, ref commoneLogs, out count))
            {
                dblogs = commoneLogs.Select(d => new LogResponseDto
                {
                    Id = d.Id,
                    ActionType = d.ActionType,
                    EntityId = d.EntityId,
                    EntityType = d.EntityType,
                    ObjectCreateDate = d.ObjectCreateDate,
                    UserLogin = d.UserLogin,
                    Operator = d.Operator,
                    BuisnesComment = d.BuisnesComment
                }).ToList();

                return new ClientListResponseDto<LogResponseDto> { Data = dblogs, TotalCount = count };
            }
            else
            {
                dblogs = new List<LogResponseDto>();
                return new ClientListResponseDto<LogResponseDto> { Data = dblogs, TotalCount = 0, Success = false, Message = $"Ошибка получения логов" };
            }
        }

        public void Post(List<ElasticLogRequestDto> requestLogs)
        {
            foreach (ElasticLogRequestDto requestLog in requestLogs)
            {
                string indexName = requestLog.EntityTypeCode.ToLower();

                if (!_client.Indices.Exists(Indices.Index(indexName)).Exists)
                {
                    _logger.LogInformation("Создание нового индекса {indexName}");

                    var rawLogCreateIndexResponse = _client.Indices.Create(indexName, c => c
                        .Map<ElasticLogData<object>>(m => m.AutoMap()));

                    if (!rawLogCreateIndexResponse.IsValid)
                    {
                        _logger.LogError($"Ошибка создания индекса {indexName}: {rawLogCreateIndexResponse.ServerError?.Error?.Reason}");
                    }
                }
            }

            Task.Run(() => SaveLogs(requestLogs));
        }
        #endregion

        #region private methods
        /// <summary>
        /// Сохранение логов в бд
        /// </summary>
        /// <param name="requestLogs"></param>
        private void SaveLogs(List<ElasticLogRequestDto> requestLogs)
        {
            try
            {
                string entityTypes = string.Join(", ", requestLogs.Select(r => $"{ r.DatabaseName}.{r.EntityType}"));
                _logger.LogInformation($"Запись логов для объектов {entityTypes}");
                var elasticLogs = new Dictionary<ElasticLog, Dictionary<string, ElasticLogFieldRequestDto>>();
                var elasticRawLogs = new List<ElasticLogRawData>();
                string requestId = Guid.NewGuid().ToString();

                foreach (ElasticLogRequestDto requestLog in requestLogs)
                {
                    if (string.IsNullOrEmpty(requestLog.EntityTypeCode))
                    {
                        _logger.LogError($"Не указан код таблицы");
                        continue;
                    }

                    var logId = Guid.NewGuid().ToString().ToLower();
                    var log = new ElasticLog
                    {
                        Id = logId,
                        ActionType = requestLog.ActionType,
                        ObjectCreateDate = requestLog.CreatedDate,
                        DatabaseName = requestLog.DatabaseName,
                        EntityId = requestLog.EntityId,
                        EntityType = requestLog.EntityType,
                        EntityTypeCode = requestLog.EntityTypeCode,
                        UserLogin = requestLog.UserLogin,
                        Operator = requestLog.Operator,
                        SubdivisionId = requestLog.SubdivisionId,
                        BuisnesComment = requestLog.BuisnesComment,
                        IndexName = LogIndexName,
                        RequestId = requestId
                    };

                    bool anyChanges = AnyChanges(log, requestLog);

                    if (anyChanges)
                    {
                        elasticLogs.Add(log, requestLog.JsonData);
                    }
                }

                foreach (KeyValuePair<ElasticLog, Dictionary<string, ElasticLogFieldRequestDto>> log in elasticLogs)
                {
                    var rawLogId = Guid.NewGuid().ToString().ToLower();
                    var jsonData = new Dictionary<string, ElasticLogField>();

                    foreach (KeyValuePair<string, ElasticLogFieldRequestDto> field in log.Value)
                    {
                        if (field.Value.Value.Name == null)
                        {
                            continue;
                        }

                        var fieldValue = new ElasticLogLinkValue { Name = field.Value.Value.Name };

                        //если есть ссылка на оригинальную сущность, то ищем ссылку на залогируемую сущность
                        if (field.Value.Value.Id > 0)
                        {
                            fieldValue.Id = field.Value.Value.Id;

                            //поиск ссылочного объекта в одном из сохраняемых логах текущего запроса
                            ElasticLog linkElasticLog = elasticLogs.Keys.Where(e => e.EntityTypeCode == field.Value.Value.EntityCode && e.EntityId == field.Value.Value.Id).FirstOrDefault();

                            //если ссылочный объект нашелся
                            if (linkElasticLog != null)
                            {
                                fieldValue.LogId = linkElasticLog.Id;
                            }
                            else
                            {
                                //иначе ищем в последний лог записи
                                ElasticLog linkLog;
                                var searchOldLogSearchDescripter = new SearchDescriptor<ElasticLog>()
                                        .Index(LogIndexName)
                                        .Size(1)
                                        .Query(q => q
                                            .Bool(b => b
                                                .Must(m => m.Term(t => t.EntityId, field.Value.Value.Id)
                                                    && m.Term(t => t.EntityTypeCode, field.Value.Value.EntityCode))
                                            )
                                        )
                                        .Sort(s => s.Field(f => f.ObjectCreateDateString, SortOrder.Descending));

                                TryGetElasticDocument(searchOldLogSearchDescripter, out linkLog);

                                if (linkLog != null)
                                {
                                    fieldValue.LogId = linkLog.Id;
                                }
                            }
                        }

                        jsonData.Add(field.Key, new ElasticLogField { Name = field.Value.Name, Value = fieldValue });
                    }

                    var rawLog = new ElasticLogRawData
                    {
                        Id = rawLogId,
                        LogId = log.Key.Id,
                        IndexName = log.Key.EntityTypeCode.ToLower(),
                        JsonData = jsonData
                    };
                    elasticRawLogs.Add(rawLog);
                }

                if (elasticLogs.Count > 0)
                {
                    var logBulk = new BulkDescriptor();
                    logBulk.Index(LogIndexName);
                    logBulk.IndexMany<ElasticLog>(elasticLogs.Keys, (descriptor, log) => descriptor.Index(log.IndexName).Id(log.Id));
                    var logResponse = _client.Bulk(b => logBulk);

                    if (logResponse.Errors)
                    {
                        foreach (var itemWithError in logResponse.ItemsWithErrors)
                        {
                            _logger.LogError($"Ошибка записи основного лога {itemWithError.Id}: {itemWithError.Error?.Reason}");
                        }
                    }
                }

                if (elasticRawLogs.Count > 0)
                {
                    var rawLogBulk = new BulkDescriptor();
                    rawLogBulk.IndexMany<ElasticLogRawData>(elasticRawLogs, (descriptor, log) => descriptor.Index(log.IndexName).Id(log.Id));
                    var rawResponse = _client.Bulk(b => rawLogBulk);

                    if (rawResponse.Errors)
                    {
                        foreach (var itemWithError in rawResponse.ItemsWithErrors)
                        {
                            _logger.LogError($"Ошибка записи лога с данными {itemWithError.Id}: {itemWithError.Error?.Reason}");
                        }
                    }
                }

                _logger.LogInformation($"Запись логов для объектов {entityTypes} успешно завершено");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка записи лога : {ex.GetOriginalException().Message}");
            }
        }

        /// <summary>
        /// Есть ли отличия  по сравнению с предыдущей записью
        /// </summary>
        /// <param name="log"></param>
        /// <param name="requestLog"></param>
        /// <returns></returns>
        private bool AnyChanges(ElasticLog log, ElasticLogRequestDto requestLog)
        {
            if (log.ActionType == ElasticLogBuilder.ActionType.Edit)
            {
                try
                {
                    ElasticLog oldLog = GetOldLog(log);

                    if (oldLog != null)
                    {
                        ElasticLogRawData oldLogData = GetLogRawData(oldLog);

                        if (oldLogData != null)
                        {
                            foreach (KeyValuePair<string, ElasticLogFieldRequestDto> newLogfield in requestLog.JsonData)
                            {
                                ElasticLogField oldLogField;

                                //Старое значение пустое, а новое - не пустое
                                if (!oldLogData.JsonData.TryGetValue(newLogfield.Key, out oldLogField) && newLogfield.Value.Value.Name != null)
                                {
                                    return true;
                                }

                                if (oldLogField != null)
                                {
                                    //Изменилось наименование свойства
                                    if (oldLogField.Name != newLogfield.Value.Name)
                                    {
                                        return true;
                                    }

                                    //Изменились ссылка
                                    if (oldLogField.Value.Id != newLogfield.Value.Value.Id)
                                    {
                                        return true;
                                    }

                                    //У не ссылочного свойства изменилось значение
                                    if (oldLogField.Value.Id == 0 && oldLogField.Value.Name != newLogfield.Value.Value.Name)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка поиска наличия изменений: {ex.GetOriginalException().Message}");
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Получение лога данных
        /// </summary>
        /// <param name="logDataIndexName">Индекс лога данных</param>
        /// <param name="logId">id документа в в главном логе</param>
        /// <returns></returns>
        private bool TryGetElasticDocument<T>(SearchDescriptor<T> searchDescripter, out T logData) where T : ElasticIndex
        {
            var searchLogData = _client.Search<T>(s => searchDescripter);

            if (searchLogData.IsValid)
            {
                var searchLogDataResult = searchLogData.Hits.FirstOrDefault();
                logData = searchLogDataResult?.Source;

                if (logData != null)
                {
                    logData.Id = searchLogDataResult.Id;
                    return true;
                }
            }
            else
            {
                string message = $"Ошибка поиска";
                _logger.LogError($"{message}: {searchLogData.ServerError?.Error?.Reason}. {searchLogData.ServerError?.Error?.RootCause?.FirstOrDefault()?.Reason}");
            }

            logData = null;
            return false;
        }

        /// <summary>
        /// Получение главного лога
        /// </summary>
        /// <param name="logIndexName">Индекс главного лога</param>
        /// <param name="logId">id документа в в главном логе</param>
        /// <returns>главный лог</returns>
        private bool TryGetElasticLog(string logIndexName, string logId, out ElasticLog log)
        {
            GetResponse<ElasticLog> logResponse = _client.Get<ElasticLog>(logId, g => g.Index(logIndexName));

            if (logResponse.IsValid && logResponse.Source != null)
            {
                log = logResponse.Source;
                log.Id = logResponse.Id;
                return true;
            }

            string message = $"Ошибка получения лога {logId}";
            _logger.LogError($"{message}: {logResponse.ServerError?.Error}");
            log = null;
            return false;
        }

        private List<string> GetTableList()
        {
            var tableList = new List<string>();
            //http://localhost:9200/_aliases
            return tableList;
        }

        /// <summary>
        /// Формирует сортировку
        /// </summary>
        /// <param name="sorters"></param>
        /// <returns></returns>
        private SortDescriptor<ElasticLog> GetSortDescriptor(List<OrderColumn> sorters)
        {
            SortDescriptor<ElasticLog> sortDescriptor = new SortDescriptor<ElasticLog>();

            foreach (var sort in sorters)
            {
                var sortOrder = SortOrder.Descending;

                if (sort.direction == "ASC")
                {
                    sortOrder = SortOrder.Ascending;
                }

                switch (sort.property.ToLower())
                {
                    case "entityid":
                        sortDescriptor.Field(f => f.Field(p => p.EntityId).Order(sortOrder).Mode(SortMode.Min));
                        break;
                    case "actiontype":
                        sortDescriptor.Field(f => f.Field(p => p.ActionType).Order(sortOrder).Mode(SortMode.Min));
                        break;
                    case "entitytype":
                        sortDescriptor.Field(f => f.Field(p => p.EntityType).Order(sortOrder).Mode(SortMode.Min));
                        break;
                    case "userlogin":
                        sortDescriptor.Field(f => f.Field(p => p.UserLogin).Order(sortOrder).Mode(SortMode.Min));
                        break;
                    case "operator":
                        sortDescriptor.Field(f => f.Field(p => p.Operator).Order(sortOrder).Mode(SortMode.Min));
                        break;
                }

            }

            var objectcreatedateSort = sorters.Where(s => s.property.ToLower() == "objectcreatedate").FirstOrDefault();
            var objectcreatedateSortOrder = SortOrder.Descending;

            if (objectcreatedateSort != null && objectcreatedateSort.direction == "ASC")
            {
                objectcreatedateSortOrder = SortOrder.Ascending;
            }

            sortDescriptor.Field(f => f.ObjectCreateDateString, objectcreatedateSortOrder);
            return sortDescriptor;
        }

        /// <summary>
        /// Получение общих логов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <param name="includeEntyties"></param>
        /// <returns></returns>
        private bool TryGetCommonLogs(StoreLoadParams storeParams, List<string> orIds, ref List<ElasticLog> elasticLogs, out long count, bool needAggregationCountByRequest = false)
        {
            //todo: передать только фильтры по индексу log
            Func<BoolQueryDescriptor<ElasticLog>, IBoolQuery> boolQuery = GetElasticBoolQuery(storeParams.filter);
            Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> querySelector;

            if (orIds != null && orIds.Count > 0)
            {
                var shouldQueries = new List<Func<QueryContainerDescriptor<ElasticLog>, QueryContainer>>();
                Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> orIdsQuery = (q) => q.Ids(s => s.Values(orIds));
                shouldQueries.Add(orIdsQuery);

                if (boolQuery != null)
                {
                    Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> filterQuery = (q) => q.Bool(boolQuery);
                    shouldQueries.Add(filterQuery);
                }

                Func<BoolQueryDescriptor<ElasticLog>, IBoolQuery> parentBoolQuery = (bq) => bq.Should(shouldQueries);
                querySelector = (q) => q.Bool(parentBoolQuery);
            }
            else
            {
                if (boolQuery == null)
                {
                    querySelector = (q) => q.MatchAll();
                }
                else
                {
                    querySelector = (q) => q.Bool(boolQuery);
                }
            }

            SortDescriptor<ElasticLog> sortDescriptor = GetSortDescriptor(storeParams.sort);
            var searchDescriptor = new SearchDescriptor<ElasticLog>();
            searchDescriptor.Index(LogIndexName)
                .Query(querySelector)
                .Sort(s => sortDescriptor);

            if (needAggregationCountByRequest)
            {
                searchDescriptor.Size(500);
                searchDescriptor.Aggregations(a => a.Cardinality("request_id", t => t.Field(f => f.RequestId)));
            }
            else
            {
                searchDescriptor.From(storeParams.start).Size(storeParams.limit);
            }

            ISearchResponse<ElasticLog> searchResponse = _client.Search<ElasticLog>(s => searchDescriptor);
            count = 0;

            if (searchResponse.HitsMetadata != null)
            {
                if (needAggregationCountByRequest)
                {
                    count = (long)searchResponse.Aggregations.Cardinality("request_id").Value;
                }
                else
                {
                    var countResponse = _client.Count<ElasticLog>(s => s.Index(LogIndexName).Query(querySelector));
                    count = countResponse.Count;
                }

                IEnumerable<ElasticLog> elasticLogE = searchResponse.HitsMetadata.Hits.Select(s => new ElasticLog
                {
                    Id = s.Id,
                    ActionType = s.Source.ActionType,
                    BuisnesComment = s.Source.BuisnesComment,
                    DatabaseName = s.Source.DatabaseName,
                    EntityId = s.Source.EntityId,
                    EntityType = s.Source.EntityType,
                    EntityTypeCode = s.Source.EntityTypeCode,
                    IndexName = s.Source.IndexName,
                    ObjectCreateDate = s.Source.ObjectCreateDate,
                    Operator = s.Source.Operator,
                    RequestId = s.Source.RequestId,
                    SubdivisionId = s.Source.SubdivisionId,
                    UserLogin = s.Source.UserLogin
                }).AsEnumerable();

                if (needAggregationCountByRequest)
                {
                    elasticLogs = elasticLogE.Skip(storeParams.start).Take(storeParams.limit).ToList();
                }
                else
                {
                    elasticLogs = elasticLogE.ToList();
                }
            }
            else
            {
                if (searchResponse.ServerError?.Error?.Reason != null)
                {
                    _logger.LogError($"Ошибка поиска документов: {searchResponse.ServerError?.Error?.Reason}. {searchResponse.ServerError?.Error?.RootCause?.FirstOrDefault()?.Reason}");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Получение Elastic запроса с фильтрацией
        /// </summary>
        /// <param name="storeLoadParams"></param>
        /// <param name="storeParams"></param>
        /// <returns></returns>
        private Func<BoolQueryDescriptor<ElasticLog>, IBoolQuery> GetElasticBoolQuery(List<FilterColumn> filter)
        {
            var eqFilters = filter.Where(f => f.@operator == "eq");
            var gteFilters = filter.Where(f => f.@operator == "gte");
            var lteFilters = filter.Where(f => f.@operator == "lte");
            var neFilters = filter.Where(f => f.@operator == "ne");
            var inFilters = filter.Where(f => f.@operator == "in");
            var likeFilters = filter.Where(f => f.@operator == "like");
            var mustQueries = new List<Func<QueryContainerDescriptor<ElasticLog>, QueryContainer>>();
            var mustNotQueries = new List<Func<QueryContainerDescriptor<ElasticLog>, QueryContainer>>();

            foreach (FilterColumn filterColumn in eqFilters)
            {
                switch (filterColumn.property.ToLower())
                {
                    case "objectcreatedate":
                        DateTime objectcreatedate;

                        if (!DateTime.TryParseExact(filterColumn.value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out objectcreatedate))
                        {
                            throw new Exception($"Не верно указано значение для фильтра objectcreatedate = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateGreaterQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).GreaterThanOrEquals(objectcreatedate.Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateGreaterQuery);
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateLessQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).LessThanOrEquals(objectcreatedate.AddDays(1).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateLessQuery);
                        break;
                    case "entityid":
                        long longentityid;

                        if (!long.TryParse(filterColumn.value, out longentityid))
                        {
                            throw new Exception($"Не верно указано значение для фильтра EntityId = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entityidQuery = (q) => q.Term(f => f.EntityId, longentityid);
                        mustQueries.Add(entityidQuery);
                        break;
                    case "subdivisionid":
                        long longsubdivisionid;

                        if (!long.TryParse(filterColumn.value, out longsubdivisionid))
                        {
                            throw new Exception($"Не верно указано значение для фильтра SubdivisionId = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> subdivisionidQuery = (q) => q.Term(f => f.SubdivisionId, longsubdivisionid);
                        mustQueries.Add(subdivisionidQuery);
                        break;
                    case "entitytypecode":
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entitytypecodeQuery = (q) => q.Term(f => f.EntityTypeCode, filterColumn.value);
                        mustQueries.Add(entitytypecodeQuery);
                        break;
                    case "ids":
                        List<string> ids = filterColumn.value.Split(',').ToList();
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> idsQuery = (q) => q.Ids(s => s.Values(ids));
                        mustQueries.Add(idsQuery);
                        break;
                }
            }

            foreach (FilterColumn filterColumn in gteFilters)
            {
                switch (filterColumn.property.ToLower())
                {
                    case "objectcreatedate":
                        DateTime objectcreatedate;

                        if (!DateTime.TryParseExact(filterColumn.value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out objectcreatedate))
                        {
                            throw new Exception($"Не верно указано значение для фильтра objectcreatedate = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).GreaterThanOrEquals(objectcreatedate.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateQuery);
                        break;
                    case "entityid":
                        long longentityid;

                        if (!long.TryParse(filterColumn.value, out longentityid))
                        {
                            throw new Exception($"Не верно указано значение для фильтра EntityId = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entityidQuery = (q) => q.LongRange(t => t.Field(f => f.EntityId).GreaterThanOrEquals(longentityid));
                        mustQueries.Add(entityidQuery);
                        break;
                }
            }

            foreach (FilterColumn filterColumn in lteFilters)
            {
                switch (filterColumn.property.ToLower())
                {
                    case "objectcreatedate":
                        DateTime objectcreatedate;

                        if (!DateTime.TryParseExact(filterColumn.value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out objectcreatedate))
                        {
                            throw new Exception($"Не верно указано значение для фильтра objectcreatedate = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).LessThanOrEquals(objectcreatedate.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateQuery);
                        break;
                    case "entityid":
                        long longentityid;

                        if (!long.TryParse(filterColumn.value, out longentityid))
                        {
                            throw new Exception($"Не верно указано значение для фильтра EntityId = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entityidQuery = (q) => q.LongRange(t => t.Field(f => f.EntityId).LessThanOrEquals(longentityid));
                        mustQueries.Add(entityidQuery);
                        break;
                }
            }

            foreach (FilterColumn filterColumn in neFilters)
            {
                switch (filterColumn.property.ToLower())
                {
                    case "objectcreatedate":
                        DateTime objectcreatedate;

                        if (!DateTime.TryParseExact(filterColumn.value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out objectcreatedate))
                        {
                            throw new Exception($"Не верно указано значение для фильтра objectcreatedate = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateGreaterQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).LessThan(objectcreatedate.Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateGreaterQuery);
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> objectcreatedateLessQuery = (q) => q
                            .DateRange(d => d.Field(f => f.ObjectCreateDateString).GreaterThan(objectcreatedate.AddDays(1).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")));
                        mustQueries.Add(objectcreatedateLessQuery);
                        break;
                    case "entityid":
                        long longentityid;

                        if (!long.TryParse(filterColumn.value, out longentityid))
                        {
                            throw new Exception($"Не верно указано значение для фильтра EntityId = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entityidQuery = (q) => q.Term(f => f.EntityId, longentityid);
                        mustNotQueries.Add(entityidQuery);
                        break;
                    case "actiontype":
                        int intactiontype;

                        if (!int.TryParse(filterColumn.value, out intactiontype))
                        {
                            throw new Exception($"Не верно указано значение для фильтра actiontype = {filterColumn.value}");
                        }

                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> actiontypeQuery = (q) => q.Term(f => f.ActionType, intactiontype);
                        mustNotQueries.Add(actiontypeQuery);
                        break;
                }
            }

            foreach (FilterColumn filterColumn in inFilters)
            {
                if (filterColumn.property.ToLower() == "actiontype")
                {
                    long longactiontype;

                    if (!long.TryParse(filterColumn.value, out longactiontype))
                    {
                        throw new Exception($"Не верно указано значение для фильтра actiontype = {filterColumn.value}");
                    }

                    Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> actiontypeQuery = (q) => q.Term(f => f.ActionType, longactiontype);
                    mustQueries.Add(actiontypeQuery);
                }
            }

            foreach (FilterColumn filterColumn in likeFilters)
            {
                switch (filterColumn.property.ToLower())
                {
                    case "entitytype":
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> entitytypeQuery = (q) => q
                            .Match(q => q.Field(f => f.EntityType).Query(filterColumn.value).Fuzziness(Fuzziness.EditDistance(1)));
                        mustQueries.Add(entitytypeQuery);
                        break;
                    case "userlogin":
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> userloginQuery = (q) => q
                            .Match(q => q.Field(f => f.UserLogin).Query(filterColumn.value));
                        mustQueries.Add(userloginQuery);
                        break;
                    case "operator":
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> operatorQuery = (q) => q
                            .Match(q => q.Field(f => f.Operator).Query(filterColumn.value));
                        mustQueries.Add(operatorQuery);
                        break;
                    case "buisnescomment":
                        Func<QueryContainerDescriptor<ElasticLog>, QueryContainer> busnescommentQuery = (q) => q
                            .Match(q => q.Field(f => f.BuisnesComment).Query(filterColumn.value).Fuzziness(Fuzziness.EditDistance(1)));
                        mustQueries.Add(busnescommentQuery);
                        break;
                }
            }

            Func<BoolQueryDescriptor<ElasticLog>, IBoolQuery> boolQuery = null;

            if (mustQueries.Count > 0 && mustNotQueries.Count > 0)
            {
                boolQuery = (bq) => bq.Must(mustQueries).MustNot(mustNotQueries);
            }
            else if (mustQueries.Count > 0 && mustNotQueries.Count == 0)
            {
                boolQuery = (bq) => bq.Must(mustQueries);
            }
            else if (mustQueries.Count == 0 && mustNotQueries.Count > 0)
            {
                boolQuery = (bq) => bq.MustNot(mustNotQueries);
            }

            return boolQuery;
        }

        /// <summary>
        /// Получение Elastic запроса с фильтрацией в индекс log
        /// </summary>
        /// <param name="storeLoadParams"></param>
        /// <param name="storeParams"></param>
        /// <returns></returns>
        private Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> GetLogRawDataFilteredQuery(List<FilterColumn> filter)
        {
            Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> querySelector;
            var eqFilters = filter.Where(f => f.@operator == "eq");
            var likeFilters = filter.Where(f => f.@operator == "like");
            var mustQueries = new List<Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer>>();

            foreach (FilterColumn filterColumn in eqFilters)
            {
                if (filterColumn.property == "LogId")
                {
                    Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> logIdQuery = (q) => q.Term(t => t.LogId, filterColumn.value);
                    mustQueries.Add(logIdQuery);
                }
                else
                {
                    Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> entitytypeQuery = (q) =>
                        q.Nested(n => n.Path(p => p.JsonData).Query(q1 => q1.Term(t => t.JsonData[filterColumn.property].Value.Name, filterColumn.value)));
                    mustQueries.Add(entitytypeQuery);
                }
            }

            foreach (FilterColumn filterColumn in likeFilters)
            {
                Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> entitytypeQuery = (q) => q.Nested(n => n
                    .Path(p => p.JsonData)
                    .Query(q1 => q1.Match(m => m.Field(f => f.JsonData[filterColumn.property].Value.Name).Query(filterColumn.value).Fuzziness(Fuzziness.EditDistance(1)))));
            }

            if (mustQueries.Count > 0)
            {
                Func<BoolQueryDescriptor<ElasticLogRawData>, IBoolQuery> boolQuery = (bq) => bq.Must(mustQueries);
                querySelector = (q) => q.Bool(boolQuery);
            }
            else
            {
                querySelector = (q) => q.MatchAll();
            }

            return querySelector;
        }

        /// <summary>
        /// Получение предыдущего обобщенного лога записи
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private ElasticLog GetOldLog(ElasticLog log)
        {
            ElasticLog oldLog;
            var searchOldLogSearchDescripter = new SearchDescriptor<ElasticLog>()
                    .Index(LogIndexName)
                    .Size(1)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m.Term(t => t.EntityId, log.EntityId)
                                && m.Term(t => t.EntityTypeCode, log.EntityTypeCode)
                                && m.DateRange(r => r.Field(f => f.ObjectCreateDateString).LessThan(log.ObjectCreateDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fff"))))
                            .MustNot(mn => mn.Term(t => t.Id, log.Id))
                        )
                    )
                    .Sort(s => s.Field(f => f.ObjectCreateDateString, SortOrder.Descending));

            TryGetElasticDocument(searchOldLogSearchDescripter, out oldLog);

            return oldLog;
        }

        /// <summary>
        /// Получение истории договора для клиента
        /// </summary>
        /// <param name="commoneLog"></param>
        /// <returns></returns>
        private ContractLogResponseDto GetContractLogResponseDto(ElasticLog commoneLog, List<FilterColumn> requestFilter)
        {
            var contractLogResponseDto = new ContractLogResponseDto
            {
                BuisnesComment = commoneLog.BuisnesComment,
                LogId = commoneLog.Id,
                SaveDateTime = commoneLog.ObjectCreateDate,
                ActionType = commoneLog.ActionType,
                RequestId = commoneLog.RequestId,
                Responsible = commoneLog.Operator,
                ChangeFields = new List<ContractLogMutation>()
            };
            var logDataFilter = new List<FilterColumn> { new FilterColumn {
                        @operator = "eq",
                        property = "LogId",
                        value = commoneLog.Id
                    } };
            string logDataIndexName = commoneLog.EntityTypeCode.ToLower();

            //todo: переделать фильтрацию по статусам с учетом логов связанных с договором сущностей (арендаторы), если понадобиться включить ее
            //if (logDataIndexName == ContractLogIndexName)
            //{
            //    var contractFilterFields = new List<string> { "Status" };

            //    foreach (FilterColumn requestFilterColumn in requestFilter)
            //    {
            //        if (contractFilterFields.Contains(requestFilterColumn.property))
            //        {
            //            logDataFilter.Add(requestFilterColumn);
            //        }
            //    }
            //}

            ElasticLogRawData logData = null;
            ElasticLogRawData contractLogData = null;
            Func<QueryContainerDescriptor<ElasticLogRawData>, QueryContainer> querySelector = GetLogRawDataFilteredQuery(logDataFilter);
            var logDataSearchDescripter = new SearchDescriptor<ElasticLogRawData>().Index(logDataIndexName).Size(1).Query(querySelector);

            if (TryGetElasticDocument(logDataSearchDescripter, out logData))
            {
                if (logDataIndexName == ContractLogIndexName)
                {
                    contractLogData = logData;
                    var exceptDtoFields = new List<string>() { "EntityVersion", "MetaId", "Id", "IsCurrent" };
                    var dateFields = new List<string>() { "Date", "TerminationDate", "ActionDateStart", "ActionDateEnd", "ReturnDate", "DateTransmission", "DateRentStart" };
                    List<ContractLogMutation> changedFields = GetChangedFields(commoneLog, logData.JsonData, exceptDtoFields, dateFields);
                    contractLogResponseDto.ChangeFields.AddRange(changedFields);
                }
                else
                {
                    contractLogResponseDto.ActionType = ElasticLogBuilder.ActionType.Edit;
                    string contractLogId = logData.JsonData["Contract"].Value.LogId;

                    if (!string.IsNullOrEmpty(contractLogId))
                    {
                        var contractLogDataSearchDescripter = new SearchDescriptor<ElasticLogRawData>().Index(ContractLogIndexName).Size(1).Query(q => q.Term(t => t.LogId, contractLogId));
                        TryGetElasticDocument(contractLogDataSearchDescripter, out contractLogData);
                    }

                    var elasticLogLinkValueResponseDto = new ElasticLogLinkValueResponseDto
                    {
                        Id = commoneLog.Id
                    };
                    var logMutation = new ContractLogMutation();

                    switch (logDataIndexName)
                    {
                        case ContractcontragentLogIndexName:
                            {
                                logMutation.PropertyName = "Арендаторы";
                                ElasticLogField newContragentField = null;
                                ElasticLogField newContragentAmountField = null;
                                ElasticLogField newContragentDateField = null;
                                logData.JsonData.TryGetValue("Contragent", out newContragentField);
                                logData.JsonData.TryGetValue("Amount", out newContragentAmountField);
                                string newContragentNameFieldString = newContragentField?.Value?.Name;
                                string newContragentAmountFieldString = newContragentAmountField?.Value?.Name;
                                string newContragentDateString = string.Empty;

                                if (logData.JsonData.TryGetValue("Date", out newContragentDateField))
                                {
                                    newContragentDateString = GetDateFromElasticLogField(newContragentDateField);
                                }

                                elasticLogLinkValueResponseDto.Name = $"{newContragentNameFieldString}, {newContragentAmountFieldString} р., {newContragentDateString}";

                                if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Edit)
                                {
                                    ElasticLog commonOldLog = GetOldLog(commoneLog);

                                    if (commonOldLog != null)
                                    {
                                        ElasticLogRawData oldLogData = GetLogRawData(commonOldLog);

                                        if (oldLogData != null)
                                        {
                                            ElasticLogField oldContragentField = null;
                                            ElasticLogField oldContragentAmountField = null;
                                            ElasticLogField oldContragentDateField = null;
                                            oldLogData.JsonData.TryGetValue("Contragent", out oldContragentField);
                                            oldLogData.JsonData.TryGetValue("Amount", out oldContragentAmountField);
                                            string oldContragentNameFieldString = oldContragentField?.Value?.Name;
                                            string oldContragentAmountFieldString = oldContragentAmountField?.Value?.Name;
                                            string oldContragentDateString = string.Empty;

                                            if (oldLogData.JsonData.TryGetValue("Date", out oldContragentDateField))
                                            {
                                                oldContragentDateString = GetDateFromElasticLogField(oldContragentDateField);
                                            }

                                            if (newContragentNameFieldString != oldContragentNameFieldString || newContragentAmountFieldString != oldContragentAmountFieldString
                                                || newContragentDateString != oldContragentDateString)
                                            {
                                                var elasticOldLogLinkValueResponseDto = new ElasticLogLinkValueResponseDto
                                                {
                                                    Id = commonOldLog.Id,
                                                    Name = $"{oldContragentNameFieldString}, {oldContragentAmountFieldString} р., {oldContragentDateString}"
                                                };
                                                logMutation.PropertyValueOld.Add(elasticOldLogLinkValueResponseDto);
                                            }
                                            else
                                            {
                                                logMutation.PropertyValueOld.Add(new ElasticLogLinkValueResponseDto
                                                {
                                                    Name = "Не найдены данные предыдущего лога"
                                                });
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case ContractdocumentownershipLogIndexName:
                            {
                                if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Edit)
                                {
                                    return null;
                                }

                                logMutation.PropertyName = "Объекты аренды";
                                ElasticLogField documentOwnershipField = null;
                                logData.JsonData.TryGetValue("DocumentOwnership", out documentOwnershipField);
                                elasticLogLinkValueResponseDto.Name = documentOwnershipField?.Value?.Name;
                                break;
                            }
                        case PennaltyAccrualPeriodLogIndexName:
                            {
                                logMutation.PropertyName = "Периоды начисления пени";
                                ElasticLogField newCommentField = null;
                                ElasticLogField newPennaltyAmountField = null;
                                ElasticLogField newPennaltyStartDateField = null;
                                logData.JsonData.TryGetValue("Comment", out newCommentField);
                                logData.JsonData.TryGetValue("PennaltyAmount", out newPennaltyAmountField);
                                string newCommentFieldString = newCommentField?.Value?.Name;
                                string newPennaltyAmountFieldString = newPennaltyAmountField?.Value?.Name;
                                string newPennaltyStartDateFieldString = string.Empty;

                                if (logData.JsonData.TryGetValue("PennaltyStartDate", out newPennaltyStartDateField))
                                {
                                    newPennaltyStartDateFieldString = GetDateFromElasticLogField(newPennaltyStartDateField);
                                }

                                elasticLogLinkValueResponseDto.Name = $"{newPennaltyAmountFieldString} р., {newPennaltyStartDateFieldString}, {newCommentFieldString}";

                                if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Edit)
                                {
                                    ElasticLog commonOldLog = GetOldLog(commoneLog);

                                    if (commonOldLog != null)
                                    {
                                        ElasticLogRawData oldLogData = GetLogRawData(commonOldLog);

                                        if (oldLogData != null)
                                        {
                                            ElasticLogField oldCommentField = null;
                                            ElasticLogField oldPennaltyAmountField = null;
                                            ElasticLogField oldPennaltyStartDateField = null;
                                            oldLogData.JsonData.TryGetValue("Comment", out oldCommentField);
                                            oldLogData.JsonData.TryGetValue("PennaltyAmount", out oldPennaltyAmountField);
                                            string oldCommentFieldString = oldCommentField?.Value?.Name;
                                            string oldPennaltyAmountFieldString = oldPennaltyAmountField?.Value?.Name;
                                            string oldPennaltyStartDateString = string.Empty;

                                            if (oldLogData.JsonData.TryGetValue("PennaltyStartDate", out oldPennaltyStartDateField))
                                            {
                                                oldPennaltyStartDateString = GetDateFromElasticLogField(oldPennaltyStartDateField);
                                            }

                                            if (newPennaltyAmountFieldString != oldPennaltyAmountFieldString || newPennaltyStartDateFieldString != oldPennaltyStartDateString
                                                || newCommentFieldString != oldCommentFieldString)
                                            {
                                                var elasticOldLogLinkValueResponseDto = new ElasticLogLinkValueResponseDto
                                                {
                                                    Id = commonOldLog.Id,
                                                    Name = $"{oldPennaltyAmountFieldString} р., {oldPennaltyStartDateString}, {oldCommentFieldString}"
                                                };
                                                logMutation.PropertyValueOld.Add(elasticOldLogLinkValueResponseDto);
                                            }
                                            else
                                            {
                                                logMutation.PropertyValueOld.Add(new ElasticLogLinkValueResponseDto
                                                {
                                                    Name = "Не найдены данные предыдущего лога"
                                                });
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                    }

                    switch (commoneLog.ActionType)
                    {
                        case ElasticLogBuilder.ActionType.Create:
                        case ElasticLogBuilder.ActionType.Edit:
                            logMutation.PropertyValueNew.Add(elasticLogLinkValueResponseDto);
                            break;
                        case ElasticLogBuilder.ActionType.Delete:
                            logMutation.PropertyValueOld.Add(elasticLogLinkValueResponseDto);
                            break;
                    }

                    contractLogResponseDto.ChangeFields.Add(logMutation);
                }
            }

            ElasticLogField contractStatusField;

            if (contractLogData != null && contractLogData.JsonData.TryGetValue("Status", out contractStatusField))
            {
                contractLogResponseDto.ContractStatus = contractStatusField.Value.Name;
            }

            if (contractLogResponseDto.ChangeFields.Count > 0)
            {
                return contractLogResponseDto;
            }

            return null;
        }

        /// <summary>
        /// Получение сокращенной даты (yyyy-MM-dd) из поля elastic
        /// </summary>
        /// <param name="dateField">Поле elastic</param>
        /// <returns></returns>
        private string GetDateFromElasticLogField(ElasticLogField dateField)
        {
            DateTime date;
            string dateString = string.Empty;

            if (DateTime.TryParse(dateField?.Value?.Name, out date))
            {
                dateString = date.ToString("yyyy-MM-dd");
            }

            return dateString;
        }

        /// <summary>
        /// Получение измененных полей лога
        /// </summary>
        /// <param name="commoneLog">Общий лог</param>
        /// <param name="jsonData">Данные лога</param>
        /// <param name="exceptDtoFields">Исключить поля</param>
        private List<ContractLogMutation> GetChangedFields(ElasticLog commoneLog, Dictionary<string, ElasticLogField> jsonData, List<string> exceptDtoFields, List<string> dateFields = null)
        {
            var changedFields = new List<ContractLogMutation>();
            ElasticLogRawData contractOldLogData = null;

            if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Edit)
            {
                ElasticLog contractCommonOldLog = GetOldLog(commoneLog);

                if (contractCommonOldLog != null)
                {
                    contractOldLogData = GetLogRawData(contractCommonOldLog);

                    if (contractOldLogData == null)
                    {
                        _logger.LogError($"Не найден лог c данными для общего лога {contractCommonOldLog.Id}");
                    }
                }
            }

            foreach (KeyValuePair<string, ElasticLogField> field in jsonData)
            {
                if (exceptDtoFields.Contains(field.Key))
                {
                    continue;
                }

                string newValueString = field.Value.Value.Name;// 

                if (dateFields != null && dateFields.Contains(field.Key))
                {
                    newValueString = GetDateFromElasticLogField(field.Value);
                }

                var contractLogMutation = new ContractLogMutation { PropertyName = field.Value.Name };
                var elasticLogLinkValueResponseDto = new ElasticLogLinkValueResponseDto { Name = newValueString };

                if (!string.IsNullOrEmpty(field.Value.Value.LogId))
                {
                    elasticLogLinkValueResponseDto.Id = field.Value.Value.LogId;
                }

                if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Delete)
                {
                    contractLogMutation.PropertyValueOld.Add(elasticLogLinkValueResponseDto);
                }
                else
                {
                    contractLogMutation.PropertyValueNew.Add(elasticLogLinkValueResponseDto);
                }

                if (commoneLog.ActionType == ElasticLogBuilder.ActionType.Edit && contractOldLogData != null)
                {
                    ElasticLogField oldDateField;

                    if (contractOldLogData.JsonData.TryGetValue(field.Key, out oldDateField))
                    {
                        string oldValueString = oldDateField.Value.Name;

                        if (dateFields != null && dateFields.Contains(field.Key))
                        {
                            oldValueString = GetDateFromElasticLogField(oldDateField);
                        }

                        if (oldDateField.Name != field.Value.Name || oldDateField.Value.Id != field.Value.Value.Id || oldValueString != newValueString)
                        {
                            contractLogMutation.PropertyValueOld.Add(new ElasticLogLinkValueResponseDto { Id = field.Value.Value.LogId, Name = oldValueString });
                        }
                        else 
                        {
                            continue;
                        }
                    }
                }

                changedFields.Add(contractLogMutation);
            }

            return changedFields;
        }

        /// <summary>
        /// Попытка получить данные лога по связи его ссылочного поля с id сущности
        /// </summary>
        /// <param name="indexName">Имя индекса</param>
        /// <param name="logLinkFieldName">Имя ссылочного поля</param>
        /// <param name="entityId">Ссылка на сущность</param>
        /// <param name="dataLogs">Логи данных</param>
        /// <returns></returns>
        private bool TryGetElasticLogRawDataByEntityIdLinkField(string indexName, string logLinkFieldName, long entityId, out IReadOnlyCollection<ElasticLogRawData> dataLogs)
        {
            var searchDescripter = new SearchDescriptor<ElasticLogRawData>().Index(indexName)
                    .Query(q => q.Nested(n => n.Path(p => p.JsonData).Query(q2 => q2.Term(t => t.JsonData[logLinkFieldName].Value.Id, entityId))));
            var searchResult = _client.Search<ElasticLogRawData>(s => searchDescripter);

            if (searchResult.IsValid)
            {
                dataLogs = searchResult.Documents;
                return true;
            }
            else
            {
                dataLogs = null;
            }

            return false;
        }

        /// <summary>
        /// Получение списка id общего лога связанных сущностей
        /// </summary>
        /// <param name="contractcontragentLogIndexName"></param>
        /// <param name="v"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        private List<string> GetRelatedEntityCommonLogIds(string dataLogIndexName, string refFieldName, long entityid)
        {
            //todo: учитывать storeParams.filter
            var commonLogIdList = new List<string>();
            IReadOnlyCollection<ElasticLogRawData> dataLogs;

            if (TryGetElasticLogRawDataByEntityIdLinkField(dataLogIndexName, refFieldName, entityid, out dataLogs))
            {
                foreach (ElasticLogRawData logData in dataLogs)
                {
                    if (!commonLogIdList.Contains(logData.LogId))
                    {
                        commonLogIdList.Add(logData.LogId);
                    }
                }
            }

            return commonLogIdList;
        }

        /// <summary>
        /// Получение данных лога
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private ElasticLogRawData GetLogRawData(ElasticLog log)
        {
            ElasticLogRawData logData;
            var logDataSearchDescripter = new SearchDescriptor<ElasticLogRawData>().Index(log.EntityTypeCode.ToLower()).Size(1).Query(q => q.Term(t => t.LogId, log.Id));
            TryGetElasticDocument(logDataSearchDescripter, out logData);
            return logData;
        }
        #endregion
    }
}
