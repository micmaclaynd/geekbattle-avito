namespace ApiGateway.Configurations {
    public class RouteConfiguration {
        public bool IsNestedUrls { get; set; } = false;
        public string Path { get; set; }
        public bool IsRequiredAuth { get; set; } = false;
        public Uri ProxyUri { get; set; }
        public IEnumerable<string> Methods { get; set; }
    }
}
