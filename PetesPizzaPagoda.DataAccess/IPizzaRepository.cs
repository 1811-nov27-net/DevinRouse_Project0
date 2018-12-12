using System;
using System.Collections.Generic;
using System.Text;

namespace PetesPizzaPagoda.DataAccess
{
    /// <summary>
    /// An interface for repository objects that handle data access for objects of an online pizza store
    /// </summary>
    public interface IPizzaRepository
    {
        /// <summary>
        /// Generate a suggested order based on used history
        /// </summary>
        /// <param name="user">The user to generate an order for</param>
        void GetSuggestedOrder(Users user);

        /// <summary>
        /// Return a list of all users sorted by last name, then first name
        /// </summary>
        IList<Users> AllUsersByName { get; }

        /// <summary>
        /// Get a list of the order entries of a given order
        /// </summary>
        /// <param name="order">The order to return the details of</param>
        /// <returns>The list of order entries for the order</returns>
        IList<OrderEntries> ShowOrderDetails(Orders order);

        /// <summary>
        /// Get a list of all orders placed at a given location
        /// </summary>
        /// <param name="location">The location to return all orders for</param>
        /// <returns>The list of orders for the location</returns>
        IList<Orders> GetLocationsOrderHistory(int location);

        /// <summary>
        /// Get a list of all orders place by a given user
        /// </summary>
        /// <param name="user">The user to return all orders for</param>
        /// <returns>The list of orders for the user</returns>
        IList<Orders> GetUsersOrderHistory(int user);

        /// <summary>
        /// Get a list of all orders, sorted by cheapest to most expensive
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        IList<Orders> GetOrderHistoryByCheapest();

        /// <summary>
        /// Get a list of all orders, sorted by most expensive to cheapest
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        IList<Orders> GetOrderHistoryByMostExpensive();

        /// <summary>
        /// Get a list of all orders, sorted by earliest to most recent
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        IList<Orders> GetOrderHistoryByEarliest();

        /// <summary>
        /// Get a list of all orders, sorted by most recent to earliest
        /// </summary>
        /// <returns>The sorted list of orders</returns>
        IList<Orders> GetOrderHistoryByLatest();

        /// <summary>
        /// Check to see if a given user has ordered from a given location within the last two hours
        /// </summary>
        /// <param name="userId">The selected user</param>
        /// <param name="locationId">The selected location</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        bool TooEarly(int user, int location);

        /// <summary>
        /// Check to see if an order exceeds the limit of 12 pizzas per order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        bool ValidOrderSize(Orders order);

        /// <summary>
        /// Check to see if an order exceeds the limit of $500 total price per order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        bool ValidOrderPrice(Orders order);

        /// <summary>
        /// Check to see if the toppings an order exceed the amount of toppings available at the location on the order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>A boolean value indicating whether the condition in the summary is true</returns>
        bool EnoughIngredients(Orders order);

        /// <summary>
        /// Create a new Orders object with a given user and location
        /// </summary>
        /// <param name="userId">ID of the user object</param>
        /// <param name="locationId">ID of the location object</param> 
        void CreateOrder(int userId, int locationId);

        /// <summary>
        /// Get the total price of a given order
        /// </summary>
        /// <param name="order">The order to get the total for</param>
        /// <returns>A decimal value of the total price</returns>
        decimal GetOrderTotal(Orders order);

        /// <summary>
        /// Create a new OrderEntries object for the given order, pizza, and quantity of the given pizza
        /// </summary>
        /// <param name="orderId">Integer ID of the order</param>
        /// <param name="pizzaId">Integer ID of the given pizza</param>
        /// <param name="quantity">Integer number for how many of the selected pizza</param>
        void CreateOrderEntry(int orderId, int pizzaId, int quantity);

        /// <summary>
        /// Get the total number of pizzas on the order
        /// </summary>
        /// <param name="order">The order being checked</param>
        /// <returns>Integer value of the total number of pizzas on the order</returns>
        int GetNumberOfOrderItems(Orders order);

        /// <summary>
        /// Deletes an order and all associated order entries
        /// </summary>
        /// <param name="order">The order to be deleted</param>
        void CancelOrder(Orders order);

        /// <summary>
        /// Save changes to the database
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Get a user object based on ID
        /// </summary>
        /// <param name="id">Integer ID of the user to be returned</param>
        /// <returns>User object to get info from</returns>
        Users GetUserById(int id);

        /// <summary>
        /// Get an order object based on ID
        /// </summary>
        /// <param name="id">Integer ID of the order to be returned</param>
        /// <returns>Order object to get info from</returns>
        Orders GetOrderById(int id);

        /// <summary>
        /// Get the order currently working by the current user
        /// </summary>
        /// <param name="userId">Integer ID of the order to be returned</param>
        /// <returns>Current latest order</returns>
        Orders GetWorkingOrder(int userId);

        /// <summary>
        /// Get a list of all pizza
        /// </summary>
        /// <returns>The list of pizzas</returns>
        IList<Pizzas> ListAllPizzas();

        /// <summary>
        /// Delete an order entry
        /// </summary>
        /// <param name="entryID">Integer ID of the entry to be deleted</param>
        void DeleteOrderEntry(int entryID);
    }
}

