using FougeraClub.Application.DTOs;

namespace FougeraClub.Application.Interfaces.Services
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDto>> GetOrdersAsync(string supplierName, DateTime? fromDate, DateTime? toDate);
        Task<PurchaseOrderDto> CreateNewOrderDtoAsync();
        Task<PurchaseOrderDto?> GetOrderDtoByIdAsync(int id);
        Task<List<SupplierLookupDto>> GetSuppliersAsync();
        Task SaveAsync(PurchaseOrderDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
