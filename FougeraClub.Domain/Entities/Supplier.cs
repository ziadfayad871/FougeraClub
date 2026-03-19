using System.Collections.Generic;

namespace FougeraClub.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        
        // The signature as image data 
        public string? SignatureData { get; set; }

        // Navigation property for purchase orders
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
