namespace Shared.Interfaces.Analytics {
    public class ICategory {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint ParentId { get; set; }
        public uint UserId { get; set; }
    }
}
