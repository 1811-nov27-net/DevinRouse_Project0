using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.DataAccess
{
    public partial class Users
    {
        public Users()
        {
            Orders = new HashSet<Orders>();
        }

        public int UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DefaultLocation { get; set; }

        public virtual Locations DefaultLocationNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
