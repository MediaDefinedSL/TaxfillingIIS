using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TaxFiling.Web.Helpers
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewToStringAsync<TModel>(
            this Controller controller,
            string viewPathName,
            TModel model)
        {
            if (string.IsNullOrEmpty(viewPathName))
            {
                viewPathName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using var writer = new StringWriter();
            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            ViewEngineResult viewResult;

            if (viewPathName.EndsWith(".cshtml"))
            {
                viewResult = viewEngine.GetView(viewPathName, viewPathName, false);
            }
            else
            {
                viewResult = viewEngine.FindView(controller.ControllerContext, viewPathName, false);
            }

            if (!viewResult.Success)
                throw new FileNotFoundException($"View '{viewPathName}' not found.");

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }

        public static async Task<string> RenderViewToStringAsync<TModel>(
            this Controller controller,
            string viewPathName)
        {
            if (string.IsNullOrEmpty(viewPathName))
            {
                viewPathName = controller.ControllerContext.ActionDescriptor.ActionName;
            }            

            using var writer = new StringWriter();
            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;


            ViewEngineResult viewResult = null;

            if (viewPathName.EndsWith(".cshtml"))
            {
                viewResult = viewEngine.GetView(viewPathName, viewPathName, false);
            }
            else
            {
                viewResult = viewEngine.FindView(controller.ControllerContext, viewPathName, false);
            }

            if (!viewResult.Success)
                throw new FileNotFoundException($"View '{viewPathName}' not found.");

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}
