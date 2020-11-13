using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace XyProject.Controllers
{
    [Route("api/CCTest/[action]")]
    [ApiController]
    public class CCTestController : ControllerBase
    {
       
        [HttpGet]
        public void add(int id)
        {
            //测试提交

           
        }
    }
}