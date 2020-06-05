using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ApplicationRepository repository;

        public MetricsController(ApplicationRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<Metrics>> onGet([FromQuery] int productID)
        {

            if (productID > 0)
            {
                MetricsEntity me = await repository.getLastUpdatedMetrics(productID);

                Metrics metrics;

                if (me != null)
                {

                    metrics = Metrics.getMetricsFromEntity(me);
                    return Ok(metrics);
                }
                else
                {
                    return NotFound("There is no metrics for such productId");
                }
            }

            else
            {
                return NotFound("productId not applicable");
            }

        }

        [HttpPost("sql")]

        public async Task<ActionResult> onPostNotification([FromQuery] int productID, [FromQuery] string token)
        {
            if (productID > 0 && token != null)
            {
                Metrics metrics;

                ActionResult<Metrics> result = await onGet(productID);

                if (result.Result == Ok())
                {
                    //response from server in string format
                    string responseFromServer = "";

                    //getting value for metrics object to use its attributes for post request body
                    metrics = result.Value;

                    WebRequest webRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    webRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server
                    webRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAcs7khsM:APA91bFFRRvujAgXnb2JTQKfD0QBLCwGPlfQm4bUaMm6TY7kqrhXwq0ik-Lst-KCMItoqIadL8Z_lHlQKvq32wonTWFpVL9_qW00Egt1gwWcyYG3GWaXrraBwoUyfhLHlAbJEpxdjwty"));
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
                        to = "doS5bgkJRFSm7Mg8tY_LOr:APA91bGmrsTdWsKa8Aeq2MzCpB3d46MVtdgqpD7BliDjepzJG3iiYRx0uQcRSU3wy8UsO00ALoIrrzSrmUsNnBwnTkdlaCES1RpS4XB5Qly1qOrPRTfzJ6bBk9SXfWOj3iZnN1CBT2UC",
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
                                        //result.Response = sResponseFromServer;
                                    }
                            }
                        }
                    }

                    return Ok(responseFromServer);

                    //creating http post request to android endpoint
                    //metrics = result.Value;

                    /*var client = new HttpClient();
                    StringContent httpContent = new StringContent(
                        $"{ 'data': { 'MetricsID': '','ProductID': , 'Humidity' : ,'Temperature' : , 'CO2' : ,'Noise' : ,'LastUpdated' : },'to' : 'TOKEN','direct_book_ok' : true }",
                        Encoding.UTF8,
                        "application/json"
                        );*/

                }
                return result.Result;
            }
            return BadRequest("productId or token not applicable");

        }


    }
}