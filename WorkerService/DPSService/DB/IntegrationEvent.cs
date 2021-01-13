using System;
using System.Collections.Generic;
using System.Text;

namespace DPSService.DB
{
    public class IntegrationEvent : BaseEntity
    {
        public DateTime  ItegarionDate { get; set; }

        public int Complete { get; set; }
    }
}
