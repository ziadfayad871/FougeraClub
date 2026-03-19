using FougeraClub.Domain.Entities;

namespace FougeraClub.Application.Interfaces.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task<List<PurchaseOrder>> GetOrdersAsync(string supplierName, DateTime? fromDate, DateTime? toDate);
        Task<PurchaseOrder?> GetLastOrderAsync();
        Task<PurchaseOrder?> GetByIdWithItemsAsync(int id);
        Task<List<Supplier>> GetSuppliersAsync();
        Task AddOrderAsync(PurchaseOrder order);
        Task<bool> DeleteOrderAsync(int id);
        Task RemoveOrderItemsAsync(IEnumerable<PurchaseOrderItem> items);
        Task SaveChangesAsync();
    }
}
