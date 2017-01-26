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
            var result = transactions.Sum(t => t.Quantity);

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            var result = transactions
                            .Where(t => t.Date.Year == 2016)
                            .Sum(t => t.Quantity);

            Assert.AreEqual(1160, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
            DateTime fromDate = DateTime.Now.AddDays(-7);

            var result = transactions
                            .Where(t => t.Date >= fromDate)
                            .Sum(t => t.Quantity);

            Assert.AreEqual(32, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            var result = transactions
                            .Where(t => t.Quantity > 1)
                            .Count();

            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
            var result = transactions
                            .Where(t => t.Date.DayOfWeek == DayOfWeek.Sunday)
                            .Count();

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
            var result = transactions
                            .Where(t => t.ProductName == "Chips")
                            .Sum(t => t.Quantity);

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
            var result = transactions
                            .Where(t => t.ProductName == "Chips")
                            .Where(t => t.UserName == "Jason")
                            .Sum(t => t.Quantity);

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
            var result = transactions
                            .Where(t => t.ProductName == "Chips")
                            .Where(t => t.UserName == "Jason")
                            .Where(t => t.Date.Year == 2015)
                            .Sum(t => t.Quantity);

            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
            const int MAY = 5;

            var result = transactions
                            .Where(t => t.ProductName == "Chips")
                            .Where(t => t.UserName == "Jason")
                            .Where(t => t.Date.Year == 2016 && t.Date.Month == MAY)
                            .Sum(t => t.Quantity);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            List<ItemByCount> productByCount = transactions
                            .Where(t => t.Date.Hour >= 12 && t.Date.Hour <= 13)
                            .Select(t => new ItemByCount(t.ProductName, transactions.Where(p => p.ProductName == t.ProductName).Sum(i => i.Quantity)))
                            .Distinct()
                            .ToList();

            var result = productByCount
                            .Where(t => t.count == productByCount.Max(pc => pc.count))
                            .Select(t => t.item)
                            .First();

            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {
            List<ItemByCount> productByCount = transactions
                            .Select(t => new ItemByCount(t.ProductName, transactions.Where(p => p.ProductName == t.ProductName).Sum(i => i.Quantity)))
                            .Distinct()
                            .ToList();

            var result = productByCount
                            .Where(t => t.count == productByCount.Min(pc => pc.count))
                            .Select(t => t.item)
                            .First();

            Assert.AreEqual("Cookies", result);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            List<Transaction> candyTransactions = transactions.Where(t => t.ProductName == "Candy").ToList();

            var userByQuantities = candyTransactions.GroupBy(t => t.UserName, t => t.Quantity);

            List<ItemByCount> userByTotalQuantity = userByQuantities.Select(t => new ItemByCount(t.Key, t.Sum())).ToList();

            var result = userByTotalQuantity
                                .Where(t => t.count == userByTotalQuantity.Max(q => q.count))
                                .Select(t => t.item)
                                .First();

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
            Dictionary<string, double> productPrices = products.ToDictionary(p => p.Name, p => p.Price);

            // double result = transactions.Aggregate(0.0, (total, next) => total + next.Quantity * products.Where(p => p.Name == next.ProductName).Select(p => p.Price).First());
            double result = transactions.Aggregate(0.0, (total, next) => total + next.Quantity * productPrices[next.ProductName]);

            Assert.AreEqual(3168.45, result, 0.001);
        }

        private String getUserWhoSpentTheMostMoney()
        {
            Dictionary<string, double> productPrices = products.ToDictionary(p => p.Name, p => p.Price);
            var usersByPrices = transactions.GroupBy(t => t.UserName, t => t.Quantity * productPrices[t.ProductName]);

            Dictionary<string, double> usersByTotalPrice = usersByPrices.ToDictionary(t => t.Key, t => t.Sum());

            List<KeyValuePair<string, double>> sorted = (from kv in usersByTotalPrice orderby kv.Value descending select kv).ToList();

            return sorted[0].Key;
        }

        [Test]
        public void Test_WhoSpentTheMostMoney()
        {
            String result = getUserWhoSpentTheMostMoney();

            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {
            String user = getUserWhoSpentTheMostMoney();

            String result = users
                            .Where(u => u.Name == user)
                            .Select(u => u.Password)
                            .First();

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
