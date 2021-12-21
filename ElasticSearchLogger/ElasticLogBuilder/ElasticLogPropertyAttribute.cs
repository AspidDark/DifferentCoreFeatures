using System;

namespace ElasticLogBuilder
{
    /// <summary>
    /// Атрибут указывает имя свойства для логируемого объекта, который ссылается на объект с помеченным свойством
    /// </summary>
    public class ElasticLogPropertyAttribute : Attribute
    {
        public string Property { get; set; }

        public ElasticLogPropertyAttribute()
        {

        }

        public ElasticLogPropertyAttribute(string property)
        {
            this.Property = property;
        }
    }
}
