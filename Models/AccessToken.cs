using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Echeers.Mq.Models
{
    public class AccessToken
    {
        [Key]
        public virtual int AccessTokenId { get; set; }
        public virtual string Value { get; set; }
        public virtual string ApplyAppId { get; set; }

        public virtual DateTime CreateTime { get; set; } = DateTime.Now;
        public virtual TimeSpan AliveTime { get; set; } = new TimeSpan(0, 20, 0);
        public virtual bool IsAlive => DateTime.Now  < CreateTime + AliveTime;
    }
}