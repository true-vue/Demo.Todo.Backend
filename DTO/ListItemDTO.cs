
using ToDoList.Model;

namespace ToDoList.Dto;

public class ListItemDTO
{
    public ListItemDTO()
    {

    }

    public ListItemDTO(ListItem item)
    {
        Id = item.Id;
        Text = item.Text;
        IsChecked = item.IsChecked;
    }

    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsChecked { get; set; } = false;

    public ListItem ToListItem(int UserId)
    {
        return new ListItem()
        {
            Id = this.Id,
            Text = this.Text,
            IsChecked = this.IsChecked,
            UserId = UserId
        };
    }
}