using System;
using System.Collections.Generic;

namespace FougeraClub.Domain.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Remarks { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool ApplyVat { get; set; }

        // Link to Supplier (Manager)
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; } = null!;

        // Navigation property for child items
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
