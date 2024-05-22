using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Services
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var response = context.HttpContext.Response;

            
            response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            response.Headers["Expires"] = "-1";
            response.Headers["Pragma"] = "no-cache";

            
            base.OnResultExecuting(context);
        }
    }
}

