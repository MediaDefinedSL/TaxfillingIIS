using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace TaxFiling.Web.Services  // <-- change this to your project namespace
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(Controller controller, string viewName, object model);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        //public async Task<string> RenderToStringAsync(string viewName, object model)
        //{
        //    var actionContext = new ActionContext(
        //        new DefaultHttpContext { RequestServices = _serviceProvider },
        //        new Microsoft.AspNetCore.Routing.RouteData(),
        //        new ActionDescriptor()
        //    );

        //    var viewResult = _viewEngine.FindView(actionContext, viewName, false);

        //    if (!viewResult.Success)
        //        throw new InvalidOperationException($"View '{viewName}' not found.");

        //    using var sw = new StringWriter();

        //    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        //    {
        //        Model = model
        //    };

        //    var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

        //    var viewContext = new ViewContext(
        //        actionContext,
        //        viewResult.View,
        //        viewData,
        //        tempData,
        //        sw,
        //        new HtmlHelperOptions()
        //    );

        //    await viewResult.View.RenderAsync(viewContext);
        //    return sw.ToString();
        //}

        public async Task<string> RenderToStringAsync(Controller controller, string viewName, object model)
        {
            var actionContext = new ActionContext(
                controller.HttpContext,
                controller.RouteData,
                controller.ControllerContext.ActionDescriptor
            );

            var viewResult = _viewEngine.FindView(actionContext, viewName, false);
            if (!viewResult.Success)
                throw new InvalidOperationException($"View '{viewName}' not found.");

            using var sw = new StringWriter();

            var viewData = new ViewDataDictionary(controller.ViewData)
            {
                Model = model
            };

            var tempData = new TempDataDictionary(controller.HttpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}

