using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NT_AI_Service.Controllers
{
    public class AiAdviceController : ApiController
    {
        // GET api/AiAdvice?price=0.7
        public string Get(decimal price)
        {
            return price < 0.6750m ? "enter long" : (price > 0.6785m ? "enter short" : "hold");
        }
    }
}
