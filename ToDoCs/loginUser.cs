
namespace TodoApi.Models
{
    public class loginUser
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
