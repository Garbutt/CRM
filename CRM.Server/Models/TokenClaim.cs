using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Server.Models
{
    public class TokenClaim
    {
        public string email { get; set; }

        public string role { get; set; }
    }
}