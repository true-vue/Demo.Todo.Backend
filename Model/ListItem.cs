
namespace ToDoList.Model;

public class ListItem
{
    public static ListItem New(int userId, int ListItemGroupId, string text, bool isChecked = false)
    {
        return new ListItem()
        {
            Id = 0,
            ListItemGroupId = ListItemGroupId,
            Text = text,
            IsChecked = isChecked,
            UserId = userId
        };
    }

    public int Id { get; set; }
    public int ListItemGroupId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = "";
    public bool IsChecked { get; set; } = false;
}