using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupy.Extensions.Messages
{
    /// <summary>
    /// Extension methods for flash messages
    /// </summary>
    public static class FlashMessageExtensions
    {
        public static IActionResult Success(this IActionResult result, string body)
        {
            return Message(result, "success", body);
        }

        public static IActionResult Info(this IActionResult result, string body)
        {
            return Message(result, "info", body);
        }

        public static IActionResult Warning(this IActionResult result, string body)
        {
            return Message(result, "warning", body);
        }

        public static IActionResult Danger(this IActionResult result, string body)
        {
            return Message(result, "danger",  body);
        }

        private static IActionResult Message(IActionResult result, string type, string body)
        {
            return new FlashMessageResult(result, type, body);
        }
    }
}
