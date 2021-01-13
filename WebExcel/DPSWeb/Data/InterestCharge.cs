using DPSWeb.Data.Repositoty;
using System;
using System.ComponentModel.DataAnnotations;

namespace DPSWeb.Data
{
    /// <summary>
    /// РЕЕСТР НАЧИСЛЕННЫХ %
    /// </summary>
    public class InterestCharge : BaseEntity
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
        /// Тип
        /// </summary>
        public int ContractType { get; set; }
        /// <summary>
        /// Вид
        /// </summary>
        public int ContractKind { get; set; }
        /// <summary>
        /// Сумма начисленных процентов по кредитным каникулам по договору за отчетный месяц 
        /// </summary>
        public decimal ContractAmount { get; set; }
        /// <summary>
        /// Последний календарный день месяца, за который грузим отчет
        /// </summary>
        public DateTime LastDayOfMonth { get; set; }
    }
}
