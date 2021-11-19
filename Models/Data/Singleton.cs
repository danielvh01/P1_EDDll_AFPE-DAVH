using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P1_EDDll_AFPE_DAVH.Starter;
using API_DataTransfer.Data;

using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace P1_EDDll_AFPE_DAVH.Models.Data
{
    public class Singleton : Controller
    {
        private readonly static Singleton _instance = new Singleton();
                
        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
