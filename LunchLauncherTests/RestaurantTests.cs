using System.Collections.Generic;
using LunchLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class RestaurantTests
    {
        private IList<Restaurant> GetSeededRestaurantList()
        {
            return new List<Restaurant> {
                new Restaurant { Name = "Piza Hut" },
                new Restaurant { Name = "Jimmy Johns" },
                new Restaurant { Name = "Taco Bell" },
                new Restaurant { Name = "DQ" },
                new Restaurant { Name = "Qdoba" },
                new Restaurant { Name = "Asian Grill" },
                new Restaurant { Name = "Sushiko" }
            };
        }
    }
}