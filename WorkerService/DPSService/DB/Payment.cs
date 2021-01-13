using System;

namespace DPSService.DB
{
    public class Payment : BaseEntity
    {
        //[Key]
        //public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ContractId { get; set; }
        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата платежа
        /// </summary>
        public DateTime Date { get; set; }


        /// <summary>
        /// Признак пролонгации
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }


        public DateTime ModifiedOn { get; set; }

    }
}
