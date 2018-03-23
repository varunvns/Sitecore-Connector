using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Tracking;
using Sitecore.Configuration;
using Sitecore.MediaFramework;
using Sitecore.SecurityModel.License;

namespace Brightcove.MediaFramework.Brightcove.Analytics
{
    public abstract class EventTrigger : Sitecore.MediaFramework.Analytics.EventTrigger
    {
        protected override void TriggerEvent(PageEventData eventData)
        {
            var analyticsEnabled = Configuration.Settings.AnalyticsEnabled;

            if (!analyticsEnabled)
                return;
            if (!Tracker.IsActive)
                Tracker.StartTracking();
            if (Tracker.Current.CurrentPage == null)
                return;
            switch (eventData.Name.ToLowerInvariant())
            {
                case "playbackstarted":
                    eventData.PageEventDefinitionId = ItemIDs.PageEvents.PlaybackStarted.ToGuid();
                    break;
                case "playbackcompleted":
                    eventData.PageEventDefinitionId = ItemIDs.PageEvents.PlaybackCompleted.ToGuid();
                    break;
                case "playbackchanged":
                    eventData.PageEventDefinitionId = ItemIDs.PageEvents.PlaybackChanged.ToGuid();
                    break;
                case "playbackerror":
                    eventData.PageEventDefinitionId = ItemIDs.PageEvents.PlaybackError.ToGuid();
                    break;
                default:
                    break;
            }
            ((IPageContext)Tracker.Current.CurrentPage).Register(eventData);
        }
    }
}