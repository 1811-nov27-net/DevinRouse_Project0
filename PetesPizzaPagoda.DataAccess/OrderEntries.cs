using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class OrderEntries
    {
        public int OeId { get; set; }
        public int OnOrder { get; set; }
        public int Pizza { get; set; }
        public int Quantity { get; set; }

        public virtual Orders OnOrderNavigation { get; set; }
        public virtual Pizzas PizzaNavigation { get; set; }
    }
}
