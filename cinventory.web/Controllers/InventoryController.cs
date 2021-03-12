using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static cinventory.web.Utils;

namespace cinventory.web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger)
        {
            _logger = logger;

        }

        public void InitializeTestContext()
        {
            Startup startup = new Startup(null);
        }

        /// <summary>
        /// Return the inventory list. To improve performance, Caching should be enabled.
        /// </summary>
        /// <param name="computerType"></param>
        /// <param name="brand"></param>
        /// <param name="loadLookups"></param>
        /// <returns></returns>
        [HttpGet("list")]
        //[ResponseCache(Duration =5, VaryByQueryKeys = new string[]{"computerType", "brand"}, Location = ResponseCacheLocation.Any)]
        public object GetList(string computerType = "", string brand = "", bool loadLookups = false)
        {
            computerType = string.Concat("", computerType);
            brand = string.Concat("", brand);
            if (loadLookups)
            {
                return new
                {
                    inventory = Startup._inventory.Where(item =>
                                                (computerType == "" || item.computerType == computerType)
                                                    && (brand == "" || item.brand == brand)).Take(100).ToList(), // fool proof by limiting the result size
                    lookupLists = GetLookupLists()
                };
            }
            else
            {
                return new
                {
                    inventory = Startup._inventory.Where(item =>
                                                (computerType == "" || item.computerType == computerType)
                                                    && (brand == "" || item.brand == brand)).Take(100).ToList() // fool proof by limiting the result size
                };
            }
        }

        /// <summary>
        /// Lookup values should be coming from the datastore
        /// </summary>
        /// <returns></returns>
        private object GetLookupLists()
        {
            return new
            {
                computerTypes = new string[] { "Desktop", "Laptop" },
                brands = new string[] { "HP", "Dell", "MSI", "Microsoft" }
            };
        }

        /// <summary>
        /// Load items to edit via a POST
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("one")]
        public JsonResponse PostOne(int id)
        {
            JsonResponse response = new JsonResponse();
            if (id == 0) 
            {
                // item not in the system, we should send back default settings
                InventoryItem item = new InventoryItem()
                {
                    computerType = "Laptop", // just to show the use of default values
                    brand = "",
                    features = Startup.laptopFeatures,
                    processor= "",
                    price=0
                };

                response = new JsonResponse(1, "", item);
            }
            else 
            {
                // item could be in the system, let's search and find
                IEnumerable< InventoryItem> items = Startup._inventory.Where(item => item.id == id);
                if (items.Count() == 0)
                {
                    response = new JsonResponse(0, "Item you requested is not in the system.");
                }
                else
                {
                    response = new JsonResponse(1, "", items.First());
                }
            }

            return response;
        }

        /// <summary>
        /// Save inventory item ( Add/ update)
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public JsonResponse PostSave(InventoryItem inventoryItem)
        {
            JsonResponse response = new JsonResponse();
            if (Validation.TryValidate(inventoryItem, ref response))
            {
                // when the id is 0, we have a new inventory item
                if (inventoryItem.id == 0)
                {
                    // item not in the system, so we add to the database
                    inventoryItem.id = Startup._inventory.Count() + 1;
                    Startup._inventory.Add(inventoryItem);

                    response = new JsonResponse(1, "Item successfully added.");
                }
                else
                {
                    // item could be in the system, let's search and find
                    IEnumerable<InventoryItem> items = Startup._inventory.Where(item => item.id == inventoryItem.id);
                    if (items.Count() == 0)
                    {
                        // item not found, so we send a message
                        response = new JsonResponse(0, "Item you updated is no in the system anymore.");
                    }
                    else
                    {
                        // item found, so we update. In relity, this will be a database call
                        int index = Startup._inventory.FindIndex(ii => ii.id == inventoryItem.id);
                        Startup._inventory[index] = inventoryItem;
                        response = new JsonResponse(0, "Item successfully updated.");
                    }
                }
            }
            return response;
        }

        public class InventoryItem
        {
            private List<InventoryItemFeature> _features = new List<InventoryItemFeature>();
            [Required]
            public int id { get; set; }
            [Required]
            public string computerType { get; set; }
            public string processor { get; set; }
            [Required]
            public string brand { get; set; }
            public int price { get; set; }
            public List<InventoryItemFeature> features { get { return _features; } set { _features = value; } }
            public int noOfUSBPorts { get; set; }
            public int noOfRAMPorts { get; set; }
        }

        private enum ComputerType
        {
            None,
            Desktop,
            Laptop
        }

        public class InventoryItemFeature
        {
            public string featureName{ get;set;}
            public string featureValue { get; set; }
        }

    }
}
