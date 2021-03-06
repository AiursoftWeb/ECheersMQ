﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Echeers.Mq.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class MqUser : IdentityUser
    {
        [InverseProperty(nameof(App.Owner))]
        public IEnumerable<App> Apps { get; set; }
    }
}
