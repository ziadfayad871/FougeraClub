using FougeraClub.Domain.Entities;

namespace FougeraClub.Application.Interfaces.Repositories
{
    public interface IPurchaseOrderRepository : IGenericRepository<PurchaseOrder>
    {
        Task<List<PurchaseOrder>> GetOrdersAsync(string supplierName, DateTime? fromDate, DateTime? toDate);
        Task<PurchaseOrder?> GetLastOrderAsync();
        Task<PurchaseOrder?> GetByIdWithItemsAsync(int id);
        Task<List<Supplier>> GetSuppliersAsync();
        Task<bool> DeleteOrderAsync(int id);
        void RemoveOrderItems(IEnumerable<PurchaseOrderItem> items);
    }
}
