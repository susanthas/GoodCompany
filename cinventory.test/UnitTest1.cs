using cinventory.web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;
using static cinventory.web.Utils;

namespace cinventory.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Inventory_PostOne_Id_Existing_Return_1(int value)
        {
            // Arrange
            InventoryController ic = new InventoryController(null);
            ic.InitializeTestContext();

            // Act
            JsonResponse response = ic.PostOne(value);

            //Asert
            Assert.AreEqual(1, response.code);
        }

        [TestMethod]
        public void Inventory_PostOne_Id_NonExisting_Return_0()
        {
            // Arrange
            InventoryController ic = new InventoryController(null);
            ic.InitializeTestContext();
            
            // Act
            JsonResponse response = ic.PostOne(110);
            
            //Assert
            Assert.AreEqual(0, response.code);
        }

        [TestMethod]
        public void Inventory_PostOne_Id_0_Return_1()
        {
            // Arrange
            InventoryController ic = new InventoryController(null);
            ic.InitializeTestContext();

            // Act
            JsonResponse response = ic.PostOne(0);

            // Assert
            Assert.AreEqual(1, response.code);
        }

    }
}
