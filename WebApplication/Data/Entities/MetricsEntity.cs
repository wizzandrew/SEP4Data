using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Data.Entities
{
    public class MetricsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MetricsID { get; set; }

        [Required]
        public double Humidity { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public double Noise { get; set; }

        [Required]
        public double CO2 { get; set; }

        public Nullable<DateTime> LastUpdated { get; set; }

        [ForeignKey("WeeklyStatisticsID")]
        public Nullable<int> W_S_ID { get; set; }

        public int ProductID { get; set; }
    }
}
