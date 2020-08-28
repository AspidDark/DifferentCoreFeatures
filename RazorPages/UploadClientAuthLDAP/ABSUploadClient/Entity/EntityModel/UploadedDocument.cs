using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABSUploadClient.Entity.EntityModel
{
    public class UploadedDocument
    {
        public int Id { get; set; }
        /// <summary>
        /// Вермя загрузки документа
        /// </summary>
        public DateTime UploadTime { get; set; }
        /// <summary>
        /// Имя оператора
        /// </summary>
        [MaxLength(100)]
        public string UserName { get; set; }
        /// <summary>
        /// Код источника
        /// </summary>
        [MaxLength(100)]
        public string SourceValue { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        [MaxLength(500)]
        public string FileName { get; set; }
        /// <summary>
        /// Имя сохраненного файла 
        /// </summary>
        [MaxLength(500)]
        public string BackupFileName { get; set; }

    }
}
