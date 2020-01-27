using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQBase
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public Message(int id, string name, DateTime dateTime)
        {
            Id = id;
            Name = name;
            CreateDate = dateTime;
        }
    }
}
