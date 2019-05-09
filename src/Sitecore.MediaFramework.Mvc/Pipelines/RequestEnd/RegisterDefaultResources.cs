namespace Sitecore.MediaFramework.Mvc.Pipelines.RequestEnd
{
    using System;
    using Sitecore.MediaFramework.Mvc.Extentions;
    using Sitecore.Mvc.Pipelines.Request.RequestEnd;
    using Sitecore.Mvc.Presentation;
    using System.Linq;

    public class RegisterDefaultResources : RequestEndProcessor
    {
        public override void Process(RequestEndArgs args)
        {
            try
            {
                PageContext pageContext = args.PageContext;
                if (pageContext == null || pageContext.Item == null)
                {
                    args.AbortPipeline();
                    return;
                }

                var apipath = Configuration.Settings.GetSetting("Sitecore.MediaFramework.Mvc.ApiPath", "api/sitecore/");
                var route = pageContext?.RequestContext?.HttpContext?.Request?.RawUrl;
                if (String.IsNullOrEmpty(route))
                {
                    args.AbortPipeline();
                    return;
                }
                if (apipath.Split('|').ToList().Any(route.Contains))
                {
                    args.AbortPipeline();
                    return;
                }

                pageContext.RequestContext.HttpContext.RegisterDefaultResources();
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}