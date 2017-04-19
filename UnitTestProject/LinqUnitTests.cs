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
            var result =transactions.Sum(a =>a.Quantity); // TODO

            Assert.AreEqual(3001, result);
        }

        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedIn2016()
        {
            DateTime startDate= new DateTime(2016, 1,1);
            DateTime endDate = new DateTime(2016, 12,31);
            var result = transactions.Where(b =>b.Date >= startDate && b.Date<=endDate).Sum(a =>a.Quantity); // TODO

            Assert.AreEqual(1160, result);
        }
        
        [Test]
        public void Test_WhatIsTheTotalQuantityPurchasedInThePast7Days()
        {
            var result =transactions.Where(a => a.Date >=DateTime.Now.AddDays(-7) && a.Date<=DateTime.Now.AddDays(0)).Sum(b =>b.Quantity); // TODO
           
            Assert.AreEqual(24, result);
        }

        [Test]
        public void Test_HowManyTransactionsBoughtMoreThan1Quantity()
        {
            var result = transactions.Where(a =>a.Quantity>1).Count(); // TODO

            Assert.AreEqual(1001, result);
        }

        [Test]
        public void Test_HowManyTransactionsOccuredOnSundays()
        {
            var result =transactions.Where(a =>a.Date.DayOfWeek==DayOfWeek.Sunday).Count(); // TODO

            Assert.AreEqual(267, result);
        }

        [Test]
        public void Test_WhatIsTheAverageQuantityPurchased()
        {
            var result =transactions.Average(a =>a.Quantity); // TODO

            Assert.AreEqual(1.5005, result, 0.0001);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHaveBeenBought()
        {
            var result = transactions.Where(a => a.ProductName == "Chips").Sum(b =>b.Quantity); // TODO

            Assert.AreEqual(390, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsHasJasonBought()
        {
            var result = transactions.Where(a =>a.ProductName=="Chips" && a.UserName=="Jason").Sum(b =>b.Quantity); // TODO

            Assert.AreEqual(44, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyIn2015()
        {
            DateTime startDate = new DateTime(2015, 1, 1);
            DateTime endDate = new DateTime(2015, 12, 31);
            var result = transactions.Where(a => a.ProductName == "Chips" && a.UserName == "Jason"&& a.Date >= startDate && a.Date <= endDate).Sum(p =>p.Quantity); // TODO
           
            Assert.AreEqual(33, result);
        }

        [Test]
        public void Test_HowManyBagsOfChipsDidJasonBuyInMay2016()
        {
            DateTime startDate = new DateTime(2016, 5, 1);
            DateTime endDate = new DateTime(2016, 5, 31);
            var result = transactions.Where(a => a.ProductName == "Chips" && a.UserName == "Jason" && a.Date >= startDate && a.Date <= endDate).Sum(p =>p.Quantity);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_WhatProductSellsTheMostBetween12And1PM()
        {
            var list = from trans in transactions
                         where trans.Date.TimeOfDay >=TimeSpan.Parse("12:00") && trans.Date.TimeOfDay <=TimeSpan.Parse("13:00")
                         group trans by trans.ProductName into transGroup
                         select new { prodName = transGroup.Key, most = transGroup.Max(p => p.Quantity) };
            var result = list.First().prodName;
            Assert.AreEqual("Candy", result);
        }

        [Test]
        public void Test_WhatProductSellsTheLeast()
        {
            var list = from trans in transactions
                       group trans by trans.ProductName into transGroup
                       select new { prodName = transGroup.Key, most = transGroup.Sum(p => p.Quantity) };
            
            var result = list.OrderBy(a => a.most).Select(b => b.prodName).First();
            
            Assert.AreEqual("Cookies", result);
        }

        [Test]
        public void Test_WhoBoughtTheMostCandy()
        {
            var list = from trans in transactions
                       where trans.ProductName=="Candy"
                       group trans by trans.UserName into transGroup
                       select new { userName = transGroup.Key, most = transGroup.Sum(p => p.Quantity) };
            var result = list.Select(p =>p.userName).First(); // TODO

            Assert.AreEqual("David", result);
        }

        [Test]
        public void Test_WhatIsTheTotalDollarValueOfAllTransactions()
        {
            var list = from a in products
                         join r in transactions on a.Name equals r.ProductName
                         select new { a.Price, r.Quantity };// TODO
            
            var result = list.Sum(a => a.Price * a.Quantity);
            Assert.AreEqual(3168.45, result, 0.001);
        }

        [Test]
        public void Test_WhoSpentTheMostMoney()
        {
            var list = from a in products
                       join r in transactions on a.Name equals r.ProductName
                       select new { a.Price, r.Quantity,r.UserName };

            var moneyList = from b in list
                         group b by b.UserName into nameGroup
                         let maxMoney = nameGroup.Sum(p => p.Price * p.Quantity)
                         select new { maxMoney, nameGroup.Key };
            var result = moneyList.OrderByDescending(p=>p.maxMoney).Select(b =>b.Key).First();
            Assert.AreEqual("Rod", result);
        }

        [Test]
        public void Test_WhatIsThePasswordOfThePersonWhoSpentTheMostMoney()
        {
            var list = from a in products
                       join r in transactions on a.Name equals r.ProductName
                       select new { a.Price, r.Quantity, r.UserName };

            var moneyList = from b in list
                            group b by b.UserName into nameGroup
                            let maxMoney = nameGroup.Sum(p => p.Price * p.Quantity)
                            select new { maxMoney, nameGroup.Key };
            var mostMoneyPerson = moneyList.OrderByDescending(p => p.maxMoney).Select(b => b.Key).First();
            
            var result = users.Where(p=>p.Name==mostMoneyPerson).Select(a =>a.Password).First(); 

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
