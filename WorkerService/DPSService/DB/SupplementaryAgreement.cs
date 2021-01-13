using System;
using System.ComponentModel.DataAnnotations;

namespace DPSService.DB
{
    /// <summary>
    /// РЕЕСТР ДОП. СОГЛАШЕНИЙ
    /// </summary>
    public class SupplementaryAgreement : BaseEntity
    {
        /// <summary>
        /// Id договора
        /// </summary>
        public Guid ContractId { get; set; }
        /// <summary>
        /// Номер договора в системе АБС
        /// </summary>
        [MaxLength(50)]
        public string ContractNumberABS { get; set; }
        /// <summary>
        /// Дата начала кредитных каникул
        /// </summary>
        public DateTime GracePeriodStartDate { get; set; }

        /// <summary>
        /// Новая дата окончания займа
        /// </summary>
        public DateTime NewContractEndDate { get; set; }

    }
}
