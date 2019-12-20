using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailCheckerService;
using Microsoft.AspNetCore.Mvc;

namespace EmailSerivceTest.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IEmailService _emailService;
        public ValuesController(IEmailService emailService)
        {
            this._emailService = emailService;
        }
        // GET api/values

        [HttpGet]
        [Route("GetEmails")]
        public object GetEmails()
        {
            this._emailService.TestCheckEmailAsync();
            return null;
        }

        [HttpGet]
        [Route("SendMail")]
        public object SendMail()
        {
            return null;
        }



        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
