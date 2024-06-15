
using ToDoList.Model;

namespace ToDoList.Dto;

public class ListItemDTO
{
    public ListItemDTO()
    {
        // parameter less constructors are required for json deserialization to work
    }

    public ListItemDTO(ListItem item)
    {
        Id = item.Id;
        Text = item.Text;
        ListItemGroupId = item.ListItemGroupId;
        IsChecked = item.IsChecked;
    }

    public int Id { get; set; }
    public int ListItemGroupId { get; set; }
    public string Text { get; set; } = "";
    public bool IsChecked { get; set; } = false;

    public ListItem ToListItem(int UserId)
    {
        return new ListItem()
        {
            Id = this.Id,
            ListItemGroupId = ListItemGroupId,
            Text = this.Text,
            IsChecked = this.IsChecked,
            UserId = UserId
        };
    }
}