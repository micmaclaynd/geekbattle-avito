namespace Shared.Interfaces.Analytics {
    public class IAddPriceHttpRequest {
        public string CategoryName { get; set; }
        public string LocationName { get; set; }
        public string MatrixName { get; set; }
        public double Price { get; set; }
    }
}
