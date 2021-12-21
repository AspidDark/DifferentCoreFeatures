using ElasticLogBuilder;
using LogService.Dto;
using LogService.Dto.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogService.Model
{
    /// <summary>
    /// Работа с логами
    /// </summary>
    public interface IDBLogger
    {
        /// <summary>
        /// Запись логов
        /// </summary>
        /// <param name="requestLogs"></param>
        void Post(List<ElasticLogRequestDto> requestLogs);

        /// <summary>
        /// Получение списка логов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        Task<ClientListResponseDto<LogResponseDto>> GetList(BaseParamsDto baseParams);

        /// <summary>
        /// Получение истории лога
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ClientGetResponseDto<LogHistoryDto>> Get(string id);

        /// <summary>
        /// Получение истории договора
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        Task<ClientListResponseDto<ContractLogResponseDto>> GetContractLog(BaseParamsDto baseParams);
    }
}
