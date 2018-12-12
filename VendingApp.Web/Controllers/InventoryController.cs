using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VendingApp.Infrastructure.Data;
using VendingApp.Infrastructure.Helpers;
using VendingApp.Infrastructure.Models;
using VendingApp.Infrastructure.Services;

namespace VendingApp.Web.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly IConfigService _configService;
        private readonly InventoryService _inventoryService;
        private readonly VendingDbContext _context;

        public InventoryController(IConfigService configService, InventoryService inventoryService, VendingDbContext context)
        {
            _configService = configService;
            _inventoryService = inventoryService;
            _context = context;
        }

        [HttpPost("[action]")]
        public IActionResult Checkout([FromBody] InventoryModel model)
        {
            var inventory = _context.Inventory.FirstOrDefault(x => x.ProductNr == model.ProductNr);
            if (inventory == null)
            {
                return BadRequest();
            }

            try
            {
                _inventoryService.BuyProduct(model.ProductNr);
                return Ok();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("Validation",ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
