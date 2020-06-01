using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Extensions.Messages
{
    public class FlashMessageResult : IActionResult
    {
        public IActionResult Result { get; }
        public string Type { get; }
        public string Body { get; }

        public FlashMessageResult(IActionResult result, string type, string body)
        {
            Result = result;
            Type = type;
            Body = body;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_message.type"] = Type;
            tempData["_message.body"] = Body;
            await Result.ExecuteResultAsync(context);
        }
    }
}
