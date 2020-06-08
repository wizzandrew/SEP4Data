using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Models
{
    public class User
    {
        public string UserID { get; set; }

        public string Token { get; set; }

        public int ProductID { get; set; }
        
    }
}
