using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class Pizzas
    {
        public Pizzas()
        {
            OrderEntries = new HashSet<OrderEntries>();
            PizzaToppings = new HashSet<PizzaToppings>();
        }

        public int PId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<OrderEntries> OrderEntries { get; set; }
        public virtual ICollection<PizzaToppings> PizzaToppings { get; set; }
    }
}
