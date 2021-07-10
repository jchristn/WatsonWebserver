﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Test.Parameters
{
    [RoutePrefix("MyApi")]
    public class MyApiController : ApiControllerBase
    {
        [HttpGet]
        public async Task<MyClass> GetTest1(int x, int y)
        {
            return new MyClass()
            {
                X = x * y,
                Message = base.Context.Request.Url.Full
            };
        }

        [HttpGet]
        public async Task<MyClass> GetTest2(HttpContext ctx, int x, int y)
        {
            return new MyClass()
            {
                X = x * y,
                Message = ctx.Request.Url.Full
            };
        }

        [HttpPost]
        public async Task<MyClass> PostTest1(MyClass mc)
        {
            return new MyClass()
            {
                X = mc.X * 2,
                Message = Context.Request.Url.Full
            };
        }

        [HttpPost]
        public async Task<MyClass> PostTest2(HttpContext ctx, MyClass mc)
        {
            return new MyClass()
            {
                X = mc.X * 2,
                Message = ctx.Request.Url.Full
            };
        }

        [HttpPost]
        public async Task<MyClass> PostTest3(HttpContext ctx, MyClass mc, int multiplier)
        {
            return new MyClass()
            {
                X = mc.X * multiplier,
                Message = ctx.Request.Url.Full
            };
        }
    }
}
