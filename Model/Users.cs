
namespace ToDoList.Model;

public class User
{
    public static User New(string name, string password)
    {
        return new User()
        {
            Id = 0,
            UserName = name,
            Password = password
        };
    }

    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}