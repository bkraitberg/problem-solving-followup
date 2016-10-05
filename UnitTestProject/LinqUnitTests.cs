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
            var result = transactions.Sum(a => a.Quantity); // TODO

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            //var result = transactions.Where(a => a.Date >= Convert.ToDateTime("2016-01-01"))
            //    .Sum(a => a.Quantity); // TODO
            var result = transactions.Where(a => a.Date.Year.Equals(2016))
                .Sum(a => a.Quantity); // TODO

            Assert.AreEqual(1160, result);
        }
        
        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
        var result = transactions.Where(a => a.Date >= DateTime.Today.AddDays(-7))
            .Sum(b => b.Quantity); // TODO

            //Assert.AreEqual(32, result);
            Assert.AreEqual(26, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            var result = transactions
                .Where(g => g.Quantity > 1)
                .Count(); // TODO

            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
        var result = transactions.Where(a => a.Date.DayOfWeek.Equals(DayOfWeek.Sunday))
        .Count(); // TODO

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
            var result = transactions.Where(x => x.ProductName.Equals("Chips"))
                .Sum(z => z.Quantity); // TODO

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
        var result = transactions.Where(x => x.ProductName.Equals("Chips"))
            .Where(x => x.UserName.Equals("Jason"))
            .Sum(z => z.Quantity); // TODO

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
        var result = transactions.Where(x => x.ProductName.Equals("Chips"))
        .Where(x => x.UserName.Equals("Jason"))
        .Where(a => a.Date.Year.Equals(2015))
        .Sum(z => z.Quantity); // TODO

            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
        var result = transactions.Where(x => x.ProductName.Equals("Chips"))
                                 .Where(x => x.UserName.Equals("Jason"))
                                 .Where(a => a.Date.Year.Equals(2016))
                                 .Where(a => a.Date.Month.Equals(5))
                                 .Sum(z => z.Quantity); // TODO

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            var result = transactions
                                     .Where(a => a.Date.Hour >= 12)
                                     .Where(a => a.Date.Hour < 13)
                                     .OrderByDescending(b => b.Quantity)
                                     .GroupBy(g => g.ProductName)
                                        //.Select(c => c.Key).ToString()
                                     .First().Key
                                     //.First(x => x.)
                                     //.Max(x => x.)
                                     //.Max()
                                     //.Select(a => a.Key)
                                     //.Select(b => b.Max(a => a.Quantity))
                                     ; // TODO

            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {
        var result = transactions
            //.OrderBy(b => b.Quantity)
            //.OrderByDescending(b => b.Quantity)
                                 .GroupBy(g => g.ProductName)
                                 .Select(h => new { ProductName = h.Key, Quantity = h.Sum(i => i.Quantity) })
                                 .OrderBy(j => j.Quantity)
                                 //.Select(k => k.ProductName.First())
                                 //.OrderBy(g => g.Key)
                                 .First().ProductName
                                 //.Min(a => a.ProductName)
                                 //.Select(a => a.ProductName.First())
            //.Last().Key
                                  ; // TODO

            Assert.AreEqual("Cookies", result);
            //Assert.AreEqual("Cookies", result2);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            //var result = ""; // TODO
            var result = transactions // TODO
                                     //.Where(a => a.ProductName = "Candy")
                                     .Where(a => a.ProductName.Equals("Candy"))
                                     .GroupBy(g => g.UserName)
                                     .Select(h => new { UserName = h.Key, Quantity = h.Sum(i => i.Quantity) })
                                     .OrderByDescending(j => j.Quantity)
                //.Select(c => c.Key).ToString()
                                     .First().UserName
                                     ;

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
        var temp = from t in transactions
                         join p in products on t.ProductName equals p.Name
                         select new {Price = p.Price, Qty = t.Quantity}
                          
                             ; // TODO
            var result = temp
                .Sum(a => a.Price * a.Qty)
                ;

            Assert.AreEqual(3168.45, result, 0.001);
        }

        [Test]
        public void Test_WhoSpentTheMostMoney()
        {
            var temp = from t in transactions
                       join p in products on t.ProductName equals p.Name
                       select new { Price = p.Price, Qty = t.Quantity, User = t.UserName }

                                 ; // TODO
            var result = temp
                   .GroupBy(g => g.User)
                   .Select(h => new { UserName = h.Key, Spent = h.Sum(i => i.Price * i.Qty) })
                   .OrderByDescending(j => j.Spent)
                   .First().UserName
                   ;


            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {

                var temp = from t in transactions
                           join p in products on t.ProductName equals p.Name
                           select new { Price = p.Price, Qty = t.Quantity, User = t.UserName }

                                     ; // TODO
                var temp2 = temp
                       .GroupBy(g => g.User)
                       .Select(h => new { UserName = h.Key, Spent = h.Sum(i => i.Price * i.Qty) })
                       .OrderByDescending(j => j.Spent)
                       .First().UserName
                       ;

                var result = users.Where(a => a.Name.Equals(temp2))
                    .Min(b => b.Password)
                    ;


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
