using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InternetCafeManagementSystem.DataStructures;

namespace InternetCafeManagementSystem.Tests
{
    [TestClass]
    public class CustomHashTableTests
    {
        // ✅ UT01
        [TestMethod]
        public void Add_ValidItem_ItemCanBeRetrieved()
        {
            // Arrange
            var table = new CustomHashTable<string, string>(10);

            // Act
            table.Add("C001", "Hasan");
            var result = table.Get("C001");

            // Assert
            Assert.AreEqual("Hasan", result);
        }

        // ✅ UT02: Retrieve correct value
        [TestMethod]
        public void Retrieve_Item_ReturnsCorrectValue()
        {
            // Arrange
            var table = new CustomHashTable<string, string>(10);
            table.Add("C002", "Ali");

            // Act
            var result = table.Get("C002");

            // Assert
            Assert.AreEqual("Ali", result);
        }

        // ✅ UT03: Duplicate key should throw exception
        [TestMethod]
        public void Add_DuplicateKey_ThrowsException()
        {
            // Arrange
            var table = new CustomHashTable<string, string>(10);
            table.Add("C003", "Sara");

            // Act & Assert
            try
            {
                table.Add("C003", "Duplicate");
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}