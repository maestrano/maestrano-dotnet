using Maestrano.Api;
using Maestrano.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Account
{
    public class RecurringBill:MnoObject
    {

        /// <summary>
        /// Status of the recurring bill. Either 'submitted', 'active', 'expired' or 'cancelled'
        /// </summary>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// How many units are billed for the amount charged
        /// </summary>
        [JsonProperty("units")]
        public decimal? Units { get; set; }

        /// <summary>
        /// The unit of measure for the billing cycle. Must be one of the following: 'Day', 'Week', 'SemiMonth', 'Month', 'Year'
        /// </summary>
        [JsonProperty("period")]
        public String Period { get; set; }

        /// <summary>
        /// The number of billing periods that make up one billing cycle. The combination of billing frequency and billing period must be less than or equal to one year. If the billing period is SemiMonth, the billing frequency must be 1.
        /// </summary>
        [JsonProperty("frequency")]
        public Int16? Frequency { get; set; }

        /// <summary>
        /// The number of cycles this bill should be active for. In other words it's the number of times this recurring bill should charge the customer.
        /// </summary>
        [JsonProperty("cycles")]
        public Int16? Cycles { get; set; }

        /// <summary>
        /// The date when this recurring bill should start billing the customer
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Initial non-recurring payment amount - in cents - due immediately upon creating the recurring bill
        /// </summary>
        [JsonProperty("initial_cents")]
        public Int32? InitialCents { get; set; }

        /// <summary>
        /// The amount in cents to charge to the customer
        /// </summary>
        [JsonProperty("price_cents")]
        public Int32 PriceCents { get; set; }

        /// <summary>
        /// The currency of the amount charged in http://en.wikipedia.org/wiki/ISO_4217#Active_codes ISO 4217 format
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// A description of the product billed as it should appear on customer invoice
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The id of the group you are charging
        /// Mandatory for creation
        /// </summary>
        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// Cancel the Recurring Bill
        /// </summary>
        /// <returns>if the recurring Bill was cancelled</returns>
        public Boolean Cancel(String id)
        {
            if (Preset == null)
            {
                throw new Exception("Preset has not been set on the RecurringBill. The RecurringBill needs to be retrieved before: preset.RecurringBill.Retrieve(id)");
            }
            var requestor = new RecurringBillRequestor(Preset);
            return requestor.Cancel(Id);
        }

        /// <summary>
        /// The Resource name
        /// </summary>
        public static string IndexPath()
        {
            return "account/recurring_bills";
        }

        /// <summary>
        /// The Single Resource name
        /// </summary>
        public static string ResourcePath()
        {
            return IndexPath() + "/{id}";
        }

        /// <summary>
        /// Scope REST calls to specific configuration presets
        /// </summary>
        /// <param name="preset">preset to use</param>
        public static RecurringBillRequestor With(Preset preset)
        {
            return new RecurringBillRequestor(preset);
        }
    }
}
