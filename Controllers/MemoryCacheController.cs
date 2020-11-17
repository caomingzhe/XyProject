using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using XyProject.Contracts;

namespace XyProject.Controllers
{
    [Route("api/MemoryCache/[action]")]
    [ApiController]
    public class MemoryCacheController : ControllerBase
    {
        private readonly IMemoryCacheService _memoryCacheService;

        public MemoryCacheController(IMemoryCacheService memoryCacheService)
        {
            _memoryCacheService = memoryCacheService;
        }

        [HttpPost]
        public ActionResult<string> Set([FromForm]string key, [FromForm]string value)
        {
            if (_memoryCacheService.SetCache<string>(key, value))
            {
                return "success";
            }

            return "fail";
        }

        [HttpGet]
        public ActionResult<string> Get(string key)
        {
            string str = _memoryCacheService.GetCache<string>(key);

            return str;
        }

        [HttpPost]
        public ActionResult<string> GetOrSet([FromForm]string key, [FromForm]string value)
        {
            string str = _memoryCacheService.GetOrSetCache<string>(key, value);

            return str;
        }

        [HttpPost]
        public ActionResult<string> Remove([FromForm]string key)
        {
            if (_memoryCacheService.RemoveCache(key))
            {
                return "success";
            }

            return "fail";
        }
    }
}