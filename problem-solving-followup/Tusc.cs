using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace problem_solving_followup
{
    public class Tusc
    {
        public static void Start(List<User> users, List<Product> products, List<Transaction> transactions)
        {
            // Write welcome message
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");

            // Login
            Login:

            // Prompt for user input
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            string name = Console.ReadLine();

            // Validate Username
            bool isValidUser = false; // Is valid user?
            if (!string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < 5; i++)
                {
                    User user = users[i];
                    // Check that name matches
                    if (user.Name == name)
                    {
                        isValidUser = true;
                    }
                }

                // if valid user
                if (isValidUser)
                {
                    // Prompt for user input
                    Console.WriteLine("Enter Password:");
                    string password = Console.ReadLine();

                    // Validate Password
                    bool isValidPassword = false; // Is valid password?
                    for (int i = 0; i < 5; i++)
                    {
                        User user = users[i];

                        // Check that name and password match
                        if (user.Name == name && user.Password == password)
                        {
                            isValidPassword = true;
                        }
                    }

                    // if valid password
                    if (isValidPassword == true)
                    {
                        // Show welcome message
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine("Login successful! Welcome " + name + "!");
                        Console.ResetColor();
                        
                        // Show remaining balance
                        double balance = 0;
                        foreach (var user in users)
                        {
                            // Check that name and password match
                            if (user.Name == name && user.Password == password)
                            {
                                balance = user.Balance;

                                // Show balance 
                                Console.WriteLine();
                                Console.WriteLine("Your balance is " + user.Balance.ToString("C"));
                            }
                        }

                        // Show product list
                        while (true)
                        {
                            // Prompt for user input
                            Console.WriteLine();
                            Console.WriteLine("What would you like to buy?");
                            for (int i = 0; i < products.Count; i++)
                            {
                                Product product = products[i];
                                Console.WriteLine(i + 1 + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
                            }
                            Console.WriteLine(products.Count + 1 + ": Exit");

                            // Prompt for user input
                            Console.WriteLine("Enter a number:");
                            string answer = Console.ReadLine();
                            int num = Convert.ToInt32(answer);
                            num = num - 1;

                            // Check if user entered number that equals product count
                            if (num == products.Count)
                            {
                                // Update balance
                                foreach (var user in users)
                                {
                                    // Check that name and password match
                                    if (user.Name == name && user.Password == password)
                                    {
                                        user.Balance = balance;
                                    }
                                }

                                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                                File.WriteAllText(@"Data/Users.json", json);

                                string json2 = JsonConvert.SerializeObject(products, Formatting.Indented);
                                File.WriteAllText(@"Data/Products.json", json2);

                                //transactions = GenerateRandomTransactions(users, products);

                                string json3 = JsonConvert.SerializeObject(transactions, Formatting.Indented);
                                File.WriteAllText(@"Data/Transactions.json", json3);

                                // Prevent console from closing
                                Console.WriteLine();
                                Console.WriteLine("Press Enter key to exit");
                                Console.ReadLine();
                                return;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("You want to buy: " + products[num].Name);
                                Console.WriteLine("Your balance is " + balance.ToString("C"));

                                // Prompt for user input
                                Console.WriteLine("Enter amount to purchase:");
                                answer = Console.ReadLine();
                                int quantity = Convert.ToInt32(answer);

                                // Check if balance - quantity * price is less than 0
                                if (balance - products[num].Price * quantity < 0)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine("You do not have enough money to buy that.");
                                    Console.ResetColor();
                                    continue;
                                }

                                if (products[num].Quantity < quantity)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine("Sorry, " + products[num].Name + " is out of stock");
                                    Console.ResetColor();
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (quantity > 0)
                                {
                                    balance = balance - products[num].Price * quantity;

                                    products[num].Quantity = products[num].Quantity - quantity;

                                    transactions.Add(new Transaction
                                    {
                                        UserName = name,
                                        ProductName = products[num].Name,
                                        Quantity = quantity,
                                        Date = DateTime.Now
                                    });

                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("You bought " + quantity + " " + products[num].Name);
                                    Console.WriteLine("Your new balance is " + balance.ToString("C"));
                                    Console.ResetColor();
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine();
                                    Console.WriteLine("Purchase cancelled");
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine();
                        Console.WriteLine("You entered an invalid password.");
                        Console.ResetColor();

                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("You entered an invalid user.");
                    Console.ResetColor();

                    goto Login;
                }
            }

            // Prevent console from closing
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }

        private static List<Transaction> GenerateRandomTransactions(List<User> users, List<Product> products)
        {
            var list = new List<Transaction>();

            DateTime start = DateTime.Parse("2015/01/01");
            DateTime end = DateTime.Now;
            Random r = new Random();

            for (int i = 0; i < 2000; i++)
            {
                long randTicks = r.Next(0, (int)((end.Ticks - start.Ticks) / 100000000));

                list.Add(new Transaction
                    {
                        UserName = users.ElementAt(r.Next(0, users.Count())).Name,
                        ProductName = products.ElementAt(r.Next(0, users.Count())).Name,
                        Quantity = r.Next(1, 3),
                        Date = start.AddTicks(randTicks * 100000000)
                    });
            }

            return list.OrderBy(l => l.Date).ToList();
        }
    }
}
