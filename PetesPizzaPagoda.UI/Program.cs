using Microsoft.EntityFrameworkCore;
using PetesPizzaPagoda.DataAccess;
using System;
using System.Collections.Generic;

namespace PetesPizzaPagoda.UI
{
    class Program
    {
        static DbContextOptions<_1811proj0Context> options = null;

        public static void Main(string[] args)
        {
            var connectionString = SecretConfiguration.ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<_1811proj0Context>();
            optionsBuilder.UseSqlServer(connectionString);
            options = optionsBuilder.Options;

            var db = new _1811proj0Context(options);
            var repo = new PizzaRepository(db);

            int activeProgram = 1;

            while (activeProgram == 1)
            {
                Console.WriteLine();
                Console.WriteLine("- - - - - - - - - - ");
                Console.WriteLine("Pete's Pizza Pagoda");
                Console.WriteLine();

                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1) Place an order");
                Console.WriteLine("2) Show all users by name");
                Console.WriteLine("3) Show order history");
                Console.WriteLine();

                Console.WriteLine("4) Exit application");
                Console.WriteLine();

                var input = Console.ReadLine();

                switch (input)
                {
                    // Place an order
                    case "1":
                        Console.WriteLine();
                        Console.WriteLine("- - - - - - - - - - ");
                        Console.WriteLine("Choose a user to place an order for:");
                        Console.WriteLine();

                        int userCount = 1;
                        var systemUsers = repo.AllUsersByName;

                        // Display all users
                        foreach (var item in systemUsers)
                        {
                            Console.WriteLine($"{userCount}) {item.LastName}, {item.FirstName}");
                            userCount++;
                        }

                        Console.WriteLine();
                        input = Console.ReadLine();

                        int selectedIndex = -1;
                    
                        // Check that input is a number
                        if (!Int32.TryParse(input,out selectedIndex))
                        {
                            Console.WriteLine("Please enter the number of a user from the list");  
                        }
                        // Check if the input number is a number on the given list
                        else if (selectedIndex > systemUsers.Count || selectedIndex <= 0)
                        {
                            Console.WriteLine("User could not be found");                               
                        }
                        // Number is valid, get user
                        else
                        {
                            var activeUser = repo.GetUserById(systemUsers[selectedIndex - 1].UId);
                            int loggedIn = 1;

                            while(loggedIn == 1)
                            {
                                Console.WriteLine();
                                Console.WriteLine("- - - - - - - - - - ");
                                Console.WriteLine($"Welcome, {activeUser.FirstName} {activeUser.LastName}!");
                                Console.WriteLine("How can we help you today?");
                                Console.WriteLine();

                                Console.WriteLine("1) Place a new order");
                                Console.WriteLine("2) View order history");
                                Console.WriteLine();

                                Console.WriteLine("3) Logout");
                                input = Console.ReadLine();

                                switch (input)
                                {
                                    // Place order for selected user
                                    case "1":
                                        int placingOrder = 1;

                                        repo.CreateOrder(activeUser.UId, activeUser.DefaultLocation);

                                        while (placingOrder == 1)
                                        {
                                            
                                            Console.WriteLine();
                                            Console.WriteLine("- - - - - - - - - - ");
                                            Console.WriteLine($"New order for {activeUser.FirstName} {activeUser.LastName}");
                                            Console.WriteLine("| Pizza | Price | Quantity |");
                                            Console.WriteLine("-------------------------------------------------------");

                                            var currentOrder = repo.GetWorkingOrder(activeUser.UId);
                                            foreach (var item in repo.ShowOrderDetails(currentOrder))
                                            {
                                                Console.WriteLine($"| {item.PizzaNavigation.Name} | ${item.PizzaNavigation.Price} |  {item.Quantity} |");
                                            }
                                            Console.WriteLine("-------------------------------------------------------");
                                            Console.WriteLine();

                                            Console.WriteLine("1) Add entry");
                                            Console.WriteLine("2) Remove entry");
                                            Console.WriteLine();
                                            Console.WriteLine("3) Place order");
                                            Console.WriteLine("4) Cancel order");
                                            Console.WriteLine();

                                            input = Console.ReadLine();

                                            switch (input)
                                            {
                                                case "1":
                                                    int addingPizza = 1;
                                                    while (addingPizza == 1)
                                                    {
                                                        Console.WriteLine();
                                                        Console.WriteLine("- - - - - - - - - - ");
                                                        Console.WriteLine("What kind of pizza would you like");
                                                        Console.WriteLine("-------------------------------------------------------");

                                                        foreach (var item in repo.ListAllPizzas())
                                                        {
                                                            Console.WriteLine($"{item.PId}) {item.Name}, ${item.Price}");
                                                        }
                                                        Console.WriteLine("-------------------------------------------------------");
                                                        Console.WriteLine();

                                                        input = Console.ReadLine();

                                                        int pizza = -1;
                                                        bool inputIsInt = Int32.TryParse(input, out pizza);
                                                        bool validId = false;

                                                        foreach (var item in repo.ListAllPizzas())
                                                        {
                                                            if (item.PId == pizza)
                                                            {
                                                                validId = true;
                                                            }
                                                        }

                                                        // Check that input is a number
                                                        if (!inputIsInt)
                                                        {
                                                            Console.WriteLine("Please enter the number of a pizza from the list");
                                                        }
                                                        else if (!validId)
                                                        {
                                                            Console.WriteLine("Pizza could not be found");
                                                        }
                                                        else
                                                        {
                                                            int getNumber = 1;
                                                            while (getNumber == 1)
                                                            {
                                                                Console.WriteLine("How many would you like to add?");
                                                                Console.WriteLine();

                                                                input = Console.ReadLine();

                                                                int quantity = -1;
                                                                if (!Int32.TryParse(input, out quantity))
                                                                {
                                                                    Console.WriteLine("Please enter a number");
                                                                    Console.WriteLine();
                                                                    Console.WriteLine("- - - - - - - - - - ");
                                                                }
                                                                else
                                                                {
                                                                    repo.CreateOrderEntry(currentOrder.OId, pizza, quantity);
                                                                    repo.SaveChanges();
                                                                    getNumber = 0;
                                                                }
                                                            }
                                                        }
                                                        addingPizza = 0;
                                                    }
                                                    break;

                                                case "2":
                                                    if(repo.ShowOrderDetails(currentOrder).Count > 0)
                                                    {
                                                        Console.WriteLine();
                                                        Console.WriteLine("- - - - - - - - - - ");
                                                        Console.WriteLine("Which entry would you like to remove?");
                                                        Console.WriteLine("| ID | Pizza | Price | Quantity |");

                                                        foreach (var item in repo.ShowOrderDetails(currentOrder))
                                                        {
                                                            Console.WriteLine($"| {item.OeId} | {item.PizzaNavigation.Name} | ${item.PizzaNavigation.Price} |  {item.Quantity} |");
                                                        }
                                                        Console.WriteLine("-------------------------------------------------------");
                                                        Console.WriteLine();

                                                        input = Console.ReadLine();

                                                        int entry = -1;
                                                        bool inputIsInt = Int32.TryParse(input, out entry);
                                                        bool validId = false;

                                                        foreach (var item in repo.ShowOrderDetails(currentOrder))
                                                        {
                                                            if (item.OeId == entry)
                                                            {
                                                                validId = true;
                                                            }
                                                        }

                                                        // Check that input is a number
                                                        if (!inputIsInt)
                                                        {
                                                            Console.WriteLine("Please enter the ID of the entry you wish to remove");
                                                            Console.WriteLine();
                                                        }
                                                        else if (!validId)
                                                        {
                                                            Console.WriteLine("Entry could not be found");
                                                            Console.WriteLine();
                                                        }
                                                        else
                                                        {
                                                            repo.DeleteOrderEntry(entry);
                                                            repo.SaveChanges();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("This order has no entries!");
                                                        Console.WriteLine();
                                                        Console.WriteLine("[press any key to continue]");
                                                        Console.ReadLine();
                                                    }
                                                    
                                                    break;

                                                case "3":
                                                    if (repo.ValidOrderPrice(currentOrder))
                                                    {
                                                        if (repo.ValidOrderSize(currentOrder))
                                                        {
                                                            currentOrder.OrderedAt = DateTime.Now;
                                                            currentOrder.TotalPrice = repo.GetOrderTotal(currentOrder);
                                                            currentOrder.NumOfOrderedItems = repo.GetNumberOfOrderItems(currentOrder);

                                                            repo.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Order must have at least 1 pizza, but no more than 12");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Total order price cannot exceed $500");
                                                    }

                                                    placingOrder = 0;
                                                    Console.WriteLine();
                                                    Console.WriteLine("Order placed!");
                                                    Console.WriteLine("[Press any key to continue]");

                                                    Console.ReadLine();
                                                    break;

                                                case "4":
                                                    placingOrder = 0;
                                                    repo.CancelOrder(currentOrder);
                                                    repo.SaveChanges();
                                                    break;

                                                default:
                                                    Console.WriteLine();
                                                    Console.WriteLine("Command not recognized");
                                                    Console.WriteLine();
                                                    break;
                                            }
                                        }


                                        break;
                                    
                                    // Get user's order history
                                    case "2":
                                        Console.WriteLine();
                                        Console.WriteLine("- - - - - - - - - - ");
                                        Console.WriteLine($"{activeUser.FirstName} {activeUser.LastName}'s order history:");
                                        DisplayOrders(repo.GetUsersOrderHistory(activeUser.UId), repo);
                                        break;

                                    // Log user out
                                    case "3":
                                        loggedIn = 0;
                                        break;

                                    default:
                                        Console.WriteLine();
                                        Console.WriteLine("Command not recognized");
                                        Console.WriteLine();
                                        break;
                                }
                            }
                        }
                        break;

                    // Show all users by name
                    case "2":
                        Console.WriteLine();
                        Console.WriteLine("- - - - - - - - - - ");
                        Console.WriteLine("All users by name:");
                        Console.WriteLine("| ID | Last Name | First Name | Default Location |");
                        Console.WriteLine("-------------------------------------------------------");

                        foreach (var item in repo.AllUsersByName)
                        {
                            Console.WriteLine($"| {item.UId} | {item.LastName} | {item.FirstName} | {item.DefaultLocation} |");
                        }

                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine();

                        Console.WriteLine("[Press any key to return to previous menu]");
                        Console.ReadLine();
                        break;

                    // Display show orders menu
                    case "3":
                        int viewOrders = 1;

                        while (viewOrders == 1)
                        {
                            Console.WriteLine();
                            Console.WriteLine("- - - - - - - - - - ");
                            Console.WriteLine("How would you like to view the orders?");
                            Console.WriteLine();

                            Console.WriteLine("1) By earliest order");
                            Console.WriteLine("2) By latest order");
                            Console.WriteLine("3) By cheapest order");
                            Console.WriteLine("4) By most expensive order");
                            Console.WriteLine();

                            Console.WriteLine("5) Return to main menu");
                            Console.WriteLine();

                            input = Console.ReadLine();

                            switch (input)
                            {
                                // Show all orders by earliest
                                case "1":
                                    Console.WriteLine();
                                    Console.WriteLine("- - - - - - - - - - ");
                                    Console.WriteLine("All orders by earliest:");
                                    DisplayOrders(repo.GetOrderHistoryByEarliest(), repo);
                                    break;

                                // Show all orders by latest
                                case "2":
                                    Console.WriteLine();
                                    Console.WriteLine("- - - - - - - - - - ");
                                    Console.WriteLine("All orders by latest:");
                                    DisplayOrders(repo.GetOrderHistoryByLatest(), repo);
                                    break;

                                // Show all orders by cheapest
                                case "3":
                                    Console.WriteLine();
                                    Console.WriteLine("- - - - - - - - - - ");
                                    Console.WriteLine("All orders by cheapest:");
                                    DisplayOrders(repo.GetOrderHistoryByCheapest(), repo);
                                    break;

                                // Show all orders by most expensive
                                case "4":
                                    Console.WriteLine();
                                    Console.WriteLine("- - - - - - - - - - ");
                                    Console.WriteLine("All orders by most expensive:");
                                    DisplayOrders(repo.GetOrderHistoryByMostExpensive(), repo);
                                    break;

                                // Return to Main Menu
                                case "5":
                                    viewOrders = 0;
                                    Console.WriteLine();
                                    Console.WriteLine("- - - - - - - - - - ");
                                    break;

                                default:
                                    Console.WriteLine();
                                    Console.WriteLine("Command not recognized");
                                    Console.WriteLine();
                                    break;
                            }
                        }

                        break;

                    // Exit Program
                    case "4":
                        activeProgram = 0;
                        Console.WriteLine();
                        Console.WriteLine("Exiting program...");
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("Command not recognized");
                        Console.WriteLine();
                        break;
                }
            }
        }

        private static void DisplayOrders(IList<Orders> orders, PizzaRepository repo)
        {
            int orderDisplay = 1;

            while (orderDisplay == 1)
            {
                Console.WriteLine("| ID | User | Location | Date/Time | Total  | Items |");
                Console.WriteLine("-------------------------------------------------------");

                foreach (var item in orders)
                {
                    Console.WriteLine($"| {item.OId} | {item.OrderFor} | {item.OrderedFrom} | {item.OrderedAt} | {item.TotalPrice} | {item.NumOfOrderedItems} |");
                }

                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();

                Console.WriteLine("1) Display details of an order");
                Console.WriteLine();
                Console.WriteLine("2) Return to previous menu");
                Console.WriteLine();

                var input = Console.ReadLine();

                switch(input)
                {
                    case "1":
                        Console.WriteLine();
                        Console.WriteLine("- - - - - - - - - - ");
                        Console.WriteLine("Enter the ID of the order to display details");
                        Console.WriteLine();

                        input = Console.ReadLine();

                        int selectedId = -1;

                        bool inputIsInt = Int32.TryParse(input, out selectedId);
                        bool validId = false;

                        foreach(var item in orders)
                        {
                            if(item.OId == selectedId)
                            {
                                validId = true;
                            }
                        }

                        // Check that input is a number
                        if (!inputIsInt)
                        {
                            Console.WriteLine("Please enter the ID of an order from the list");
                        }
                        else if (!validId)
                        {
                            Console.WriteLine("Order could not be found");
                        }
                        // Number is valid, get order
                        else
                        {
                            var orderDetails = repo.GetOrderById(selectedId);

                            Console.WriteLine();
                            Console.WriteLine("- - - - - - - - - - ");
                            Console.WriteLine($"Showing details for Order {orderDetails.OId}:");
                            Console.WriteLine("| Order | Pizza | Price | Quantity |");
                            Console.WriteLine("-------------------------------------------------------");

                            foreach (var item in repo.ShowOrderDetails(orderDetails))
                            {
                                Console.WriteLine($"| {item.OnOrder} | {item.PizzaNavigation.Name} | ${item.PizzaNavigation.Price} |  {item.Quantity} |");
                            }

                            Console.WriteLine("-------------------------------------------------------");
                            Console.WriteLine();

                            Console.WriteLine("[Press any key to return]");
                            Console.WriteLine();

                            Console.ReadLine();
                        }
                            break;

                    case "2":
                        orderDisplay = 0;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("Command not recognized");
                        Console.WriteLine();
                        break;
                }
            } 
        }
    }
}


