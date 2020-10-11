using System;

namespace ConsoleForTestAPI.Models
{
    class Photo
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } 

        public byte[] ImageSource { get; set; }

        public int? UserId { get; set; }

        public override string ToString()
        {
            return $"Photo: Id = {Id}, Date = {DateCreated.ToString("dd/MM/yy")}, User id = {UserId}";
        }
    }
}
