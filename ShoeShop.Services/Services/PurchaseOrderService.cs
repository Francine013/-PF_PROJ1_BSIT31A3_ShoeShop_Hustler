using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Repository.Entities;
using ShoeShop.Repository.Entities.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeShop.Services.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ShoeShopDbContext _db;

        public PurchaseOrderService(ShoeShopDbContext db) => _db = db;

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            if (!await _db.Suppliers.AnyAsync(x => x.Id == dto.SupplierId))
                throw new KeyNotFoundException("Supplier not found");

            var po = new PurchaseOrder
            {
                OrderNumber = dto.OrderNumber,
                SupplierId = dto.SupplierId,
                OrderDate = dto.OrderDate,
                Status = OrderStatus.Pending,
                TotalAmount = 0m,
                ExpectedDate = dto.ExpectedDate ?? DateTime.Now,
            };

            _db.PurchaseOrders.Add(po);
            await _db.SaveChangesAsync();

            decimal total = 0m;
            foreach (var item in dto.Items)
            {
                var poi = new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    ShoeColorVariationId = item.ShoeColorVariationId,
                    QuantityOrdered = item.QuantityOrdered,
                    QuantityReceived = 0,
                    UnitCost = item.UnitCost
                };
                total += item.QuantityOrdered * item.UnitCost;
                _db.PurchaseOrderItems.Add(poi);
            }

            po.TotalAmount = total;
            await _db.SaveChangesAsync();

            return await GetByIdAsync(po.Id) ?? throw new Exception("Failed to load PO");
        }
        public async Task UpdatePurchaseOrderAsync(int id, PurchaseOrderDto dto)
        {
            var po = await _db.PurchaseOrders
                .Include(p => p.OrderItems) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null)
            {
                throw new KeyNotFoundException($"Purchase Order with ID {id} not found.");
            }

            if (po.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot edit a purchase order that is not in Pending status.");
            }

            
            po.OrderNumber = dto.OrderNumber;
            po.OrderDate = dto.OrderDate;
            po.ExpectedDate = dto.ExpectedDate;
            
            _db.PurchaseOrderItems.RemoveRange(po.OrderItems);

            decimal newTotal = 0m;
            var newItems = new List<PurchaseOrderItem>();

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.QuantityOrdered <= 0) continue;

                var newItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    ShoeColorVariationId = itemDto.ShoeColorVariationId,
                    QuantityOrdered = itemDto.QuantityOrdered,
                    QuantityReceived = 0,
                    UnitCost = itemDto.UnitCost
                };
                newTotal += itemDto.QuantityOrdered * itemDto.UnitCost;
                newItems.Add(newItem);
            }

            
            po.OrderItems = newItems;
            po.TotalAmount = newTotal;

         
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear(); 
        }

       

        public async Task DeletePurchaseOrderAsync(int id)
        {
            var po = await _db.PurchaseOrders
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null)
            {
                throw new KeyNotFoundException($"Purchase Order with ID {id} not found.");
            }

            if (po.Status == OrderStatus.Received)
            {
                throw new InvalidOperationException("Cannot delete a Purchase Order that has already been received.");
            }

            _db.PurchaseOrderItems.RemoveRange(po.OrderItems);
            _db.PurchaseOrders.Remove(po);

            await _db.SaveChangesAsync();
        }

        public async Task ReceivePurchaseOrderAsync(int purchaseOrderId)
        {
            var po = await _db.PurchaseOrders.Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.Id == purchaseOrderId)
                ?? throw new KeyNotFoundException("Purchase order not found");

            if (po.Status == OrderStatus.Received)
                throw new InvalidOperationException("Already received");

            foreach (var item in po.OrderItems)
            {
                var cv = await _db.ShoeColorVariations.FindAsync(item.ShoeColorVariationId)
                             ?? throw new KeyNotFoundException("Color variation not found");

                cv.StockQuantity += item.QuantityOrdered;
                item.QuantityReceived = item.QuantityOrdered;
            }

            po.Status = OrderStatus.Received;
            await _db.SaveChangesAsync();
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
        {
            var po = await _db.PurchaseOrders
                .Include(p => p.OrderItems)
                    .ThenInclude(poi => poi.ShoeColorVariation)
                        .ThenInclude(scv => scv.Shoe)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return null;

            return new PurchaseOrderDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                SupplierId = po.SupplierId,
                SupplierName = po.Supplier.Name,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                Status = po.Status.ToString(),
                TotalAmount = po.TotalAmount,

                Items = po.OrderItems.Select(i => new PurchaseOrderItemDto
                {
                    Id = i.Id,
                    ShoeColorVariationId = i.ShoeColorVariationId,
                    QuantityOrdered = i.QuantityOrdered,
                    QuantityReceived = i.QuantityReceived,
                    UnitCost = i.UnitCost,

                    ProductName = i.ShoeColorVariation?.Shoe?.Name + " - " + i.ShoeColorVariation?.ColorName,
                    UnitPrice = i.ShoeColorVariation?.Shoe?.Price ?? 0m
                }).ToList()
            };
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync()
        {
            var list = await _db.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.OrderItems)
                .ToListAsync();

            var dtos = new List<PurchaseOrderDto>();
            foreach (var p in list)
            {
                dtos.Add(new PurchaseOrderDto
                {
                    Id = p.Id,
                    OrderNumber = p.OrderNumber,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier.Name,
                    OrderDate = p.OrderDate,
                    ExpectedDate = p.ExpectedDate,
                    Status = p.Status.ToString(),
                    TotalAmount = p.TotalAmount,

                    Items = p.OrderItems.Select(i => new PurchaseOrderItemDto
                    {
                        Id = i.Id,
                        ShoeColorVariationId = i.ShoeColorVariationId,
                        QuantityOrdered = i.QuantityOrdered,
                        QuantityReceived = i.QuantityReceived,
                        UnitCost = i.UnitCost
                    }).ToList()
                });
            }

            return dtos;
        }
    }
}