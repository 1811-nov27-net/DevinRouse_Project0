using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class Locations
    {
        public Locations()
        {
            LocationInventory = new HashSet<LocationInventory>();
            Orders = new HashSet<Orders>();
            Users = new HashSet<Users>();
        }

        public int LId { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public virtual ICollection<LocationInventory> LocationInventory { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
