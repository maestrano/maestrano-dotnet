using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace Maestrano.Configuration
{
    public class WebhookConnec
    {
        [Obsolete("Not used anymore")]
        public WebhookConnecSubscriptions Subscriptions { get; set; }
        /// <summary>
        /// Load Webhook into a WebhookAccount configuration object from a JObject 
        /// </summary>
        /// <returns>A WebhookAccount configuration object</returns>
        public static WebhookConnec LoadFromJson(JObject obj)
        {
            var config = new WebhookConnec();
            config.NotificationPath = obj["notification_path"].Value<string>();
            config.InitializationPath = obj["initialization_path"].Value<string>();
            config.ExternalIds = obj["external_ids"].Value<bool>();
            config.Subscriptions = new WebhookConnecSubscriptions();
            return config;
        }

        /// <summary>
        /// return true if connec will use external ids
        /// </summary>
        public bool ExternalIds { get; set; }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        public string NotificationPath { get; set; }

        [Obsolete("NotificationsPath is deprecated, use NotificationPath instead")]
        public string NotificationsPath { get { return NotificationPath; } set { NotificationPath = value ; } }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        public string InitializationPath { get; set; }

    }
}
