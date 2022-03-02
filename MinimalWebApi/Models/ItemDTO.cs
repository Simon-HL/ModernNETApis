namespace MinimalWebApi.Models
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ItemDTO() { }
        public ItemDTO(Item item) =>
        (Id, Name) = (item.Id, item.Name);
    }  
}
