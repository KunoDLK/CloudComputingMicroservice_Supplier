namespace Products_Service.Supplier
{
        enum SupplierSources
        {
                TestMockSuppliers = 0
        }

        interface IProductSupplier
        {
                decimal PriceCheck(int SupplierId);
        }
}