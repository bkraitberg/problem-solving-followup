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
            var result = transactions.Sum(a=>a.Quantity); // TODO

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            var result = transactions.Where(a=>a.Date.Year==2016).Sum(a=>a.Quantity); // TODO

            Assert.AreEqual(1160, result);
        }
        
        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
            /*
             * ToDo: ask about the issue regarding referenceing last 7 days
             */
            var result = transactions
                .Where(a => a.Date >= DateTime.Today.AddDays(-7))
                .Select(a => a.Quantity)
                .Sum(); // TODO
            

            Assert.AreEqual(32, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            //var result = transactions.GroupBy(a => a.Quantity).Where(g => g.Count()>1).Select(g=>g.Count()).Count(); // TODO
            var result = transactions
               .Where(a => a.Quantity > 1)
               .Count();
        
            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
            //var result = ""; // TODO
            var result = transactions.Where(a => a.Date.DayOfWeek == DayOfWeek.Sunday).Count();

            Assert.AreEqual(267, result);
        }

        [Test]
        public void Test_WhatIsTheAverageQuantityPurchased()
        {
            var result = transactions.Select(a=>a.Quantity).Average(); // TODO

            Assert.AreEqual(1.5005, result, 0.0001);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHaveBeenBought()
        {
            var result = transactions.Where(a=>a.ProductName=="Chips").Select(a=>a.Quantity).Sum(a=>(long)a); // TODO

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
            var result = transactions.Where(a => (a.ProductName == "Chips" && a.UserName == "Jason")).Select(a => a.Quantity).Sum(a => (long)a);

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
            var result = transactions.
                Where(a => (a.ProductName == "Chips" && a.UserName == "Jason" && a.Date.Year == 2015)).
                Select(a => a.Quantity).Sum(a => (long)a);

            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
            var result = transactions.
                Where(a => (a.ProductName == "Chips" && a.UserName == "Jason" && a.Date.Year == 2016 && a.Date.Month==5)).
                Select(a => a.Quantity).Sum(a => (long)a);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            var result = transactions.Where(x => x.Date.Hour >= 12)
                .Where(x => x.Date.Hour < 13)
                .OrderByDescending(x => x.Quantity)
                .GroupBy(g => g.ProductName)
                .First().Key;

            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {
            var result = transactions
                .GroupBy(a => a.ProductName)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity))
                .OrderBy(x => x.Value)
                .First().Key;
               
            Assert.AreEqual("Cookies", result);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            var result = transactions
                .Where(x => x.ProductName == "Candy")
                .GroupBy(x => x.UserName)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity))
                .OrderBy(x => x.Value)
                .Last().Key;

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
            var result = transactions
               .GroupBy(x => x.ProductName)
               .ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity))
               .Sum(g => (products.Where(x=>x.Name==g.Key).First().Price*(double) g.Value));

            Assert.AreEqual(3168.45, result, 0.001);
        }

        [Test]
        public void Test_WhoSpentTheMostMoney()
        {
            var result = transactions
              .Select(x => new { Username = x.UserName, spent = products.Where(p => p.Name == x.ProductName).First().Price * x.Quantity })
              .GroupBy(x => x.Username)
              .ToDictionary(g => g.Key, g => g.Sum(t => t.spent))
              .OrderBy(x=>x.Value)
              .Last().Key;
  
            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {
            var user_name = transactions
              .Select(x => new { Username = x.UserName, spent = products.Where(p => p.Name == x.ProductName).First().Price * x.Quantity })
              .GroupBy(x => x.Username)
              .ToDictionary(g => g.Key, g => g.Sum(t => t.spent))
              .OrderBy(x => x.Value)
              .Last().Key;
            /*
             * This two query can be merged
             */
            var result = users.Where(x=>x.Name==user_name).Select(x => x.Password).First();
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
