using System;
using System.Collections.Generic;

#nullable disable

namespace NorthWind.Models
{
    public partial class Feedback
    {
        public int Fid { get; set; }
        public string Ftitle { get; set; }
        public string Fcontent { get; set; }
        public DateTime? Fdate { get; set; }
        public string CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
