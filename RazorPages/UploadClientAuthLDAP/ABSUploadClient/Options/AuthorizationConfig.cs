namespace ABSUploadClient.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// Конфигурация авторизации
    /// </summary>
    public class AuthorizationConfig
    {
        /// <summary>
        /// Список ролей, имеющих допуск к работе с загрузкой платежей
        /// </summary>
        public List<string> AllowedRoles { get; set; }
    }
}