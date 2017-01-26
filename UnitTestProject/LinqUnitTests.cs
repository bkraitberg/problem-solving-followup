using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using problem_solving_followup;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using Assert = NUnit.Framework.Assert;

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
            var result = transactions.Sum(t => t.Quantity); 

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            var result = transactions.Where(t => t.Date.Year == 2016)
                .Sum(t => t.Quantity);            

            Assert.AreEqual(1160, result);
        }
        
        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
            var result = transactions.Where(t => t.Date >= DateTime.Now.AddDays(-9))
                .Sum(t => t.Quantity);

            Assert.AreEqual(32, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            var result = transactions.Count(t => t.Quantity > 1);

            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
            var result = transactions.Count(t => t.Date.DayOfWeek == DayOfWeek.Sunday);

            Assert.AreEqual(267, result);
        }

        [Test]
        public void Test_WhatIsTheAverageQuantityPurchased()
        {
            var result = transactions.Average(t => t.Quantity);

            Assert.AreEqual(1.5005, result, 0.0001);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHaveBeenBought()
        {
            var result = transactions.Where(t => t.ProductName.Equals("Chips"))
                .Sum(t => t.Quantity);

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
            var result = transactions.Where(t => t.ProductName.Equals("Chips"))
                .Where(t => t.UserName.Equals("Jason"))
                .Sum(t => t.Quantity);

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
            var result = transactions.Where(t => t.ProductName.Equals("Chips"))
                .Where(t => t.UserName.Equals("Jason"))
                .Where(t => t.Date.Year == 2015)
                .Sum(t => t.Quantity);

            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
            var result = transactions.Where(t => t.ProductName.Equals("Chips"))
                .Where(t => t.UserName.Equals("Jason"))
                .Where(t => t.Date.Year == 2016 && t.Date.Month == 5)
                .Sum(t => t.Quantity);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            var result =
                transactions
                    .Where(t => t.Date.Hour >= 12 && t.Date.Hour <= 13)
                    .GroupBy(g => g.ProductName)
                    .Select(g => new {g.Key, Qty = g.Sum(t => t.Quantity)})
                    .OrderBy(g => g.Qty)
                    .Last().Key;
            
            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {
            var result = transactions
                    .GroupBy(g => g.ProductName)
                    .Select(g => new { g.Key, Qty = g.Sum(t => t.Quantity) })
                    .OrderBy(g => g.Qty)
                    .First().Key;

            Assert.AreEqual("Cookies", result);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            var result = transactions
                .Where(t => t.ProductName.Equals("Candy"))
                .GroupBy(g => g.UserName)
                .Select(g => new { g.Key, Qty = g.Sum(t => t.Quantity) })
                .OrderBy(g => g.Qty)
                .Last().Key;

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
            var result = ""; // TODO

            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {
            var result = ""; // TODO

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
