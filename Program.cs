using Spectre.Console;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HabitLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new HabitDb();
            db.Database.EnsureCreated();
            AnsiConsole.MarkupLine($"[bold blue] {db.DbPath} [/]!");
            db.SaveChanges();
        }
    }
}
