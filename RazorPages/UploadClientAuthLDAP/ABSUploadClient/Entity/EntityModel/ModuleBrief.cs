using System.ComponentModel.DataAnnotations;

namespace ABSUploadClient.Entity.EntityModel
{
    public class ModuleBrief
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Accout { get; set; }

        [MaxLength(100)]
        public string ModuleValue { get; set; }
    }
}
