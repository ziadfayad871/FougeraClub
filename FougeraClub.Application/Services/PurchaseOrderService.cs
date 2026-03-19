using AutoMapper;
using FougeraClub.Application.DTOs;
using FougeraClub.Application.Interfaces.Repositories;
using FougeraClub.Application.Interfaces.Services;
using FougeraClub.Domain.Entities;

namespace FougeraClub.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<PurchaseOrderDto>> GetOrdersAsync(string supplierName, DateTime? fromDate, DateTime? toDate)
        {
            var orders = await _repo.GetOrdersAsync(supplierName, fromDate, toDate);
            return _mapper.Map<List<PurchaseOrderDto>>(orders);
        }

        public async Task<PurchaseOrderDto> CreateNewOrderDtoAsync()
        {
            var lastOrder = await _repo.GetLastOrderAsync();
            var nextNumber = 100;
            if (lastOrder != null && int.TryParse(lastOrder.OrderNumber, out var lastNum))
            {
                nextNumber = lastNum + 1;
            }

            return new PurchaseOrderDto
            {
                OrderNumber = nextNumber.ToString(),
                OrderDate = DateTime.Now,
                Items = new List<PurchaseOrderItemDto>()
            };
        }

        public async Task<PurchaseOrderDto?> GetOrderDtoByIdAsync(int id)
        {
            var order = await _repo.GetByIdWithItemsAsync(id);
            return order == null ? null : _mapper.Map<PurchaseOrderDto>(order);
        }

        public async Task<List<SupplierLookupDto>> GetSuppliersAsync()
        {
            var suppliers = await _repo.GetSuppliersAsync();
            return suppliers
                .Select(s => new SupplierLookupDto { Id = s.Id, Name = s.Name })
                .ToList();
        }

        public async Task SaveAsync(PurchaseOrderDto dto)
        {
            decimal total = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
            if (dto.ApplyVat)
            {
                total += total * 0.05m;
            }

            dto.TotalAmount = total;

            if (dto.Id == 0)
            {
                var order = _mapper.Map<PurchaseOrder>(dto);
                order.Items = _mapper.Map<List<PurchaseOrderItem>>(dto.Items);
                await _repo.AddOrderAsync(order);
            }
            else
            {
                var order = await _repo.GetByIdWithItemsAsync(dto.Id);
                if (order == null)
                {
                    throw new InvalidOperationException("Order not found");
                }

                var existingItems = order.Items.ToList();
                await _repo.RemoveOrderItemsAsync(existingItems);

                _mapper.Map(dto, order);
                order.Items = _mapper.Map<List<PurchaseOrderItem>>(dto.Items);
            }

            await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteOrderAsync(id);
        }
    }
}
