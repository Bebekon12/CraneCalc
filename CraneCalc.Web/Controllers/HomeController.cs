using CraneCalc.Application.Interfaces;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Models;
using CraneCalc.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.Web.Controllers;

public class HomeController(
        ICartRepository cartRepository, 
        ICargoRepository cargoRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? searchCargo, CancellationToken ct)
    {
        var cargos = await cargoRepository.GetCargosSearchAsync(searchCargo??"", ct);
        
        var cart = await cartRepository.GetCartByUserIdAsync(1, ct);
        
        var model = new HomeServicesModel
        {
            CargoItems = cargos,
            CartId = cart?.Id,
            Quantity = cart == null ? 0 : cart.CartCargo.Count
        };
        
        ViewBag.CurrentTitle = searchCargo;
        return View(model);
    }
    
    [HttpGet("cargo-detail/{id:guid}")]
    public async Task<IActionResult> CargoDetail(Guid id, CancellationToken ct)
    {
        var cargo = await cargoRepository.GetCargoByIdAsync(id, ct);
        
        return View(cargo);
    }
    
    [HttpGet("cart/{id:guid}")]
    public async Task<IActionResult> Cart(Guid id, CancellationToken ct)
    {
        var cart = await cartRepository.GetCartByIdAsync(id, ct);
        
        if (cart == null)
            return NotFound();

        var cargos = cart.CartCargo
            .Where(c=>!c.IsDeleted)
            .Select(cc => cc.Cargo).ToList();
    
        return View(cargos);
    }
    
    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart(Guid cargoId, CancellationToken ct)
    {
        var cart = await cartRepository.GetCartByUserIdAsync(1, ct) ?? await cartRepository
            .CreateCartAsync(new Cart
            {
                Id = Guid.NewGuid(),
                Status = Status.Draft,
                CreatedDate = DateTime.UtcNow,
                CreatorId = 1,
                CartCargo = []
            }, ct);
    
        await cargoRepository.PutCargoInCartAsync(cargoId, cart.Id, ct);
        
        var newCart = await cartRepository.GetCartByIdAsync(cart.Id, ct);
        
        if(newCart == null)
            return NotFound();

        var quantity = newCart.CartCargo.Count;
    
        return Json(new { 
            success = true, 
            quantity,
            cartUrl = Url.Action("Cart", "Home", new { newCart.Id })
        });
    }

    [HttpPost("remove-cargo-from-cart/{cargoId:guid}")]
    public async Task<IActionResult> RemoveFromCart(Guid cargoId, CancellationToken ct)
    {
        var cart = await cartRepository.GetCartByUserIdAsync(1, ct);
    
        if(cart == null)
            return NotFound();

        await cartRepository.RemoveCargoInCartAsync(cart.Id, cargoId, ct);
    
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return Ok();
        }
    
        return RedirectToAction("Cart");
    }

    [HttpPost("remove-cart")]
    public async Task<IActionResult> RemoveCart(CancellationToken ct)
    {
        var cart = await cartRepository.GetCartByUserIdAsync(1, ct);

        if (cart == null)
            return NotFound();
        
        await cartRepository.RemoveCartAsync(cart.Id, ct);
        
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return Ok(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "Home") 
            });
        }
        
        return RedirectToAction("Index");
    }
}