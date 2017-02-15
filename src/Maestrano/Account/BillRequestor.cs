using Maestrano.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maestrano.Configuration;

namespace Maestrano.Account
{
    public class BillRequestor : MnoClient<Bill>
    {
        public BillRequestor(Preset preset) : base(Bill.IndexPath(), Bill.ResourcePath(), preset)
        {
        }

        public List<Bill> All(NameValueCollection filters = null)
        {
            return All(Bill.IndexPath(), filters);
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
            return Create(att);
        }

        /// <summary>
        /// Cancel the recurring Bill
        /// </summary>
        /// <returns>if the recurring Bill was cancelled</returns>
        public Boolean Cancel(String id)
        {
            if (id != null)
            {
                Bill respBill = Delete(id);
                var Status = respBill.Status;
                return (Status.Equals("cancelled"));
            }
            return false;
        }

    }
}
