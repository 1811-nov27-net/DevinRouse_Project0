using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetesPizzaPagoda.DataAccess
{
    /// <summary>
    /// A repository for handling data access for objects of an online pizza store
    /// </summary>
    public class PizzaRepository : IPizzaRepository
    {
        public _1811proj0Context Db { get; }

        /// <summary>
        /// Initializes a new pizza repository, provided a valid dbContext
        /// </summary>
        /// <param name="db">The DbContext</param>
        public PizzaRepository(_1811proj0Context db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Save changes to the database
        /// </summary>
        public void SaveChanges()
        {
            Db.SaveChanges();
        }

        /// <summary>
        /// Create a new Orders object with a given user and location
        /// </summary>
        /// <param name="userId">ID of the user object</param>
        /// <param name="locationId">ID of the location object</param>
        public void CreateOrder(int userId, int locationId)
        {
            Orders order = new Orders();

            Users selectedUser = Db.Users.First(i => i.UId == userId);
            Locations selectedLocation = Db.Locations.First(i => i.LId == locationId);

            order.OrderForNavigation = selectedUser;
            order.OrderedFromNavigation = selectedLocation;
            order.OrderedAt = DateTime.Now;
            order.TotalPrice = 0;

            Db.Orders.Add(order);
            Db.SaveChanges();
        }
        
        /// <summary>
        /// Generate a suggested order based on used history
        /// </summary>
        /// <param name="user">The user to generate an order for</param>
        public void GetSuggestedOrder(Users user)
        {
            int totalPizzasInHistory = Db.OrderEntries
                .Join(Db.Orders, oe => oe.OnOrder, o => o.OId, (oe, o) => new { oe, o })
                .Join(Db.Users, oeo => oeo.o.OrderFor, u => u.UId, (oeo, u) => new { oeo, u })
                .Where(m => m.u.FirstName == user.FirstName)
                .Where(m => m.u.LastName == user.LastName)
                .Sum(m => m.oeo.oe.Quantity);

            int totalOrdersInHistory = Db.Orders.Where(m => m.OrderFor == user.UId).Count();

            int totalOrderEntriesInHistory = Db.OrderEntries
                .Join(Db.Orders, oe => oe.OnOrder, o => o.OId, (oe, o) => new { oe, o })
                .Join(Db.Users, oeo => oeo.o.OrderFor, u => u.UId, (oeo, u) => new { oeo, u })
                .Where(m => m.u.FirstName == user.FirstName)
                .Where(m => m.u.LastName == user.LastName)
                .Count();

            // ?!?!?!?!?!?!?!?!?!
            // Check what you wrote for the EnoughIngredients method for a solution
            var countOfPizzasTypesInHistory = Db.Pizzas
                .Join(Db.OrderEntries, p => p.PId, oe => oe.Pizza, (p, oe) => new { p, oe })
                .Join(Db.Orders, poe => poe.oe.OnOrder, o => o.OId, (poe, o) => new { poe, o })
                .Join(Db.Users, poeo => poeo.o.OrderFor, u => u.UId, (poeo, u) => new { poeo, u })
                .Where(m => m.u.FirstName == user.FirstName)
                .Where(m => m.u.LastName == user.LastName)
                .ToList()
                .GroupBy(m => m.poeo.poe.p.PId);
           


        }

        /// <summary>
        /// Get a list of a list of all users sorted by last name, then first name
        /// </summary>
        public IList<Users> AllUsersByName => Db.Users.AsNoTracking().OrderBy(u => u.FirstName).OrderBy(u => u.LastName).ToList();

        /// <summary>
        /// Get a list of the order entries of a given order
        /// </summary>
        /// <param name="order">The order to return the details of</param>
        /// <returns>The list of order entries for the order</returns>
        public IList<OrderEntries> ShowOrderDetails(Orders order) => Db.OrderEntries.AsNoTracking().Include(m => m.PizzaNavigation).Where(m => m.OnOrder == order.OId).ToList();

        /// <summary>
        /// Get a list of all orders placed at a given location
        /// </summary>
        /// <param name="location">The location to return all orders for</param>
        /// <returns>The list of orders for the location</returns>
        public IList<Orders> GetLocationsOrderHistory(int location) => Db.Orders.AsNoTracking().Where(m => m.OrderedFrom == location).ToList();

        /// <summary>
        /// Get a list of all orders place by a given user
        /// </summary>
        /// <param name="user">The user to return all orders for</param>
        /// <returns>The list of orders for the user</returns>
        public IList<Orders> GetUsersOrderHistory(int user) => Db.Orders.AsNoTracking().Where(m => m.OrderFor == user).ToList();

        /// <summary>
        /// Get a list of all orders, sorted by cheapest to most expensive
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        public IList<Orders> GetOrderHistoryByCheapest() => Db.Orders.AsNoTracking().OrderBy(m => m.TotalPrice).ToList();

        /// <summary>
        /// Get a list of all orders, sorted by most expensive to cheapest
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        public IList<Orders> GetOrderHistoryByMostExpensive() => Db.Orders.AsNoTracking().OrderByDescending(m => m.TotalPrice).ToList();

        /// <summary>
        /// Get a list of all orders, sorted by earliest to most recent
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        public IList<Orders> GetOrderHistoryByEarliest() => Db.Orders.AsNoTracking().OrderBy(m => m.OrderedAt).ToList();

        /// <summary>
        /// Get a list of all orders, sorted by most recent to earliest
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        public IList<Orders> GetOrderHistoryByLatest() => Db.Orders.AsNoTracking().OrderByDescending(m => m.OrderedAt).ToList();

        /// <summary>
        /// Check to see if a given user has ordered from a given location within the last two hours
        /// </summary>
        /// <param name="userId">The selected user</param>
        /// <param name="locationId">The selected location</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        public bool TooEarly(int userId, int locationId)
        {
            DateTime timeOfLastUserOrder = Db.Orders.Where(m => m.OrderFor == userId).Where(m => m.OrderedFrom == locationId).LastOrDefault().OrderedAt;

            if(DateTime.Now < timeOfLastUserOrder.AddHours(2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check to see if an order exceeds the limit of 12 pizzas per order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        public bool ValidOrderSize(Orders order)
        {
            if(GetNumberOfOrderItems(order) <= 12 && GetNumberOfOrderItems(order) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check to see if an order exceeds the limit of $500 total price per order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        public bool ValidOrderPrice(Orders order)
        {
            if (GetOrderTotal(order) <= 500M)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check to see if the toppings an order exceed the amount of toppings available at the location on the order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        public bool EnoughIngredients(Orders order)
        {
            bool clearsCheck = true;
            int selectedLocation = order.OrderedFrom;
            IList<LocationInventory> inventory = Db.LocationInventory.Include(m => m.ToppingNavigation).Where(m => m.Location == selectedLocation).ToList();

            foreach (var invTop in inventory)
            {
                int orderNeed = Db.OrderEntries
                .Join(Db.Pizzas, oe => oe.Pizza, p => p.PId, (oe, p) => new { oe, p })
                .Join(Db.PizzaToppings, oep => oep.p.PId, pt => pt.Pizza, (oep, pt) => new { oep, pt })
                .Where(m => m.oep.oe.OnOrder == order.OId).Where(m => m.pt.Topping == invTop.Topping).ToList().Sum(m => (m.pt.Quantity * m.oep.oe.Quantity));

                if(invTop.Quantity < orderNeed)
                {
                    clearsCheck = false;
                }
            }
            return clearsCheck;
        }

        /// <summary>
        /// Get the total price of a given order
        /// </summary>
        /// <param name="order">The order to get the total for</param>
        /// <returns>A decimal value of the total price</returns>
        public decimal GetOrderTotal(Orders order)
        {      
            return Db.OrderEntries.Include(m => m.PizzaNavigation).Where(m => m.OnOrder == order.OId).ToList().Sum(m => (m.PizzaNavigation.Price * m.Quantity));
        }

        /// <summary>
        /// Create a new OrderEntries object for the given order, pizza, and quantity of the given pizza
        /// </summary>
        /// <param name="orderId">Integer ID of the order</param>
        /// <param name="pizzaId">Integer ID of the given pizza</param>
        /// <param name="quantity">Integer number for how many of the selected pizza</param>
        public void CreateOrderEntry(int orderId, int pizzaId, int quantity)
        {
            Orders selectedOrder = Db.Orders.FirstOrDefault(i => i.OId == orderId) ?? throw new ArgumentException("The selected order has no matching ID in the database", nameof(Db.Orders));
            Pizzas selectedPizza = Db.Pizzas.FirstOrDefault(i => i.PId == pizzaId) ?? throw new ArgumentException("The selected pizza has no matching ID in the database", nameof(Db.Pizzas));

            OrderEntries orderEntry = new OrderEntries
                                    {
                                        OnOrderNavigation = selectedOrder,
                                        PizzaNavigation = selectedPizza,
                                        Quantity = quantity
                                    };

            Db.OrderEntries.Add(orderEntry);
            Db.SaveChanges();
        }

        /// <summary>
        /// Get the total number of pizzas on the order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>Integer value of the total number of pizzas on the order</returns>
        public int GetNumberOfOrderItems(Orders order)
        {
            return Db.OrderEntries.Where(m => m.OnOrder == order.OId).ToList().Sum(m => m.Quantity);
        }

        /// <summary>
        /// Deletes an order and all associated order entries
        /// </summary>
        /// <param name="order">The order to be deleted</param>
        public void CancelOrder(Orders order)
        {
            Orders cancelMe = order ?? throw new ArgumentException("The selected order does not exist", nameof(Db.Orders));

            IList <OrderEntries> entriesToRemove = Db.OrderEntries.Where(m => m.OnOrder == cancelMe.OId).ToList();

            foreach(var item in entriesToRemove)
            {
                Db.OrderEntries.Remove(item);
            }

            Db.Orders.Remove(order);
            Db.SaveChanges();
        }

        /// <summary>
        /// Get a user object based on ID
        /// </summary>
        /// <param name="id">Integer ID of the user to be returned</param>
        /// <returns>User object to get info from</returns>
        public Users GetUserById(int id)
        {
            return Db.Users.Find(id);
        }

        /// <summary>
        /// Get an order object based on ID
        /// </summary>
        /// <param name="id">Integer ID of the order to be returned</param>
        /// <returns>Order object to get info from</returns>
        public Orders GetOrderById(int id)
        {
            return Db.Orders.Find(id);
        }

        /// <summary>
        /// Get the order currently working by the current user
        /// </summary>
        /// <param name="userId">Integer ID of the order to be returned</param>
        /// <returns>Current latest order</returns>
        public Orders GetWorkingOrder(int userId)
        {
            return Db.Orders.Where(m => m.OrderFor == userId).Last();
        }

        /// <summary>
        /// Get a list of all pizza
        /// </summary>
        /// <returns>The list of pizzas</returns>
        public IList<Pizzas> ListAllPizzas() => Db.Pizzas.AsNoTracking().ToList();

        /// <summary>
        /// Delete an order entry
        /// </summary>
        /// <param name="entryID">Integer ID of the entry to be deleted</param>
        public void DeleteOrderEntry(int entryID)
        {
            Db.OrderEntries.Remove(Db.OrderEntries.Find(entryID));
            Db.SaveChanges();
        }
    }
}
