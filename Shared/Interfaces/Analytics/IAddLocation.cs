namespace Shared.Interfaces.Analytics {
    public class IAddLocationHttpRequest {
        public string ParentName { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
