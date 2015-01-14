﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Configuration
{
    public class Webhook
    {
        public Configuration.WebhookAccount Account { get; private set; }
        public Configuration.WebhookConnec Connec { get; private set; }

        /// <summary>
        /// Load Webhook configuration into a Webhook configuration object
        /// </summary>
        /// <returns>A Webhook configuration object</returns>
        public static Webhook Load()
        {
            return (new Webhook());
        }

        public Webhook()
        {
            Account = WebhookAccount.Load();
            Connec = WebhookConnec.Load();
        }
    }
}
