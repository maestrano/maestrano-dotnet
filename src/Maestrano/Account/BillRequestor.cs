using Maestrano.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Account
{
    public class BillRequestor
    {
        private string presetName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="preset">Name of a preset</param>
        public BillRequestor(string presetName = "maestrano") {
            this.presetName = presetName;
        }

        public List<Bill> All(NameValueCollection filters = null)
        {
            return MnoClient.All<Bill>(Bill.IndexPath(), filters, presetName);
        }

        public Bill Retrieve(string billId)
        {
            return MnoClient.Retrieve<Bill>(Bill.ResourcePath(), billId, presetName);
        }

        public Bill Create(String groupId, Int32 priceCents, String description, String currency = "AUD", Decimal? units = null, DateTime? periodStartedAt = null, DateTime? periodEndedAt = null, bool thirdParty = false)
        {
            var att = new NameValueCollection();
            att.Add("groupId", groupId);
            att.Add("priceCents", priceCents.ToString());
            att.Add("description", description);
            att.Add("currency", currency);
            if (units.HasValue)
                att.Add("units", units.Value.ToString());
            if (periodStartedAt.HasValue)
                att.Add("periodStartedAt", periodStartedAt.Value.ToString("s"));
            if (periodEndedAt.HasValue)
                att.Add("periodEndedAt", periodEndedAt.Value.ToString("s"));
            if (thirdParty)
                att.Add("thirdParty", "true");
            return MnoClient.Create<Bill>(Bill.IndexPath(), att, presetName);
        }
    }
}
