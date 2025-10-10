using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ShoeShop.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IWebHostEnvironment _env;

        public InventoryController(IInventoryService inventoryService, IWebHostEnvironment env)
        {
            _inventoryService = inventoryService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var shoes = await _inventoryService.GetAllShoesAsync();
                var reports = await _inventoryService.GetInventoryReportAsync();

                var viewModel = new InventoryViewModel
                {
                    Shoes = shoes,
                    InventoryReports = reports
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading inventory: {ex.Message}";
                return View(new InventoryViewModel());
            }
        }

        public IActionResult Create()
        {
            var model = new CreateShoeDto
            {
                IsActive = true,
                StockQuantity = 0,
                size = 42,
                ColorName = "Default", 
                AvailabilityStatus = "Available",
                ImageUrl = "" 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShoeDto dto, IFormFile ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    ModelState.Remove("ImageUrl");
                }
                else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                {
                    ModelState.Remove("ImageUrl");
                }

                ModelState.Remove("Description");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        })
                        .ToList();
                    var errorMessages = new List<string>();
                    foreach (var error in errors)
                    {
                        foreach (var msg in error.Errors)
                        {
                            errorMessages.Add($"{error.Field}: {msg}");
                            Console.WriteLine($"Validation Error - {error.Field}: {msg}");
                        }
                    }

                    TempData["Error"] = "Validation failed: " + string.Join(", ", errorMessages);
                    return View(dto);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        dto.ImageUrl = await SaveImageFileAsync(ImageFile);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error uploading image: {ex.Message}";
                        return View(dto);
                    }
                }
                else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                {
                    dto.ImageUrl = "https://placehold.co/400x400?text=No+Image";
                }
                if (string.IsNullOrWhiteSpace(dto.ColorName))
                {
                    dto.ColorName = "Default";
                }

                if (dto.size <= 0)
                {
                    dto.size = 42;
                }

                var createdShoe = await _inventoryService.CreateShoeAsync(dto);

                if (createdShoe == null || createdShoe.Id <= 0)
                {
                    TempData["Error"] = "Failed to create shoe. Service returned null or invalid ID.";
                    return View(dto);
                }

                if (dto.StockQuantity > 0)
                {
                    try
                    {
                        await _inventoryService.UpdateShoeStockAsync(createdShoe.Id, dto.StockQuantity);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Shoe created but failed to update stock: {ex.Message}";
                        return RedirectToAction(nameof(Edit), new { id = createdShoe.Id });
                    }
                }

                TempData["Success"] = $"Shoe '{createdShoe.Name}' created successfully with {dto.StockQuantity} stock!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating shoe: {ex.Message}";
              
                Console.WriteLine($"Create Error: {ex}");
                return View(dto);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["Error"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }
                var reports = await _inventoryService.GetInventoryReportAsync();
                var totalStock = reports
                    .Where(r => r.ShoeColorVariationId == id)
                    .Sum(r => r.StockQuantity);

                var dto = new CreateShoeDto
                {
                    Id = shoe.Id,
                    Name = shoe.Name,
                    Brand = shoe.Brand,
                    Cost = shoe.Cost,
                    Price = shoe.Price,
                    Description = shoe.Description,
                    ImageUrl = shoe.ImageUrl ?? "",
                    StockQuantity = totalStock,
                    size = shoe.size,
                    ColorName = shoe.ColorName ?? "Default",
                    AvailabilityStatus = shoe.AvailabilityStatus ?? "Available",
                    IsActive = shoe.IsActive
                };

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading shoe: {ex.Message}";
                Console.WriteLine($"Edit GET Error: {ex}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateShoeDto dto, IFormFile ImageFile)
        {
            try
            {
                if (id != dto.Id)
                {
                    TempData["Error"] = "Invalid shoe ID.";
                    return RedirectToAction(nameof(Index));
                }
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    ModelState.Remove("ImageUrl");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Edit Validation Error: {error.ErrorMessage}");
                    }
                    TempData["Error"] = "Please fix the validation errors.";
                    return View(dto);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        dto.ImageUrl = await SaveImageFileAsync(ImageFile);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error uploading image: {ex.Message}";
                        return View(dto);
                    }
                }
                else
                {
                    var existingShoe = await _inventoryService.GetShoeByIdAsync(id);
                    if (existingShoe != null && !string.IsNullOrEmpty(existingShoe.ImageUrl))
                    {
                        dto.ImageUrl = existingShoe.ImageUrl;
                    }
                }

                await _inventoryService.UpdateShoeAsync(id, dto);

                await _inventoryService.UpdateShoeStockAsync(id, dto.StockQuantity);

                TempData["Success"] = $"Shoe '{dto.Name}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating shoe: {ex.Message}";
                Console.WriteLine($"Edit POST Error: {ex}");
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["Error"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }

                var reports = await _inventoryService.GetInventoryReportAsync();
                var totalStock = reports
                    .Where(r => r.ShoeColorVariationId == id)
                    .Sum(r => r.StockQuantity);

                shoe.StockQuantity = totalStock;

                return View(shoe);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading shoe details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShoeDetailsJson(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                    return NotFound();

                var reports = await _inventoryService.GetInventoryReportAsync();
                var totalStock = reports
                    .Where(r => r.ShoeColorVariationId == id)
                    .Sum(r => r.StockQuantity);

                return Json(new
                {
                    id = shoe.Id,
                    name = shoe.Name,
                    stockQuantity = totalStock,
                    isActive = shoe.IsActive
                });
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var shoe = await _inventoryService.GetShoeByIdAsync(id);
                if (shoe == null)
                {
                    TempData["Error"] = "Shoe not found.";
                    return RedirectToAction(nameof(Index));
                }

                await _inventoryService.DeleteShoeAsync(id);
                TempData["Success"] = $"Shoe '{shoe.Name}' deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting shoe: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddColorVariation(int shoeId, string colorName, string hexCode, int stockQuantity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colorName) || string.IsNullOrWhiteSpace(hexCode))
                {
                    return Json(new { success = false, message = "Color name and hex code are required." });
                }

                if (stockQuantity < 0)
                {
                    return Json(new { success = false, message = "Stock quantity cannot be negative." });
                }

                await _inventoryService.AddColorVariationAsync(shoeId, stockQuantity, colorName, hexCode);
                return Json(new { success = true, message = "Color variation added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<string> SaveImageFileAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("No file provided");
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size cannot exceed 5MB");
            }


            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Only JPG, PNG, and GIF images are allowed");
            }

           
            string uploadDir = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }


            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadDir, uniqueFileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }


            return $"/images/{uniqueFileName}";
        }
    }
}