using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class LocationInventory
    {
        public int LiId { get; set; }
        public int Location { get; set; }
        public int Topping { get; set; }
        public int Quantity { get; set; }

        public virtual Locations LocationNavigation { get; set; }
        public virtual Toppings ToppingNavigation { get; set; }
    }
}
