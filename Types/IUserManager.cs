using ToDoList.Model;

public interface IUserManager
{
    bool IsAuthenticated
    {
        get;
    }

    User User
    {
        get;
    }
}