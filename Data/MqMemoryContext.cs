using System.Collections.Generic;
using Echeers.Mq.Models;

namespace Echeers.Mq.Data
{
    public static class MqMemoryContext
    {
        public static List<Message> Messages { get; set; }
    }
}