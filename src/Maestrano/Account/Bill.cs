using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maestrano.Api;
using System.Collections.Specialized;
using RestSharp;
using Newtonsoft.Json;
using Maestrano.Configuration;

namespace Maestrano.Account
{
    public class Bill : MnoObject
    {
        /// <summary>
        /// Status of the bill. Either 'submitted', 'invoiced' or 'cancelled'
        /// </summary>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// How many units are billed for the amount charged
        /// </summary>
        [JsonProperty("units")]
        public decimal? Units { get; set; }

        /// <summary>
        /// If the bill relates to a specific period then specifies when the period started.Both period_started_at and period_ended_at need to be filled in order to appear on customer invoice.
        /// </summary>
        [JsonProperty("period_started_at")]
        public DateTime? PeriodStartedAt { get; set; }

        /// <summary>
        /// If the bill relates to a specific period then specifies when the period ended. Both period_started_at and period_ended_at need to be filled in order to appear on customer invoice.
        /// </summary>
        [JsonProperty("period_ended_at")]
        public DateTime? PeriodEndedAt { get; set; }

        /// <summary>
        /// The id of the group you are charging
        /// Mandatory for creation
        /// </summary>
        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// The amount in cents to charge to the customer
        /// </summary>
        [JsonProperty("price_cents")]
        public Int32 PriceCents { get; set; }

        /// <summary>
        /// The currency of the amount charged in  http://en.wikipedia.org/wiki/ISO_4217#Active_codes ISO 4217 format
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// A description of the product billed as it should appear on customer invoice
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Flag for third-party bills (e.g.: charge for SMS credits). Third party bills are not subject to commissions.
        /// </summary>
        [JsonProperty("third_party")]
        public bool ThirdParty { get; set; }

        /// <summary>
        /// Cancel the Bill
        /// </summary>
        /// <returns>if the recurring Bill was cancelled</returns>
        public Boolean Cancel()
        {
            if (Preset == null)
            {
                throw new Exception("Preset has not been set on the Bill. The Bill needs to be retrieved before: preset.Bill.Retrieve(id)");
            }
            var requestor = new BillRequestor(Preset);
            return requestor.Cancel(Id);
        }


        /// <summary>
        /// The Resource name
        /// </summary>
        public static string IndexPath()
        {
            return "account/bills";
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
        /// <param name="presetName">name of preset to use</param>
        public static BillRequestor With(Preset preset)
        {
            return new BillRequestor(preset);
        }

    }
}
