
using ToDoList.Model;

namespace ToDoList.Dto;

// TODO: Refactor to partial class or base with inheritance
public class ListItemGroupDTO
{
    public ListItemGroupDTO()
    {
        // parameter less constructors are required for json deserialization to work
    }

    public ListItemGroupDTO(ListItemGroup group)
    {
        Id = group.Id;
        ListName = group.ListName;
    }

    public int Id { get; set; }
    public string ListName { get; set; } = "New list";

    public ListItemGroup ToListItem(int UserId)
    {
        return new ListItemGroup()
        {
            Id = this.Id,
            UserId = UserId,
            ListName = this.ListName,
        };
    }
}