﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StandardExtensions.UnitTests
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        public void SystemCollectionExtensions__RandomOrder__when__alphabet__then__returns_different()
        {
            var alpha = "abcdefghijklmnopqrstuvwxyz";

            var actual = alpha.RandomOrder();

            Assert.AreNotEqual(alpha, actual);
        }

        [TestMethod]
        public void SystemCollectionExtensions__RandomOrder__when__alphabet_with_supplied_rng__then__returns_different()
        {
            var alpha = "abcdefghijklmnopqrstuvwxyz";

            var actual = alpha.RandomOrder(new Random(123));

            Assert.AreNotEqual(alpha, actual);

            var actual2 = alpha.RandomOrder(new Random(123));

            Assert.AreEqual(actual, actual2);
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Evens__when__empty__then__returns_no_names()
        {
            List<string> names = new List<string>()
            {
                // Empty.
            };

            var evens = names.Evens();

            Assert.AreEqual(0, evens.Count());
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Evens__when__1_items__then__returns_1_name()
        {
            List<string> names = new List<string>()
            {
                "Mike", // 0
	        };

            var evens = names.Evens();

            Assert.AreEqual(1, evens.Count());
            Assert.AreEqual("Mike", String.Join(" ", evens));
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Evens__when__6_items__then__returns_3_names()
        {
            List<string> names = new List<string>()
            {
                "Mike", // 0
		        "Dave", // 1
		        "Jenny",// 2
		        "Sandy",// 3
		        "Jess", // 4
		        "Dingo" // 5
	        };

            var evens = names.Evens();

            Assert.AreEqual("Mike Jenny Jess", String.Join(" ", evens));
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Duplicates__when__Luke_Luke__then__returns__Luke()
        {
            var lukes = new[] { "Luke", "Luke" };

            var luke = lukes.Duplicates().Single();

            Assert.AreEqual("Luke", luke);
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Duplicates__when__Luke_Dave_Luke__then__returns__Luke()
        {
            var lukes = new[] { "Luke", "Dave", "Luke" };

            var luke = lukes.Duplicates().Single();

            Assert.AreEqual("Luke", luke);
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Duplicates__when__Luke_Luke_Luke__then__returns__Luke_Luke()
        {
            var lukes = new[] { "Luke", "Luke", "Luke" };

            var dupes = lukes.Duplicates();

            Assert.AreEqual(2, dupes.Count());
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Duplicates__when__Luke_Dave_Luke_Dave_Luke__then__returns__Dave_Luke_Luke()
        {
            var lukes = new[] { "Luke", "Dave", "Luke", "Dave", "Luke" }; // Should order as Dave, Dave, Luke, Luke, Luke

            var dupes = lukes.Duplicates();

            Assert.AreEqual(3, dupes.Count());
            Assert.AreEqual(dupes.ElementAt(0), "Dave");
            Assert.AreEqual(dupes.ElementAt(1), "Luke");
            Assert.AreEqual(dupes.ElementAt(2), "Luke");
        }

        [TestMethod]
        public void SystemCollectionsExtensions__Duplicates__when__using_key_selector__then__it_still_works()
        {
            var dates = new[] { DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue.AddDays(10) }; // First and last are year 0001

            var dupe = dates.Duplicates(i => i.Year).Single();

            Assert.AreEqual(dupe, dates[2]);
        }

        [TestMethod]
        public void CollectionExtensions_OrderByAppearanceIn__()
        {
            string[] names = new string[] { "Mathew", "Mark", "Luke", "John" };

            List<Tuple<string, int>> things = new List<Tuple<string, int>>();
            things.Add(new Tuple<string, int>("Luke", 3));
            things.Add(new Tuple<string, int>("Mathew", 1));
            things.Add(new Tuple<string, int>("Mark", 2));
            things.Add(new Tuple<string, int>("John", 4));

            var o = things.OrderByAppearanceIn(names, t => t.Item1).ToArray();

            Assert.AreEqual(1, o[0].Item2);
            Assert.AreEqual(2, o[1].Item2);
            Assert.AreEqual(3, o[2].Item2);
        }

        [TestMethod]
        public void CollectionExtensions_TryFirst__when__empty_dictionary__then__returns_false()
        {
            var dic = new Dictionary<string, string>();

            Assert.IsFalse(dic.TryFirst(out var f));
        }

        [TestMethod]
        public void CollectionExtensions_TryFirst__when__nonempty_dictionary__then__returns_true()
        {
            var dic = new Dictionary<string, string>()
            {
                ["a"] = "b"
            };

            Assert.IsTrue(dic.TryFirst(out var f));
        }

        [TestMethod]
        public void SystemCollectionsExtensions__DeepVisitEach__when__nested_collections__then__visits_all_items()
        {
            // Arrange
            var nestedList = new List<object>
            {
                "Level 1 - Item 1",
                new List<object>
                {
                    "Level 2 - Item 1",
                    "Level 2 - Item 2",
                    new List<object>
                    {
                        "Level 3 - Item 1"
                    }
                },
                "Level 1 - Item 2"
            };

            var visitedItems = new List<string>();

            // Act
            nestedList.DeepVisitEach(item =>
            {
                if (item is string str)
                {
                    visitedItems.Add(str);
                }
            });

            // Assert
            Assert.AreEqual(5, visitedItems.Count);
            CollectionAssert.Contains(visitedItems, "Level 1 - Item 1");
            CollectionAssert.Contains(visitedItems, "Level 1 - Item 2");
            CollectionAssert.Contains(visitedItems, "Level 2 - Item 1");
            CollectionAssert.Contains(visitedItems, "Level 2 - Item 2");
            CollectionAssert.Contains(visitedItems, "Level 3 - Item 1");
        }

        [TestMethod]
        public void SystemCollectionsExtensions__DeepVisitEach__when__max_depth_1__then__only_visits_top_level()
        {
            // Arrange
            var nestedList = new List<object>
            {
                "Level 1 - Item 1",
                new List<object>
                {
                    "Level 2 - Item 1",
                    "Level 2 - Item 2",
                    new List<object>
                    {
                        "Level 3 - Item 1"
                    }
                },
                "Level 1 - Item 2"
            };

            var visitedItems = new List<string>();

            // Act
            nestedList.DeepVisitEach(item =>
            {
                if (item is string str)
                {
                    visitedItems.Add(str);
                }
            }, maxDepth: 0);

            // Assert
            Assert.AreEqual(2, visitedItems.Count);
            CollectionAssert.Contains(visitedItems, "Level 1 - Item 1");
            CollectionAssert.Contains(visitedItems, "Level 1 - Item 2");
        }

        [TestMethod]
        public void SystemCollectionsExtensions__DeepVisitEach__when__with_depth_parameter__then__provides_correct_depth()
        {
            // Arrange
            var nestedList = new List<object>
            {
                "Level 1 - Item 1",
                new List<object>
                {
                    "Level 2 - Item 1",
                    "Level 2 - Item 2",
                    new List<object>
                    {
                        "Level 3 - Item 1"
                    }
                },
                "Level 1 - Item 2"
            };

            var visitedItems = new Dictionary<string, int>();

            // Act
            nestedList.DeepVisitEach((item, depth) =>
            {
                if (item is string str)
                {
                    visitedItems[str] = depth;
                }
            });

            // Assert
            Assert.AreEqual(5, visitedItems.Count);
            Assert.AreEqual(0, visitedItems["Level 1 - Item 1"]);
            Assert.AreEqual(0, visitedItems["Level 1 - Item 2"]);
            Assert.AreEqual(1, visitedItems["Level 2 - Item 1"]);
            Assert.AreEqual(1, visitedItems["Level 2 - Item 2"]);
            Assert.AreEqual(2, visitedItems["Level 3 - Item 1"]);
        }

        [TestMethod]
        public void SystemCollectionsExtensions__DeepVisitEach__when__with_dictionary__then__provides_correct_depth()
        {
            // Arrange
            var nestedDictionary = new Dictionary<string, object>
            {
                ["Key1"] = "Value1",
                ["Key2"] = new Dictionary<string, object>
                {
                    ["NestedKey1"] = "NestedValue1",
                    ["NestedKey2"] = new Dictionary<string, object>
                    {
                        ["DeepKey"] = "DeepValue"
                    }
                },
                ["Key3"] = "Value3"
            };

            var visitedItems = new Dictionary<string, int>();

            // Act
            nestedDictionary.DeepVisitEach((item, depth) =>
            {
                if (item.Value is string str)
                {
                    visitedItems[str] = depth;
                }
            });

            // Assert
            Assert.AreEqual(4, visitedItems.Count);
            Assert.AreEqual(0, visitedItems["Value1"]);
            Assert.AreEqual(0, visitedItems["Value3"]);
            Assert.AreEqual(1, visitedItems["NestedValue1"]);
            Assert.AreEqual(2, visitedItems["DeepValue"]);
        }
    }
}
