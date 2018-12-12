using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class Orders
    {
        public Orders()
        {
            OrderEntries = new HashSet<OrderEntries>();
        }

        public int OId { get; set; }
        public int OrderFor { get; set; }
        public int OrderedFrom { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumOfOrderedItems { get; set; }

        public virtual Users OrderForNavigation { get; set; }
        public virtual Locations OrderedFromNavigation { get; set; }
        public virtual ICollection<OrderEntries> OrderEntries { get; set; }
    }
}
