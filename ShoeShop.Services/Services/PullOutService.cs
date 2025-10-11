using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Repository.Entities;
using ShoeShop.Repository.Entities.Enums;

namespace ShoeShop.Services.Services
{
    public class PullOutService : IPullOutService
    {
        private readonly ShoeShopDbContext _db;
        private readonly int _managerApprovalThreshold = 10; // Example rule

        public PullOutService(ShoeShopDbContext db)
        {
            _db = db;
        }

        // -----------------------------
        // Create new pull-out request
        // -----------------------------
        public async Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var cv = await _db.ShoeColorVariations.FindAsync(dto.ShoeColorVariationId)
                     ?? throw new KeyNotFoundException("Color variation not found.");

            if (dto.Quantity > cv.StockQuantity)
                throw new InvalidOperationException("Insufficient stock.");

            var spo = new StockPullOut
            {
                ShoeColorVariationId = dto.ShoeColorVariationId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                ReasonDetails = dto.ReasonDetails,
                RequestedBy = dto.RequestedBy,
                RequestedDate = DateTime.UtcNow,
                Status = PullOutStatus.Pending
            };

            _db.StockPullOuts.Add(spo);
            await _db.SaveChangesAsync();

            return ToDto(spo);
        }

        // -----------------------------
        // Approve an existing pull-out
        // -----------------------------
        public async Task ApprovePullOutAsync(int pullOutId, string approvedBy)
        {
            var spo = await _db.StockPullOuts.FindAsync(pullOutId)
                      ?? throw new KeyNotFoundException("Pull-out not found.");

            if (spo.Status != PullOutStatus.Pending)
                throw new InvalidOperationException("Pull-out is not pending.");

            // Business rule: require manager approval when quantity > threshold
            if (spo.Quantity > _managerApprovalThreshold && string.IsNullOrEmpty(approvedBy))
                throw new InvalidOperationException("Manager approval required.");

            // Deduct stock
            var cv = await _db.ShoeColorVariations.FindAsync(spo.ShoeColorVariationId)
                     ?? throw new KeyNotFoundException("Color variation not found.");

            if (cv.StockQuantity < spo.Quantity)
                throw new InvalidOperationException("Insufficient stock to pull out.");

            cv.StockQuantity -= spo.Quantity;
            spo.ApprovedBy = approvedBy;
            spo.ApprovedDate = DateTime.UtcNow;
            spo.Status = PullOutStatus.Completed;

            await _db.SaveChangesAsync();
        }

        // -----------------------------
        // Get list of pending pull-outs
        // -----------------------------
        public async Task<IEnumerable<PullOutRequestDto>> GetPendingPullOutsAsync()
        {
            var list = await _db.StockPullOuts
                .Where(x => x.Status == PullOutStatus.Pending)
                .ToListAsync();

            return list.Select(ToDto).ToList();
        }

        // -----------------------------
        // Get all pull-outs (any status)
        // -----------------------------
        public async Task<IEnumerable<PullOutRequestDto>> GetAllPullOutsAsync()
        {
            var list = await _db.StockPullOuts.ToListAsync();
            return list.Select(ToDto).ToList();
        }

        // -----------------------------
        // Convert entity -> DTO
        // -----------------------------
        private PullOutRequestDto ToDto(StockPullOut s) => new()
        {
            Id = s.Id,
            ShoeColorVariationId = s.ShoeColorVariationId,
            Quantity = s.Quantity,
            Reason = s.Reason,
            ReasonDetails = s.ReasonDetails,
            RequestedBy = s.RequestedBy,
            ApprovedBy = s.ApprovedBy,
            RequestedDate = s.RequestedDate,
            ApprovedDate = s.ApprovedDate,
            Status = s.Status.ToString()
        };
    }
}
