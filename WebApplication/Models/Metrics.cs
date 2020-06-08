using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Models
{
    public class Metrics
    {
        public int MetricsID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public double Humidity { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public double Noise { get; set; }

        [Required]
        public double CO2 { get; set; }

        public DateTime LastUpdated { get; set; }

        public static Metrics GetMetricsFromEntity(MetricsEntity metricsEntity)
        {
            Metrics metrics;

            if (metricsEntity != null)
            {
                metrics = new Metrics
                {
                    MetricsID = metricsEntity.MetricsID,
                    ProductID = metricsEntity.ProductID,
                    Humidity = metricsEntity.Humidity,
                    Temperature = metricsEntity.Temperature,
                    Noise = metricsEntity.Noise,
                    CO2 = metricsEntity.CO2
                };

                if (metricsEntity.LastUpdated.HasValue)
                {
                    metrics.LastUpdated = (DateTime)metricsEntity.LastUpdated;
                }
                return metrics;
            }

            return null;
        }
    }
}
