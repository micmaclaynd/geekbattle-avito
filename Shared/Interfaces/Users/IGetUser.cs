namespace Shared.Interfaces.Users {
    public class IGetUserByUsernameRequest {
        public string Username { get; set; }
    }

    public class IGetUserByUsernameResponse {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class IGetUserByIdRequest {
        public uint Id { get; set; }
    }

    public class IGetUserByIdResponse {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}