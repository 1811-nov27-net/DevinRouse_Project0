using Microsoft.EntityFrameworkCore;
using PetesPizzaPagoda.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PetesPizzaPagoda.Tests
{
    public class PizzaRepositoryTests
    {
        // Putting this in a comment for copy/paste purposes
        // using (var db = new _1811proj0Context(options))

        [Fact]
        public void CreateOrderWorks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("createorder_test").Options;
            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "Test", State = "Location" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "Sample", LastName = "User", DefaultLocation = 1 });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                repo.CreateOrder(1, 1);
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Orders order = db.Orders.Include(m => m.OrderForNavigation).Include(m => m.OrderedFromNavigation).First(m => m.OrderedFromNavigation.City == "Test");

                Assert.NotEqual(0, order.OId);
                Assert.NotNull(order.OrderForNavigation);
                Assert.NotNull(order.OrderedFromNavigation);
                Assert.NotEqual(0, order.OrderFor);
                Assert.NotEqual(0, order.OrderedFrom);
                Assert.Equal("Sample", order.OrderForNavigation.FirstName);
                Assert.Equal("User", order.OrderForNavigation.LastName);
                Assert.Equal("Test", order.OrderedFromNavigation.City);
                Assert.Equal("Location", order.OrderedFromNavigation.State);
            }
        }

        [Fact]
        public void ShowOrderDetailsWorks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("showorder_test").Options;

            // Declaring variables for testing
            Orders currentOrder = new Orders();
            IList<OrderEntries> orderDetails = null;
            bool allReturnedEntriesAreForSelectedOrder = true;
            int numOfOrderItems = 4;
            int countedOrderItems = -1;

            using (var db = new _1811proj0Context(options))
            {

                db.Orders.Add(new Orders
                {   
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = currentOrder.OId,
                    Pizza = 2,
                    Quantity = 2
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = currentOrder.OId,
                    Pizza = 5,
                    Quantity = 1
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = currentOrder.OId,
                    Pizza = 3,
                    Quantity = 1
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                orderDetails = repo.ShowOrderDetails(currentOrder);

                foreach (var item in orderDetails)
                {
                    if(item.OnOrder != currentOrder.OId)
                    {
                        allReturnedEntriesAreForSelectedOrder = false;
                    }
                }

                countedOrderItems = orderDetails.Sum(m => m.Quantity);
            }

            // Assert
            using(var db = new _1811proj0Context(options))
            {
                Assert.True(orderDetails.Count() > 0);
                Assert.True(allReturnedEntriesAreForSelectedOrder);
                Assert.Equal(numOfOrderItems, countedOrderItems);
            }
        }

        [Fact]
        public void TooEarlyCheckWorks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("tooearly_test").Options;

                // Declare variables for testing - these should ultimately return the opposite boolean value given in the declaration
            bool user1OrderedTooSoon = false;
            bool user2OrderedTooSoon = true;
            bool user3OrderedTooSoon = true;

            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add users to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });

                db.Users.Add(new Users { UId = 2, FirstName = "", LastName = "", DefaultLocation = 1 });

                db.Users.Add(new Users { UId = 3, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add orders to in-memory database to be tested
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 2,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now.AddHours(-2),
                    TotalPrice = 0M
                });

                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 3,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now.AddDays(-1),
                    TotalPrice = 0M
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                Orders user1Order = db.Orders.Where(m => m.OrderFor == 1).Last();
                Orders user2Order = db.Orders.Where(m => m.OrderFor == 2).Last();
                Orders user3Order = db.Orders.Where(m => m.OrderFor == 3).Last();

                user1OrderedTooSoon = repo.TooEarly(user1Order.OrderFor, user1Order.OrderedFrom);
                user2OrderedTooSoon = repo.TooEarly(user2Order.OrderFor, user2Order.OrderedFrom);
                user3OrderedTooSoon = repo.TooEarly(user3Order.OrderFor, user3Order.OrderedFrom);
            }

            // Assert
            Assert.True(user1OrderedTooSoon);
            Assert.False(user2OrderedTooSoon);
            Assert.False(user3OrderedTooSoon);
        }

        [Fact]
        public void ValidOrderSizeWorks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("validordersize_test").Options;

            // Declare variables for testing - these should ultimately return the opposite boolean value given in the declaration
            bool order1ValidSize = false;
            bool order2ValidSize = true;
            bool order3ValidSize = false;
            bool order4ValidSize = true;

            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();


                // Add orders to in-memory database to be tested/referenced
                // This one will have 12 or less pizzas and should return valid
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have more than 12 pizzas and should return invalid
                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have exactly 12 pizza and should return valid
                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have more than 12 pizza total across multiple types of pizza and should return invalid
                db.Orders.Add(new Orders
                {   
                    OId = 4,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();


                // Add order entries to in-memory database to populate the orders being tested
                // Order 1 entry - less than 12 pizzas
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = 1,
                    Pizza = 1,
                    Quantity = 2
                });

                // Order 2 entry - more than 12 pizzas
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 15
                });

                // Order 3 entry - exactly 12 pizzas
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = 3,
                    Pizza = 1,
                    Quantity = 12
                });

                // Order 4 entries - more than 12 pizzas across multiple entries
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 4,
                    OnOrder = 4,
                    Pizza = 1,
                    Quantity = 6
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 5,
                    OnOrder = 4,
                    Pizza = 2,
                    Quantity = 7
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                order1ValidSize = repo.ValidOrderSize(db.Orders.Find(1));
                order2ValidSize = repo.ValidOrderSize(db.Orders.Find(2));
                order3ValidSize = repo.ValidOrderSize(db.Orders.Find(3));
                order4ValidSize = repo.ValidOrderSize(db.Orders.Find(4));
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.True(order1ValidSize);
                Assert.True(order3ValidSize);
                Assert.False(order2ValidSize);
                Assert.False(order4ValidSize);
            }
        }

        [Fact]
        public void ValidOrderPriceWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("validorderprice_test").Options;

            // Declare variables for testing - these should ultimately return the opposite boolean value given in the declaration
            bool order1ValidPrice = false;
            bool order2ValidPrice = true;

            // Arrange
            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 49.99M });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 44.99M });
                db.SaveChanges();


                // Add orders to in-memory database to be tested/referenced
                // This one will cost $500 or less and should return valid
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will cost more than $500 and should return invalid
                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will cost more than $500 across multiple entries and should return invalid
                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();

                // Add order entries to in-memory database to populate the orders being tested
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = 1,
                    Pizza = 2,
                    Quantity = 11
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 11
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = 3,
                    Pizza = 1,
                    Quantity = 6
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 4,
                    OnOrder = 3,
                    Pizza = 2,
                    Quantity = 5
                });
                db.SaveChanges();     
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                order1ValidPrice = repo.ValidOrderPrice(db.Orders.Find(1));
                order2ValidPrice = repo.ValidOrderPrice(db.Orders.Find(2));
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.True(order1ValidPrice);
                Assert.False(order2ValidPrice);
            }
        }

        [Fact]
        public void EnoughIngredientsWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("enoughingredients_test").Options;

            bool order1EnoughIngredients = false;
            bool order2EnoughIngredients = true;
            bool order3EnoughIngredients = true;

            // Arrange
            using (var db = new _1811proj0Context(options))
            {
                // Add locations to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.Locations.Add(new Locations { LId = 2, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 0 });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 0 });
                db.SaveChanges();

                // Add toppings to be referenced
                db.Toppings.Add(new Toppings { TId = 1, Name = "" });
                db.Toppings.Add(new Toppings { TId = 2, Name = "" });
                db.Toppings.Add(new Toppings { TId = 3, Name = "" });
                db.Toppings.Add(new Toppings { TId = 4, Name = "" });
                db.SaveChanges();

                // Add toppings to pizzas
                db.PizzaToppings.Add(new PizzaToppings { PtId = 1, Pizza = 1, Topping = 1, Quantity = 2 });
                db.PizzaToppings.Add(new PizzaToppings { PtId = 2, Pizza = 1, Topping = 2, Quantity = 2 });
                db.PizzaToppings.Add(new PizzaToppings { PtId = 3, Pizza = 1, Topping = 3, Quantity = 2 });
                db.PizzaToppings.Add(new PizzaToppings { PtId = 4, Pizza = 1, Topping = 4, Quantity = 2 });
                db.PizzaToppings.Add(new PizzaToppings { PtId = 5, Pizza = 2, Topping = 2, Quantity = 6 });
                db.PizzaToppings.Add(new PizzaToppings { PtId = 6, Pizza = 2, Topping = 4, Quantity = 6 });
                db.SaveChanges();

                // Add location inventories
                db.LocationInventory.Add(new LocationInventory { LiId = 1, Location = 1, Topping = 1, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 2, Location = 1, Topping = 2, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 3, Location = 1, Topping = 3, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 4, Location = 1, Topping = 4, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 5, Location = 2, Topping = 1, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 6, Location = 2, Topping = 2, Quantity = 10 });
                db.LocationInventory.Add(new LocationInventory { LiId = 7, Location = 2, Topping = 3, Quantity = 1 });
                db.LocationInventory.Add(new LocationInventory { LiId = 8, Location = 2, Topping = 4, Quantity = 7 });
                db.SaveChanges();

                // Add orders to in-memory database
                // This one will order from a location with enough ingredients and should return valid
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will order from a location without enough ingredients and should return invalid
                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 1,
                    OrderedFrom = 2,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will order from a location without enough ingredients for all piizas and should return invalid
                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 1,
                    OrderedFrom = 2,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();


                // Add order entries to in-memory database
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = 1,
                    Pizza = 1,
                    Quantity = 1
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 1
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = 3,
                    Pizza = 1,
                    Quantity = 1
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 4,
                    OnOrder = 3,
                    Pizza = 2,
                    Quantity = 1
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                order1EnoughIngredients = repo.EnoughIngredients(db.Orders.Find(1));
                order2EnoughIngredients = repo.EnoughIngredients(db.Orders.Find(2));
                order3EnoughIngredients = repo.EnoughIngredients(db.Orders.Find(3));
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.True(order1EnoughIngredients);
                Assert.False(order2EnoughIngredients);
                Assert.False(order3EnoughIngredients);
            }
        }

        [Fact]
        public void GetOrderTotalWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("getordertotal_test").Options;

            // Declare variables for testing - these should change from -1 by the end of the test
            decimal order1TotalPrice = -1;
            decimal order2TotalPrice = -1;
            decimal order3TotalPrice = -1;

            // Arrange
            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 49.99M });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 44.99M });
                db.SaveChanges();


                // Add orders to in-memory database to be tested/referenced
                // This one will cost have one lnid of pizza on it
                db.Orders.Add(new Orders
                {
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have multiple kinds of pizza on it
                db.Orders.Add(new Orders
                {
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();

                // Add order entries to in-memory database to populate the orders being tested
                db.OrderEntries.Add(new OrderEntries
                {
                    OnOrder = 1,
                    Pizza = 1,
                    Quantity = 3
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 2
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OnOrder = 2,
                    Pizza = 2,
                    Quantity = 1
                });
                db.SaveChanges();         
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                order1TotalPrice = repo.GetOrderTotal(db.Orders.Find(1));
                order2TotalPrice = repo.GetOrderTotal(db.Orders.Find(2));
                
                // setup to test that the total of a newly created order with no items; should total 0 (zero)
                repo.CreateOrder(1, 1);
                db.SaveChanges();

                order3TotalPrice = repo.GetOrderTotal(db.Orders.Last());
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.True(order1TotalPrice.Equals(149.97M));
                Assert.True(order2TotalPrice.Equals(144.97M));
                Assert.True(order3TotalPrice.Equals(0M));
            }
        }

        [Fact]
        public void CreateOrderEntryWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("createorderentry_test").Options;

            // Arrange
            using (var db = new _1811proj0Context(options))
            {
                db.Locations.Add(new Locations { LId = 1, City = "Test", State = "Location" });
                db.SaveChanges();

                db.Users.Add(new Users { UId = 1, FirstName = "Sample", LastName = "User", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 49.99M });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 44.99M });
                db.SaveChanges();

                // Add order to in-memory database
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                repo.CreateOrderEntry(1, 1, 2);
                repo.CreateOrderEntry(1, 2, 3);
                db.SaveChanges();
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                IList<OrderEntries> orderEntries = db.OrderEntries.Include(m => m.OnOrderNavigation).Where(m => m.OnOrder == 1).ToList();

                Assert.True(orderEntries[0].OnOrder == 1);
                Assert.True(orderEntries[1].OnOrder == 1);
                Assert.True(orderEntries[0].Pizza == 1);
                Assert.True(orderEntries[1].Pizza == 2);
                Assert.True(orderEntries[0].Quantity == 2);
                Assert.True(orderEntries[1].Quantity == 3);
                Assert.True(orderEntries[0].OnOrderNavigation.OrderFor == 1);
                Assert.True(orderEntries[1].OnOrderNavigation.OrderFor == 1);
                Assert.True(orderEntries[0].OnOrderNavigation.OrderedFrom == 1);
                Assert.True(orderEntries[1].OnOrderNavigation.OrderedFrom == 1);
            }
        }

        [Fact]
        public void GetNumberOfOrderItemsWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("getnumberoforderitems_test").Options;

            // Declare variables for testing - these should change from -1 by the end of the test
            int itemsOnOrder1 = -1;
            int itemsOnOrder2 = -1;
            int itemsOnOrder3 = -1;

            // Arrange
            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 49.99M });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 44.99M });
                db.SaveChanges();


                // Add orders to in-memory database to be tested/referenced
                // This order will have 5 items on it
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have 7 items on it across two order entries
                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have no items on it (no order entries associated with the order)
                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();

                // Add order entries to in-memory database to populate the orders being tested
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = 1,
                    Pizza = 2,
                    Quantity = 5
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 4
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = 2,
                    Pizza = 2,
                    Quantity = 3
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                itemsOnOrder1 = repo.GetNumberOfOrderItems(db.Orders.Find(1));
                itemsOnOrder2 = repo.GetNumberOfOrderItems(db.Orders.Find(2));
                itemsOnOrder3 = repo.GetNumberOfOrderItems(db.Orders.Find(3));
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.True(itemsOnOrder1 == 5);
                Assert.True(itemsOnOrder2 == 7);
                Assert.True(itemsOnOrder3 == 0);
            }
        }

        [Fact]
        public void CancelOrderWorks()
        {
            var options = new DbContextOptionsBuilder<_1811proj0Context>().UseInMemoryDatabase("cancelorder_test").Options;

            using (var db = new _1811proj0Context(options))
            {
                // Add location to in-memory database to be referenced
                db.Locations.Add(new Locations { LId = 1, City = "", State = "" });
                db.SaveChanges();

                // Add user to in-memory database to be referenced
                db.Users.Add(new Users { UId = 1, FirstName = "", LastName = "", DefaultLocation = 1 });
                db.SaveChanges();

                // Add pizzas to in-memory database to be referenced
                db.Pizzas.Add(new Pizzas { PId = 1, Name = "", Price = 49.99M });
                db.Pizzas.Add(new Pizzas { PId = 2, Name = "", Price = 44.99M });
                db.SaveChanges();


                // Add orders to in-memory database to be tested/referenced
                // This order will have 1 order entry to be removed
                db.Orders.Add(new Orders
                {
                    OId = 1,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have 2 order entries to be removed
                db.Orders.Add(new Orders
                {
                    OId = 2,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });

                // This one will have no order entries to be removed
                db.Orders.Add(new Orders
                {
                    OId = 3,
                    OrderFor = 1,
                    OrderedFrom = 1,
                    OrderedAt = DateTime.Now,
                    TotalPrice = 0M
                });
                db.SaveChanges();

                // Add order entries to in-memory database to populate the orders being tested
                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 1,
                    OnOrder = 1,
                    Pizza = 2,
                    Quantity = 5
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 2,
                    OnOrder = 2,
                    Pizza = 1,
                    Quantity = 4
                });

                db.OrderEntries.Add(new OrderEntries
                {
                    OeId = 3,
                    OnOrder = 2,
                    Pizza = 2,
                    Quantity = 3
                });
                db.SaveChanges();
            }

            // Act
            using (var db = new _1811proj0Context(options))
            {
                var repo = new PizzaRepository(db);

                repo.CancelOrder(db.Orders.Find(1));
                repo.CancelOrder(db.Orders.Find(2));

                // Attmpet to cancel an order with no order entries (this still needs to work in order to actually cancel the order)
                repo.CancelOrder(db.Orders.Find(3));
            }

            // Assert
            using (var db = new _1811proj0Context(options))
            {
                Assert.Null(db.OrderEntries.Find(1));
                Assert.Null(db.OrderEntries.Find(2));
                Assert.Null(db.OrderEntries.Find(3));
                Assert.Null(db.Orders.Find(1));
                Assert.Null(db.Orders.Find(2));
                Assert.Null(db.Orders.Find(3));

                // Ask if there's a way to set this up to test that it throws an exceptions
                // Assert.Throws<ArgumentException>(repo.CancelOrder(db.Orders.Find(4)));
            }
        }
    }
}
