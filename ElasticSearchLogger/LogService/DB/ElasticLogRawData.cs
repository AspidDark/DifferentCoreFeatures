using Nest;
using System.Collections.Generic;

namespace LogService.DB
{
    /// <summary>
    /// Индекс с данными логируемых сущностей
    /// </summary>
    [ElasticsearchType]
    public class ElasticLogRawData: ElasticLogData<Dictionary<string, ElasticLogField>>
    {
    }
}
