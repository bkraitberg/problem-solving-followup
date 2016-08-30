using System;
using System.Text;
using System.Collections.Generic;
using problem_solving_followup;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnitTestProject
{
    [TestFixture]
    public class LinqUnitTests
    {
        private List<User> users;
        private List<User> originalUsers;
        private List<Product> products;
        private List<Product> originalProducts;
        private List<Transaction> transactions;
        private List<Transaction> originalTransactions;

        [SetUp]
        public void Test_Initialize()
        {
            // Load users from data file
            originalUsers = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(@"Data/Users.json"));
            users = DeepCopy<List<User>>(originalUsers);

            // Load products from data file
            originalProducts = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(@"Data/Products.json"));
            products = DeepCopy<List<Product>>(originalProducts);

            // Load transactions from data file
            originalTransactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(@"Data/Transactions.json"));
            transactions = DeepCopy<List<Transaction>>(originalTransactions);
        }

        [TearDown]
        public void Test_Cleanup()
        {
            // Restore users
            string json = JsonConvert.SerializeObject(originalUsers, Formatting.Indented);
            File.WriteAllText(@"Data/Users.json", json);
            users = DeepCopy<List<User>>(originalUsers);

            // Restore products
            string json2 = JsonConvert.SerializeObject(originalProducts, Formatting.Indented);
            File.WriteAllText(@"Data/Products.json", json2);
            products = DeepCopy<List<Product>>(originalProducts);

            // Restore transactions
            string json3 = JsonConvert.SerializeObject(originalTransactions, Formatting.Indented);
            File.WriteAllText(@"Data/Transactions.json", json3);
            transactions = DeepCopy<List<Transaction>>(originalTransactions);
        }

        [Test]
        public void Test_WhatIsTheTotalNumberOfTransactions()
        {
            var result = transactions.Count();

            Assert.AreEqual(2000, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchased()
        {
            //var result = transactions.Select(a => a.Quantity).Sum(); // TODO
            var result = transactions.Sum(a => a.Quantity);

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            var result = transactions.Where(a => a.Date.Year == 2016).Sum(q => q.Quantity); // TODO

            Assert.AreEqual(1160, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
            //var result = transactions.Where(a => a.Date.Year == 2016 && a.Date.Month == 08 && a.Date.Day >= 23).Sum(q => q.Quantity); // TODO
            var result = transactions.Where(a => a.Date >= DateTime.Now.AddDays(-8)).Sum(q => q.Quantity); // TODO
            Assert.AreEqual(32, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            var result = transactions.Where(a => a.Quantity > 1).Count();// TODO

            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
            var result = transactions.Where(a => a.Date.DayOfWeek == DayOfWeek.Sunday).Count(); // TODO

            Assert.AreEqual(267, result);
        }

        [Test]
        public void Test_WhatIsTheAverageQuantityPurchased()
        {

            var result = transactions.Average(a => a.Quantity); // TODO

            Assert.AreEqual(1.5005, result, 0.0001);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHaveBeenBought()
        {
            var result = transactions.Where(a => a.ProductName == "Chips").Sum(q => q.Quantity); // TODO

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
            var result = transactions.Where(a => a.ProductName == "Chips" && a.UserName == "Jason").Sum(q => q.Quantity); // TODO

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
            var result = transactions.Where(a => a.ProductName == "Chips" && a.UserName == "Jason" && a.Date.Year == 2015).Sum(q => q.Quantity); // TODO

            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
            var result = transactions.Where(a => a.ProductName == "Chips" && a.UserName == "Jason" && a.Date.Year == 2016 && a.Date.Month == 05).Sum(q => q.Quantity); // TODO

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            var result = transactions.GroupBy(a => new { a.ProductName, a.Date, a.Quantity })
                        .Where(r => r.Key.Date.TimeOfDay >= new TimeSpan(12, 0, 0) && r.Key.Date.TimeOfDay <= new TimeSpan(13, 0, 0))
                         .OrderByDescending(r => r.Sum(b => b.Quantity)).FirstOrDefault().Key.ProductName;

            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {

            var result = transactions
                        .GroupBy(a => new { a.ProductName, a.Quantity })
                        .OrderByDescending(r => r.Sum(b => b.Quantity)).Last().Key.ProductName;

            Assert.AreEqual("Cookies", result);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            var result = transactions
                          .GroupBy(a => new { a.UserName, a.ProductName, a.Quantity })
                          .Where(r => r.Key.ProductName == "Candy")
                          .OrderByDescending(p => p.Sum(b => b.Quantity))
                          .FirstOrDefault().Key.UserName;

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
            var result = transactions.Sum(a => a.Quantity * (products.Where(q => q.Name == a.ProductName).Select(q => q.Price).First()));

            Assert.AreEqual(3168.45, result, 0.001);
        }

        [Test]
        public void Test_WhoSpentTheMostMoney()
        {
            var result = transactions
               .Select(a => new { Name = a.UserName, val = products.Where(b => b.Name == a.ProductName).First().Price * a.Quantity })
               .GroupBy(a => a.Name)
               .ToDictionary(r => r.Key, r => r.Sum(c => c.val))
               .OrderBy(a => a.Value)
               .Last().Key;

            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {
            var x = transactions
              .Select(a => new { Name = a.UserName, val = products.Where(b => b.Name == a.ProductName).First().Price * a.Quantity })
              .GroupBy(a => a.Name)
              .ToDictionary(r => r.Key, r => r.Sum(c => c.val))
              .OrderBy(a => a.Value)
              .Last().Key;

            var result = users.Where(a => a.Name == x).Select(a => a.Password).First();

            Assert.AreEqual("optx", result);
        }

        private static T DeepCopy<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
