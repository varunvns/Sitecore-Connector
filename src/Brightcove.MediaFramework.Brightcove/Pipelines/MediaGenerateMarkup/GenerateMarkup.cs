// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Generates a markup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using RestSharp.Extensions;
using Sitecore.Text;

namespace Brightcove.MediaFramework.Brightcove.Pipelines.MediaGenerateMarkup
{
  using System;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.MediaFramework.Pipelines.MediaGenerateMarkup;

  /// <summary>
  /// Generates a markup.
  /// </summary>
  public class GenerateMarkup : MediaGenerateMarkupProcessorBase
  {
    /// <summary>
    /// Processes generating of a markup.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public override void Process(MediaGenerateMarkupArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Generator, "args.Generator");

      switch (args.MarkupType)
      {
        case MarkupType.Frame:
          args.Result.Html = args.Properties.Collection[BrightcovePlayerParameters.EmbedStyle].HasValue() &&
                             args.Properties.Collection[BrightcovePlayerParameters.EmbedStyle] == Brightcove.Constants.EmbedJavascript ?
            args.Result.Html = GenerateJavascriptEmbed(args) :
            args.Result.Html = GenerateIframeEmbed(args);
            
          //args.Result.Html = args.Generator.GetFrame(args);
          break;
        case MarkupType.FrameUrl:
          args.Result.Html = args.Generator.GenerateFrameUrl(args);
          break;
        case MarkupType.Link:
          if (string.IsNullOrEmpty(args.LinkTitle))
          {
            args.LinkTitle = Translate.Text("Media Link");
          }

          args.Result.Html = args.Generator.GenerateLinkHtml(args);
          break;
        case MarkupType.Html:
          if (!args.Properties.ForceRender && Context.PageMode.IsExperienceEditorEditing)
          {
            args.Result.Html = args.Generator.GetPreviewImage(args);
          }
          else
          {
            args.Result = args.Generator.Generate(args);
          }
          break;
      }
    }

    public virtual string GenerateFrameUrl(MediaGenerateMarkupArgs args)
    {
      var itemId = args.MediaItem.ID.ToGuid().ToString("N");
      var playerId = args.PlayerItem.ID.ToGuid().ToString("N");
      var url = new UrlString($"{Sitecore.MediaFramework.Constants.PlayerIframeUrl}?itemId={itemId}&playerId={playerId}");

      return url.ToString();
    }

    public virtual string GenerateBrightcoveUrl(MediaGenerateMarkupArgs args, bool isJs)
    {
      var resource = isJs ? "index.min.js" : $"index.html?videoId={args.MediaItem[BrightcovePlayerParameters.MediaId]}";
      var url = new UrlString($"//players.brightcove.net/{args.AccountItem[BrightcovePlayerParameters.PublisherId]}/{args.PlayerItem[BrightcovePlayerParameters.PlayerId]}_default/{resource}");
      return url.ToString();
    }

    protected virtual string GenerateJavascriptEmbed(MediaGenerateMarkupArgs args)
    {
      string responsive = String.Empty;
      string responsiveStyle = String.Empty;
      string responsiveClosingTags = String.Empty;
      string autoplay = String.Empty;
      string muted = String.Empty;

      // Add autoplay
      if (args.Properties.Collection[BrightcovePlayerParameters.EmbedStyle] != null)
      {
        var calcPadding = ((float)args.Properties.Height / args.Properties.Width) * 100;
        responsive = $"<div style='position: relative; display: block; max-width: {args.Properties.Width}px;'><div style='padding-top: {calcPadding}%;'>";
        responsiveStyle = "style='position: absolute; top: 0px; right: 0px; bottom: 0px; left: 0px; width: 100%; height: 100%;'";
        responsiveClosingTags = "</div></div>";
      }
      // Add autoplay
      if (args.Properties.Collection[BrightcovePlayerParameters.Autoplay] != null)
      {
        autoplay = "autoplay='autoplay'";
      }
      // Add muted
      if (args.Properties.Collection[BrightcovePlayerParameters.Muted] != null)
      {
        muted = "muted='muted'";
      }
      
	    return $@"{responsive}
                <video data-video-id='{args.MediaItem[BrightcovePlayerParameters.MediaId]}'
                  data-account='{args.AccountItem[BrightcovePlayerParameters.PublisherId]}' 
	                data-player='{args.PlayerItem[BrightcovePlayerParameters.PlayerId]}' 
	                data-embed='default' 
	                data-application-id 
	                class='video-js' 
	                controls='true' {autoplay} {muted} 
	                {responsiveStyle}></video>
                  <script src='{this.GenerateBrightcoveUrl(args, true)}'></script>
                {responsiveClosingTags}";
    }

    protected virtual string GenerateIframeEmbed(MediaGenerateMarkupArgs args)
    {
      string width = $"width='{args.Properties.Width}'";
      string height = $"height='{args.Properties.Height}'";
      string responsive = String.Empty;
      string responsiveStyle = String.Empty;
      string responsiveClosingTags = String.Empty;
      string queryStr = String.Empty;

      // Add autoplay
      if (args.Properties.Collection[BrightcovePlayerParameters.EmbedStyle] == Brightcove.Constants.SizingResponsive)
      {
        var calcPadding = ((float)args.Properties.Height / args.Properties.Width) * 100;
        responsive = $"<div style='position: relative; display: block; max-width: {args.Properties.Width}px;'><div style='padding-top: {calcPadding}%;'>";
        responsiveStyle = "style='position: absolute; top: 0px; right: 0px; bottom: 0px; left: 0px; width: 100%; height: 100%;'";
        responsiveClosingTags = "</div></div>";
        width = height = String.Empty;
      }

      if (args.Properties.Collection[BrightcovePlayerParameters.Autoplay] != null)
        queryStr += "&autoplay=true";
      if (args.Properties.Collection[BrightcovePlayerParameters.Muted] != null)
        queryStr += "&muted=true";

      return $@"{responsive}
                <iframe scrolling='no' class='player-frame' {width} {height} frameborder='0' allowfullscreen='' webkitallowfullscreen='' mozallowfullscreen=''
                  src='{this.GenerateBrightcoveUrl(args, false)}{queryStr}' {responsiveStyle}></iframe>
              {responsiveClosingTags}";
    }
  }
}