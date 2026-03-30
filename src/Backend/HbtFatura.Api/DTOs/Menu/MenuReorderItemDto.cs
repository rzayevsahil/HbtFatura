namespace HbtFatura.Api.DTOs.Menu;

public class MenuReorderItemDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
}
