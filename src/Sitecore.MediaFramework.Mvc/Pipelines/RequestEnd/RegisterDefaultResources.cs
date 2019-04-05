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
        if (pageContext == null)
        {
          return;
        }
        
        var apipath = Sitecore.Configuration.Settings.GetSetting("Sitecore.MediaFramework.Mvc.ApiPath", "api/sitecore/");
        if(apipath.Split('|').ToList().Any(Sitecore.Links.LinkManager.GetItemUrl(pageContext.Item).Contains))
        {
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