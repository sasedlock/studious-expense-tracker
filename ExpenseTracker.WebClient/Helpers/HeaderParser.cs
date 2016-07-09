using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;

namespace ExpenseTracker.WebClient.Helpers
{
    public class HeaderParser
    {
        public static PagingInfo FindAndParsePagingInfo(HttpResponseHeaders responseHeaders)
        {
            if (responseHeaders.Contains("X-Pagination"))
            {
                var xPag = responseHeaders.FirstOrDefault(ph => ph.Key == "X-Pagination").Value;

                return JsonConvert.DeserializeObject<PagingInfo>(xPag.FirstOrDefault());
            }

            return null;
        }
    }
}