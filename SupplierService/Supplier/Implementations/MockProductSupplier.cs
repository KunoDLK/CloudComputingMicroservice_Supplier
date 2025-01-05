namespace Products_Service.Supplier.Implementation
{
        class MockProductSupplier : IProductSupplier
        {
                public decimal PriceCheck(int SupplierId)
                {
                        if (SupplierId == 1)
                        {
                                return new decimal(10.2);
                        }
                        else
                        {
                                throw new ArgumentException("Unknown Item");
                        }
                }
        }
}