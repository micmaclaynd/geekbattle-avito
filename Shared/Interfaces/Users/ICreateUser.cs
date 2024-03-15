namespace Shared.Interfaces.Users {
    public class ICreateUserRequest {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ICreateUserResponse {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}