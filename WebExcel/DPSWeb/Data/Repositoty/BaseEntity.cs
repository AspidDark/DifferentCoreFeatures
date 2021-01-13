using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPSWeb.Data.Repositoty
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }
    }
}
