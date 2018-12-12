using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class Toppings
    {
        public Toppings()
        {
            LocationInventory = new HashSet<LocationInventory>();
            PizzaToppings = new HashSet<PizzaToppings>();
        }

        public int TId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<LocationInventory> LocationInventory { get; set; }
        public virtual ICollection<PizzaToppings> PizzaToppings { get; set; }
    }
}
