
namespace ToDoList.Model;

public class ListItem
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsChecked { get; set; } = false;
}