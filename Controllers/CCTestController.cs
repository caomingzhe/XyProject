using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTCache.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace XyProject.Controllers
{
    [Route("api/CCTest/[action]")]
    [ApiController]
    public class CCTestController : ControllerBase
    {
        private readonly ICacheTarget _cache;

        public CCTestController(ICacheTarget cache)
        {
            _cache = cache;
        }


        [HttpGet]
        public object add(int id)
        {
            //测试提交
            _cache.Add("hellowword", "一起看世界");

            _cache.Replace("hellowword", "一起看世界22",new TimeSpan(0,10,0));

            return _cache.Get("hellowword");
        }
    }
}