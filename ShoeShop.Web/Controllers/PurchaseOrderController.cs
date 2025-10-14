using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeShop.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _poService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryService _inventoryService;

        public PurchaseOrderController(
            IPurchaseOrderService poService,
            ISupplierService supplierService,
            IInventoryService inventoryService)
        {
            _poService = poService;
            _supplierService = supplierService;
            _inventoryService = inventoryService;
        }

        private async Task LoadDropdowns()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            var shoeVariations = await _inventoryService.GetAllShoeVariationsAsync();

            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name");
            ViewBag.ShoeVariations = new SelectList(shoeVariations, "Id", "Name");
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _poService.GetAllAsync();
            return View(orders);
        }
        public async Task<IActionResult> Details(int id)
        {
            var po = await _poService.GetByIdAsync(id);

            if (po == null)
            {
                return NotFound();
            }
            return View(po);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            var model = new CreatePurchaseOrderDto
            {
                OrderDate = DateTime.Today,
                Items = new List<PurchaseOrderItemDto> { new PurchaseOrderItemDto() }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _poService.CreatePurchaseOrderAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An unexpected error occurred while saving the purchase order.");
                }
            }

            await LoadDropdowns();
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var po = await _poService.GetByIdAsync(id);

            if (po == null)
            {
                return NotFound();
            }

            await LoadDropdowns();


            return View(po);
        }



[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, PurchaseOrderDto dto)
{
    // ...
    if (ModelState.IsValid)
    {
   
    }

    var errors = ModelState.Values.SelectMany(v => v.Errors);
    foreach (var error in errors)
    {
        System.Diagnostics.Debug.WriteLine(error.ErrorMessage); 
    }
    
    await LoadDropdowns();
    return View(dto); 
}

        
        public async Task<IActionResult> Delete(int id)
        {
            var po = await _poService.GetByIdAsync(id);
            if (po == null)
            {
                return NotFound();
            }

            return View(po);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                
                await _poService.DeletePurchaseOrderAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                ViewData["ErrorMessage"] = $"Error deleting purchase order: {ex.Message}";
                var po = await _poService.GetByIdAsync(id);
                return View(po); 
            }
        }


        public async Task<IActionResult> GetNewItemRow(int index)
        {
            var shoeVariations = await _inventoryService.GetAllShoeVariationsAsync();
            ViewBag.ShoeVariations = new SelectList(shoeVariations, "Id", "Name");

            ViewData["Index"] = index;
            return PartialView("_PurchaseOrderItemRow", new PurchaseOrderItemDto());
        }


        public async Task<IActionResult> Receive(int id)
        {
            try
            {
                
                await _poService.ReceivePurchaseOrderAsync(id);
                TempData["SuccessMessage"] = $"Purchase Order #{id} successfully received and inventory updated.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Purchase order not found.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred during receiving.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}