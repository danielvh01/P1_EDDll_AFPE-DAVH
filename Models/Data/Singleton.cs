using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1_EDDll_AFPE_DAVH.Models.Data
{
    public class Singleton : Controller
    {
        private readonly static Singleton _instance = new Singleton();

        private Singleton()
        {

        }

        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
