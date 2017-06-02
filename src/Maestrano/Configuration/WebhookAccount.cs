using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace Maestrano.Configuration
{
    public class WebhookAccount
    {
        /// <summary>
        /// Load Webhook into a WebhookAccount configuration object from a JObject 
        /// </summary>
        /// <returns>A WebhookAccount configuration object</returns>
        public static WebhookAccount LoadFromJson(JObject obj)
        {
            var config = new WebhookAccount();
            config.GroupPath = obj["group_path"].Value<string>();
            config.GroupUserPath = obj["group_user_path"].Value<string>();
            return config;
        }

        /// <summary>
        /// Application REST endpoint for groups
        /// </summary>
        public String GroupPath { get; set; }

        [Obsolete("GroupsPath is deprecated, use GroupPath instead")]
        public string GroupsPath { get { return GroupPath; } set { GroupPath = value; } }


        /// <summary>
        /// Application REST endpoint for group > users
        /// </summary>
        public String GroupUserPath { get; set; }


        [Obsolete("GroupUsersPath is deprecated, use GroupUserPath instead")]
        public string GroupUsersPath { get { return GroupUserPath; } set { GroupUserPath = value; } }


    }
}
