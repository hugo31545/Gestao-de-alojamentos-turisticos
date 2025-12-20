using POOfase1trabalho13_11.Models.Entities.Interfaces;

public class User : IEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime DataRegisto { get; set; }
}