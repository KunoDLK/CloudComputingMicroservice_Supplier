namespace Products_Service.Supplier
{
    public enum SupplierSources
    {
        TestMockSuppliers = 0
    }

    public interface IProductSupplier
    {
        decimal PriceCheck(int SupplierId);
        int AvailableStock(int SupplierId);

    }

    public abstract class ProductSupplier : IProductSupplier
    {
        public SupplierSources SupplierSource { get; }

        public abstract int AvailableStock(int SupplierId);
        public abstract decimal PriceCheck(int SupplierId);

        public ProductSupplier(SupplierSources supplierSources)
        {
            SupplierSource = supplierSources;
        }
    }
}