using System;
using System.Runtime.InteropServices;

namespace ConsoleForTestAPI
{
    public class User
    {
        public int Id { get; set; }

        public string UserLogin { get; set; }

        public string UserPassword { get; set; }

        public override string ToString()
        {
            return $"User: Id = {Id}, Login = {UserLogin}, Password = {UserPassword}";
        }
    }
}