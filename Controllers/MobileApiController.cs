﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using maddweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace maddweb.Controllers
{
    [Route("api/mobile")]
    public class MobileApiController : Controller
    {
        // GET: api/<controller>
        [HttpGet("GetPastWeekCases")]
        public IActionResult GetPastWeekCases()
        {
            
            var url = $"https://api.smartable.ai/coronavirus/stats/SG?Subscription-Key=3009d4ccc29e4808af1ccc25c69b4d5d";
            /*
            var httpClient = HttpClientFactory.Create();
            HttpResponseMessage data = await httpClient.GetAsync(url);
            String rawResponse = await data.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<Cases>(rawResponse);
            */
            dynamic data = WebUtl.CallWebApi(url);

            if (data != null)
            {
                dynamic totalStats = data.stats.history;
                List<dynamic> recentWeekCases = new List<dynamic>();

                //Return previous 7 days COVID-19 Cases
                for(int i = 7; i > 0; i--)
                {
                    int currentConfirmed = totalStats[totalStats.Count - i].confirmed;
                    int previousConfirmed = totalStats[totalStats.Count - (i + 1)].confirmed;
                    int diff = currentConfirmed - previousConfirmed;

                    DateTime dt = Convert.ToDateTime(totalStats[totalStats.Count - i].date);
                    string date = String.Format("{0:d/MMM}", dt);

                    var result = new
                    {
                        date,
                        cases = diff
                    };
                    recentWeekCases.Add(result);
                }
                return Ok(recentWeekCases);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetMonthlyCases")]
        public IActionResult GetMonthlyCases()
        {
            var url = $"https://api.smartable.ai/coronavirus/stats/SG?Subscription-Key=3009d4ccc29e4808af1ccc25c69b4d5d";

            dynamic data = WebUtl.CallWebApi(url);

            if (data != null)
            {
                dynamic totalStats = data.stats.history;
                List<dynamic> monthlyCases = new List<dynamic>();

                for(int i = 0; i < totalStats.Count; i++)
                {
                    //Format Date of JSON Result
                    DateTime dt = Convert.ToDateTime(totalStats[i].date);
                    string newDate = String.Format("{0:d/M/yyyy}", dt);

                    string[] currentDate = newDate.Split("/");

                    int confirmedCases = totalStats[i].confirmed;

                    int recoveredCases = totalStats[i].recovered;


                    //Check if Day = 01 for each Month
                    if (currentDate[0].Equals("1"))
                    {
                        var result = new
                        {
                            month = Int32.Parse(currentDate[1]) - 1,
                            confirmedCases,
                            recoveredCases
                        };
                        monthlyCases.Add(result);
                    }
                    
                }


                return Ok(monthlyCases);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetTop20News")]
        public IActionResult GetTop20News()
        {
            var url = $"https://api.smartable.ai/coronavirus/news/SG?Subscription-Key=3009d4ccc29e4808af1ccc25c69b4d5d";

            dynamic data = WebUtl.CallWebApi(url);
            List<dynamic> topNews = new List<dynamic>();

            if (data != null)
            {
                dynamic totalNews = data.news;
                for(int i = 0; i < 20; i++)
                {
                    string imageLink = "";
                    try
                    {
                        imageLink = totalNews[i].images[0].url;
                    }
                    catch
                    {
                        imageLink = "NA";
                    }

                    string title = totalNews[i].title;
                    string link = totalNews[i].webUrl;

                    var result = new
                    {
                        title,
                        link,
                        by = imageLink
                    };

                    topNews.Add(result);
                }
                return Ok(topNews);
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
