using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;

namespace ShoeShop.Web.Controllers
{
    public class PullOutController : Controller
    {
        private readonly IPullOutService _pullOutService;

        public PullOutController(IPullOutService pullOutService)
        {
            _pullOutService = pullOutService;
        }

        public async Task<IActionResult> Index()
        {
            var pullOuts = await _pullOutService.GetPendingPullOutsAsync();

            var viewModel = pullOuts.Select(p => new PullOutsViewModel
            {
                Id = p.Id,
                ShoeColorVariationId = p.ShoeColorVariationId,
                Quantity = p.Quantity,
                Reason = p.Reason,
                ReasonDetails = p.ReasonDetails,
                RequestedBy = p.RequestedBy,
                RequestedDate = p.RequestedDate,
                Status = p.Status
            }).ToList();

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatePullOutDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction("Details", "Inventory", new { id = dto.ShoeColorVariationId });
            }

            try
            {
                await _pullOutService.CreatePullOutRequestAsync(dto);
                TempData["Success"] = "Pull-out request submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Inventory", new { id = dto.ShoeColorVariationId });
        }


        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _pullOutService.ApprovePullOutAsync(id, User?.Identity?.Name ?? "Admin");
                TempData["Success"] = "Pull-out request approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
