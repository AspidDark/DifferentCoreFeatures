using System;
using System.Collections.Generic;
using System.Text;

namespace DPSService.DB
{
    public class ContractData :BaseEntity
    {
        public Guid ContractId { get; set; }

        public DateTime ContractClosedOn { get; set; }
    }
}
