namespace Ecommerce.Data
{
public class UserProfile
{
    public string Username { get; set; }

    public UserProfile(string username)
    {
        Username = username;
    }
}

}