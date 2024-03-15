namespace Shared.Interfaces.Analytics {
    public class IAddCategoryHttpRequest {
        public string ParentName { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
