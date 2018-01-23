using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echeers.Mq.Models
{
    public class App
    {
        public string Id { get; set; }
        public string Secret { get; set; }

        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public MqUser Owner { get; set; }

    }
}