using Maestrano.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Maestrano.Configuration
{

    [Obsolete]
    public class WebhookConnecSubscriptions : Dictionary<String, Boolean>
    {
        public static WebhookConnecSubscriptions LoadFromJson(JObject obj)
        {
            var config = new WebhookConnecSubscriptions();
            foreach(var x in obj) {
                config[StringExtensions.ToCamelCase(x.Key)] = x.Value.Value<Boolean>();
            }
            return config;
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Accounts
        {
            get { return true; }
            set { this["accounts"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Company
        {
            get { return true; }
            set { this["company"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer invoices
        /// </summary>
        [Obsolete]
        public bool Invoices
        {
            get { return true; }
            set { this["invoices"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer sales Orders
        /// </summary>
        [Obsolete]
        public bool SalesOrders
        {
            get { return true; }
            set { this["salesOrders"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer purchase Orders
        /// </summary>
        [Obsolete]
        public bool PurchaseOrders
        {
            get { return true; }
            set { this["purchaseOrders"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer quotes
        /// </summary>
        [Obsolete]
        public bool Quotes
        {
            get { return true; }
            set { this["quotes"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Payments
        {
            get { return true; }
            set { this["payments"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Journals
        {
            get { return true; }
            set { this["journals"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Items
        {
            get { return true; }
            set { this["items"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Organizations
        {
            get { return true; }
            set { this["organizations"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool People
        {
            get { return true; }
            set { this["people"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Projects
        {
            get { return true; }
            set { this["projects"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool TaxCodes
        {
            get { return true; }
            set { this["taxCodes"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool TaxRates
        {
            get { return true; }
            set { this["taxRates"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Events
        {
            get { return true; }
            set { this["events"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Venues
        {
            get { return true; }
            set { this["venues"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool EventOrders
        {
            get { return true; }
            set { this["eventOrders"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool WorkLocations
        {
            get { return true; }
            set { this["workLocations"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool PayItems
        {
            get { return true; }
            set { this["payItems"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool Employees
        {
            get { return true; }
            set { this["employees"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool PaySchedules
        {
            get { return true; }
            set { this["paySchedules"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool TimeSheets
        {
            get { return true; }
            set { this["timeSheets"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool TimeActivities
        {
            get { return true; }
            set { this["timeActivities"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool PayRuns
        {
            get { return true; }
            set { this["payRuns"] = value; }
        }

        /// <summary>
        /// Whether to receive notifications related to the customer company
        /// </summary>
        [Obsolete]
        public bool PayStubs
        {
            get { return true; }
            set { this["payStubs"] = value; }
        }
    }
}
