// ShoeShop.Web/Pages/PullOuts/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Web.Pages.PullOuts
{
    public class IndexModel : PageModel
    {
        private readonly IPullOutService _pullOutService;
        private readonly ILogger<IndexModel> _logger; // Logger for debugging

        public IndexModel(IPullOutService pullOutService, ILogger<IndexModel> logger)
        {
            _pullOutService = pullOutService;
            _logger = logger;
        }

        [BindProperty]
        public IEnumerable<PullOutRequestDto> PendingPullOuts { get; set; } = new List<PullOutRequestDto>();

        public async Task OnGetAsync()
        {
            try
            {
                PendingPullOuts = await _pullOutService.GetPendingPullOutsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending pull-outs.");
                // Optionally show error message on page
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            try
            {
                // Note: Hardcoded "Admin" as the approvedBy user since no actual login is implemented yet.
                await _pullOutService.ApprovePullOutAsync(id, "AdminUser");
                TempData["SuccessMessage"] = $"Pull Out Request #{id} successfully approved and stock has been deducted.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = $"Pull Out Request #{id} not found.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"Error approving request: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving pull-out.");
                TempData["ErrorMessage"] = "An unexpected error occurred during approval.";
            }

            return RedirectToPage();
        }
    }
}