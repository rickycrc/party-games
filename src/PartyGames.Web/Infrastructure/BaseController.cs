using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using PartyGames.Web.Models.Common;

namespace PartyGames.Web.Infrastructure
{
    public class BaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (behavior == JsonRequestBehavior.DenyGet
                && string.Equals(Request.HttpMethod, "GET",
                    StringComparison.OrdinalIgnoreCase))
                //Call JsonResult to throw the same exception as JsonResult
                return new JsonResult();

            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected ActionResult AjaxSuccess(string message, string redirect = null)
        {
            return Json(new
            {
                Success = true,
                Message = message,
                Redirect = redirect
            }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult AjaxError(string message, string redirect = null)
        {
            return Json(new
            {
                Success = false,
                Message = message,
                Redirect = redirect
            }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult AjaxError(List<string> messages, string redirect = null)
        {
            return Json(new
            {
                Success = false,
                Messages = messages,
                Redirect = redirect
            }, JsonRequestBehavior.AllowGet);
        }

        protected void AddPanelMessage(PanelMessageModel messageModel)
        {
            var panelMessages = TempData["panel_messages"] as List<PanelMessageModel> ?? new List<PanelMessageModel>();
            panelMessages.Add(messageModel);

            TempData["panel_messages"] = panelMessages;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var exception = filterContext.Exception;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary(filterContext.Controller.ViewData)
                {
                    Model = exception
                }
            };

            filterContext.ExceptionHandled = true;
        }
    }
}