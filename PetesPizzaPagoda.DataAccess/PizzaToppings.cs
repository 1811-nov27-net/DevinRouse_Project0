using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class PizzaToppings
    {
        public int PtId { get; set; }
        public int Pizza { get; set; }
        public int Topping { get; set; }
        public int Quantity { get; set; }

        public virtual Pizzas PizzaNavigation { get; set; }
        public virtual Toppings ToppingNavigation { get; set; }
    }
}
