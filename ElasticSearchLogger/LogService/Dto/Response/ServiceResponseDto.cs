namespace LogService.Dto
{
    /// <summary>
    /// Общий тип возвращаемого ответа сервису
    /// </summary>
    public class ServiceResponseDto
    {
        /// <summary>
        /// Успех обработки запроса
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Дополнительное сообщение клиенту
        /// </summary>
        public string Message { get; set; }
    }
}
