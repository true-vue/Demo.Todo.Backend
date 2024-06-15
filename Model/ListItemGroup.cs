
namespace ToDoList.Model;

public class ListItemGroup
{
    public static ListItemGroup New(int userId, string listName)
    {
        return new ListItemGroup()
        {
            Id = 0,
            UserId = userId,
            ListName = listName,
        };
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public string ListName { get; set; } = "New list";
}