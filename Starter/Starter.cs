using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace P1_EDDll_AFPE_DAVH.Starter
{
    public class Starter
    {
        public HttpClient Start()
        {
            var Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44711");
            return Client;
        }
    }
}
