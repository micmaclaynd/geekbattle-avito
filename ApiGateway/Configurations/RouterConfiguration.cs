namespace ApiGateway.Configurations {
    public class RouterConfiguration {
        public string DefaultContentType { get; set; }
        public string DefaultTransferEncoding { get; set; }
        public IEnumerable<RouteConfiguration> Routes { get; set; }
    }
}
