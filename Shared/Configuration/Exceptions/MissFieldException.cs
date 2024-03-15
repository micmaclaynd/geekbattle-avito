namespace Shared.Configuration.Exceptions {
    public class MissFieldException(string field) : Exception($"{field} missing") { }
}