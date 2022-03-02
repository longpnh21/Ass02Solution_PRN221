using System;
using System.Collections.Generic;

#nullable disable

namespace NorthWind.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int? Type { get; set; }
    }
}
