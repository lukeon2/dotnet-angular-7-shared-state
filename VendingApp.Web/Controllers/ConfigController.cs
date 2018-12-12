using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VendingApp.Infrastructure.Data;
using VendingApp.Infrastructure.Models;
using VendingApp.Infrastructure.Services;

namespace VendingApp.Web.Controllers
{
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        private readonly IConfigService _configService;
        private readonly VendingDbContext _context;

        public ConfigController(VendingDbContext context, IConfigService configService)
        {
            _context = context;
            _configService = configService;
        }


        [HttpGet("[action]")]
        public ConfigViewModel Get(string currency)
        {
            var config = _configService.GetAllData(currency);
            return config;
        }


        [HttpPost("[action]")]
        public IActionResult SetCurrency([FromBody] CurrencyModel model)
        {
            try
            {
                _configService.UpdateCurrency(model.Symbol);
                return Ok();
            }
            catch (ValidationException ex)
            {
                // TODO: move to middleware
                ModelState.AddModelError("Validation", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("[action]")]
        public IActionResult UpdateCoins([FromBody] CoinModel model)
        {
            try
            {
                _configService.UpdateCoins(model.Amount);
                return Ok();
            }
            catch (ValidationException ex)
            {
                // TODO: move to middleware
                ModelState.AddModelError("Validation", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("[action]")]
        public IActionResult CancelCoins()
        {
            try
            {
                var change = _configService.CancelCoins();
                return Ok(change);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("Validation", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }

    public class CurrencyModel
    {
        public string Symbol { get; set; }
    }

    public class CoinModel
    {
        public decimal Amount { get; set; }
    }
}
