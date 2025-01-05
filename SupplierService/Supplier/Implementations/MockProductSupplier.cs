namespace Products_Service.Supplier.Implementation
{
      class MockProductSupplier : ProductSupplier
    {
        public MockProductSupplier(SupplierSources supplierSources) : base(supplierSources)
        {
        }
        
        public override int AvailableStock(int SupplierId)
        {
            return GenerateDeterministicValue(SupplierId, 0, 10000);
        }

        public override decimal PriceCheck(int SupplierId)
        {
            var price = GenerateDeterministicValue(SupplierId, 0, 5000) / 100m; // Scale to 0-50 range
            return price;
        }

        private int GenerateDeterministicValue(int seed, int min, int max)
        {
            var random = new Random((int)SupplierSource * seed); // Deterministic random based on seed
            return random.Next(min, max + 1);
        }
    }
}