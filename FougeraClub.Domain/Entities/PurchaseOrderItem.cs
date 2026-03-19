namespace FougeraClub.Domain.Entities
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;

        // Foreign Key
        public int PurchaseOrderId { get; set; }
        
        // Navigation property to parent
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
