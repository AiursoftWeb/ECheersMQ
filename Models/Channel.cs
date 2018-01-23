using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Echeers.Mq.Models
{
    public class Channel
    {

        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ConnectKey { get; set; }
        public DateTime CreateTime { get; set; }
        public TimeSpan LifeTime { get; set; }
        public string AppId { get; set; }
        [ForeignKey("AppId")]
        [JsonIgnore]
        public App App { get; set; }

        public bool IsAlive() => DateTime.Now < CreateTime + LifeTime;

    }
}