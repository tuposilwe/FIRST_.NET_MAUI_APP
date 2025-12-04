namespace MyMaui.Models
{
    public class Item // used by EF to create a Users table in the Db
    {
        // User is a Domain Model
        public int Id { get; set; } // Primary Key Column
        public required string Name { get; set; } // required Name Column

    }
}