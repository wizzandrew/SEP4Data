using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        public readonly IApplicationRepository repository;
        public readonly string firebaseKey;

        public MetricsController(IApplicationRepository repository)
        {
            this.repository = repository;
            firebaseKey = "AAAAcs7khsM:APA91bFFRRvujAgXnb2JTQKfD0QBLCwGPlfQm4bUaMm6TY7kqrhXwq0ik-Lst-KCMItoqIadL8Z_lHlQKvq32wonTWFpVL9_qW00Egt1gwWcyYG3GWaXrraBwoUyfhLHlAbJEpxdjwty";
        }

        /// <summary>
        /// Get last updated metrics soecifying product Id
        /// </summary>
        /// <param name="productID">Id of the product</param>
        /// <returns>Metrics object or status code</returns>
        [ProducesResponseType(typeof(Metrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<Metrics>> onGet([FromQuery] int productID)
        {
            MetricsEntity me;

            if (productID > 0)
            {
                try
                {
                    me = await repository.getLastUpdatedMetrics(productID);

                    Metrics metrics;

                    if (me != null)
                    {
                        metrics = Metrics.getMetricsFromEntity(me);
                        return metrics;
                    }
                    else
                    {
                        return NotFound("There is no metrics for such productId");
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return StatusCode(500, "Internal server error");
                }
            }

            else
            {
                return NotFound("ProductId not applicable");
            }
        }


        /// <summary>
        /// Posts notification to google firebase with metrics object specifying metrics Id and token
        /// </summary>
        /// <param name="metricsID">Id of the metrics</param>
        /// <param name="token">Token given to user during registration in google firebase</param>
        /// <returns>status code</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("sql")]

        public async Task<ActionResult> onPostNotification([FromQuery] int metricsID, [FromQuery] string token)
        {
            if (metricsID > 0 && token != null)
            {
                Metrics metrics;

                try
                {
                    var result = await repository.getMetricsById(metricsID);

                    if (result!= null)
                    {
                        //response from server in string format
                        string responseFromServer = "No response from Android firebase cloud storage";

                        //getting value for metrics object to use its attributes for post request body
                        metrics = Metrics.getMetricsFromEntity(result);

                        WebRequest webRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                        webRequest.Method = "post";
                        //serverKey - Key from Firebase cloud messaging server
                        webRequest.Headers.Add(string.Format("Authorization: key={0}", firebaseKey));
                        //Sender Id - From firebase project setting
                        //webRequest.Headers.Add(string.Format(Sender: id ={ 0} ", "XXXXX..));
                        webRequest.ContentType = "application/json";
                        var payload = new
                        {
                            data = new
                            {
                                MetricsID = metrics.MetricsID,
                                ProductID = metrics.ProductID,
                                Humidity = metrics.Humidity,
                                Temperature = metrics.Temperature,
                                CO2 = metrics.CO2,
                                Noise = metrics.Noise,
                                LastUpdated = metrics.LastUpdated
                            },
                            to = token,
                            direct_book_ok = true
                        };

                        string postBody = JsonConvert.SerializeObject(payload).ToString();
                        Byte[] byteArray = Encoding.UTF8.GetBytes(postBody);
                        webRequest.ContentLength = byteArray.Length;
                        using (Stream dataStream = webRequest.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);

                            using (WebResponse webResponse = webRequest.GetResponse())
                            {
                                using (Stream dataStreamResponse = webResponse.GetResponseStream())
                                {
                                    if (dataStreamResponse != null) using (StreamReader reader = new StreamReader(dataStreamResponse))
                                        {
                                            responseFromServer = reader.ReadToEnd().ToString();
                                        }
                                }

                            }
                        }
                        return Ok(responseFromServer);

                    }

                    return BadRequest("Metrics can not be found for provided productId");
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return StatusCode(500, "Internal server error");
                }
            }

            return BadRequest("ProductId or Token not applicable");

        }

    }
}