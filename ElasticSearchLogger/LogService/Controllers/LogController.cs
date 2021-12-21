using ElasticLogBuilder;
using LogService.Dto;
using LogService.Dto.Response;
using LogService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        const string PostLogError = "Ошибка записи логов";
        private readonly ILogger<LogController> _logger;
        private readonly Model.IDBLogger _DBLogger;

        public LogController(Model.IDBLogger DBLogger, ILogger<LogController> logger)
        {
            _logger = logger;
            _DBLogger = DBLogger;
        }

        /// <summary>
        /// Запись логов
        /// </summary>
        /// <param name="requestLogs"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseDto PostLog([FromBody] List<ElasticLogRequestDto> requestLogs)
        {
            try
            {
                if (requestLogs.Count > 0)
                {
                    string entityTypes = string.Join(", ", requestLogs.Select(r => $"{ r.DatabaseName}.{r.EntityType}"));
                    _logger.LogInformation($"Запрос на логирование объектов: {entityTypes}");
                    _DBLogger.Post(requestLogs);
                }
                else
                {
                    _logger.LogInformation($"Запрос на логирование объектов: 0");
                }

                return new ServiceResponseDto();
            }
            catch (Exception ex)
            {
                return new ServiceResponseDto { Success = false, Message = PostLogError };
            }
        }

        /// <summary>
        /// Получение логов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ClientResponseDto> List([FromQuery] BaseParamsDto baseParams)
        {
            try
            {
                ClientListResponseDto<LogResponseDto> data = await _DBLogger.GetList(baseParams);
                return data;
            }
            catch (Exception ex)
            {
                Exception error = ex.GetOriginalException();
                string message = $"Ошибка получения логов";
                _logger.LogError(error, message);
                return new ClientResponseDto { Success = false, Message = message };
            }
        }

        /// <summary>
        /// Получение логов договора
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        [HttpGet("GetContractLog")]
        public async Task<ClientResponseDto> GetContractLog([FromQuery] BaseParamsDto baseParams)
        {
            try
            {
                ClientListResponseDto<ContractLogResponseDto> data = await _DBLogger.GetContractLog(baseParams);
                return data;
            }
            catch (Exception ex)
            {
                Exception error = ex.GetOriginalException();
                string message = $"Ошибка получения логов договоров";
                _logger.LogError(error, message);
                return new ClientResponseDto { Success = false, Message = message };
            }
        }

        /// <summary>
        /// Получение истории лога
        /// </summary>
        /// <param name="baseGetParams"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ClientResponseDto> GetLog(string id)
        {
            try
            {
                ClientGetResponseDto<LogHistoryDto> data = await _DBLogger.Get(id);
                return data;
            }
            catch (Exception ex)
            {
                Exception error = ex.GetOriginalException();
                string message = $"Ошибка получения лога";
                _logger.LogError(error, message);
                return new ClientResponseDto { Success = false, Message = message };
            }
        }
    }
}
