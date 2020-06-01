using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OBPostupy.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrisController : ControllerBase
    {
        private string url = "https://oris.orientacnisporty.cz/API/";
        [HttpGet]
        public async Task<string> Get(int? id, string date)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response = null;
                if (date != null)
                {
                    response = await client.GetAsync("?format=json&method=getEventList&datefrom=" + date + "&dateto=" + date);
                }
                else
                {
                    response = await client.GetAsync("?format=json&method=getEvent&id=" + id);
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }

                return null;
            }
        }

    }
}
