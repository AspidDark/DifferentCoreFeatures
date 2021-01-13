using System;
using System.ComponentModel.DataAnnotations;

namespace DPSService.DB
{
    public class CalculationResult : BaseEntity
    {
        /// <summary>
        /// Номер займа 
        /// </summary>
        public Guid ContractId { get; set; } //AccountNumber
        /// <summary>
        /// Индентификатор займа из АБС 
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// Дата последней выплаты. Дата последнего произведенного платежа клиентом
        /// </summary>
        public DateTime DateAccountOpened { get; set; }
        /// <summary>
        /// Дата последней выплаты. Дата последнего произведенного платежа клиентом
        /// </summary>
        public DateTime DateOfLastPayment { get; set; }
        /// <summary>
        /// Состояние счёта
        /// </summary>
         [MaxLength(2)]
        public string AccountRating { get; set; }
        /// <summary>
        /// Дата состояния счёта
        /// </summary>
        public DateTime DateAccountRating { get; set; }
        /// <summary>
        /// Исходная сумма кредита
        /// </summary>
        public decimal ContractAmount { get; set; }
        /// <summary>
        /// Баланс
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Просрочка
        /// </summary>
        public decimal PastDue { get; set; }
        /// <summary>
        /// Следующий платеж
        /// </summary>
        public decimal NextPaymentAmount { get; set; }
        /// <summary>
        /// Своевременность платежей
        /// </summary>
        [MaxLength(10)]
        public string MOP { get; set; }
        /// <summary>
        /// Дата окончания срока договора
        /// </summary>
        public DateTime DateofContractTermination { get; set; }
        /// <summary>
        /// Дата финального платежа
        /// </summary>
        public DateTime DatePaymentDue { get; set; }
        /// <summary>
        /// Дата финальной выплаты процентов
        /// </summary>
        public DateTime DateInterestPaymentDue { get; set; }
        /// <summary>
        /// Текущая задолженность
        /// </summary>
        public decimal AmountOutstanding { get; set; }
        /// <summary>
        /// Дата фактического исполнения обязательств в полном объеме
        /// </summary>
        public DateTime CompleteObligationsDate { get; set; }
        /// <summary>
        /// Срочная задолженность по основному долгу на дату последнего платежа
        /// </summary>
        public string PrincipalAmountOutstanding { get; set; }
        /// <summary>
        /// Срочная задолженность по процентам на дату последнего платежа
        /// </summary>
        public string InterestAmountOutstanding { get; set; }
        /// <summary>
        /// Сумма подлежащих уплате комиссий и иных аналогичных требований к заемщику в составе срочной задолженности на дату последнего платежа
        /// </summary>
        public string OtherAmountOutstanding { get; set; }
        /// <summary>
        /// Просроченная задолженность по основному долгу на дату последнего платежа
        /// </summary>
        public string PrincipalAmountPastDue { get; set; }
        /// <summary>
        /// Просроченная задолженность по процентам на дату последнего платежа
        /// </summary>
        public string InterestAmountPastDue { get; set; }
        /// <summary>
        /// Сумма просроченных комиссий, а также сумма неустойки (штрафы, пени) и иных аналогичных требований к заемщику на дату последнего платежа
        /// </summary>
        public string OtherAmountPastDue { get; set; }
        /// <summary>
        /// Дата начала льготного периода
        /// </summary>
        public DateTime GracePeriodStartDate { get; set; }
        /// <summary>
        /// Дата окончания льготного периода
        /// </summary>
        public DateTime GracePeriodEndDate { get; set; }
        /// <summary>
        /// Дата неподтверждения/неустановления льготного периода
        /// </summary>
        public DateTime DateGracePeriodDeclined { get; set; }
        /// <summary>
        /// Основание установления льготного периода 
        /// </summary>
        [MaxLength(10)]
        public string GracePeriodReason { get; set; }
        /// <summary>
        /// Дата последнего пропущенного платежа
        /// </summary>
        
        public DateTime CreditCredDateMissedpayout { get; set; }
        /// <summary>
        /// Сумма фактического исполнения обязательств субъектом КИ в неполном размере (последний платеж) по основному долгу
        /// </summary>
        public string OpCredSumPayout { get; set; }
        /// <summary>
        /// Общая сумма, заплаченная субъектом КИ по основному долгу
        /// </summary>
        public decimal OpCredSumPaid { get; set; }
        /// <summary>
        /// Дата возникновения текущей просроченной задолженности по основному долгу
        /// </summary>
        public DateTime OpCredDateOverdue { get; set; }
        /// <summary>
        /// Сумма следующего платежа по основному долгу
        /// </summary>
        public string OpCredSumNextpayout { get; set; }
        /// <summary>
        /// Дата следующего платежа
        /// </summary>
        public DateTime OpCredDateNextpayout { get; set; }
        /// <summary>
        /// Сумма фактического исполнения обязательств субъектом КИ в неполном размере (последний платеж)
        /// </summary>
        public decimal TaCredSumPayout { get; set; }
    }
}
