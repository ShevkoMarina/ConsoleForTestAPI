using System;

namespace ConsoleForTestAPI.Models
{
    class GetPhotoByDateRequest
    {
        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}
