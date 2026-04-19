using Spectre.Console;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HabitLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new HabitDb();
            db.Database.EnsureCreated();
            while(true)
            {
                MainMenu(db);
            }
        }
        static void MainMenu(HabitDb db)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Choose an option")
                .AddChoices("Add a new habit", "See all habits", "Delete a habit", "Delete all habits","Exit"));

            switch (choice)
            {
                case "Add a new habit":
                    AddHabit(db);
                    break;
                case "See all habits":
                    ShowHabits(db);
                    break;
                case "Delete a habit":
                    DeleteHabit(db);
                    break;
                case "Delete all habits":
                    if (AnsiConsole.Confirm("[red]This option will delete everything,Continue with Deletion[/]?"))
                    {
                        DeleteAllHabits(db);
                        AnsiConsole.MarkupLine("[green]All habits has been deleted successfully[/]");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Operation cancelled[/]");
                        Thread.Sleep(1000);
                    }
                    break;
                case "Exit":
                    AnsiConsole.MarkupLine("[green]Program exited successfully![/]");
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                    break;
            }
        }
        static void AddHabit(HabitDb db)
        {
            var name = AnsiConsole.Ask<string>("Enter habit name:");
            var quantity = AnsiConsole.Ask<int>("Enter quantity:");

            var habit = new Habit
            {
                Name = name,
                Quantity = quantity,
                Date = DateTime.Now
            };

            db.Habits.Add(habit);
            db.SaveChanges();

            AnsiConsole.MarkupLine("[green]Habit added![/]");
        }
        static void ShowHabits(HabitDb db)
        {
            var allHabits = db.Habits.ToList();
            var table = new Table();

            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Quantity");
            table.AddColumn("Date");
            
            foreach (var habit in allHabits)
            {
                table.AddRow(
                    habit.Id.ToString(),
                    habit.Name,
                    habit.Quantity.ToString(),
                    habit.Date.ToShortDateString()
                );
            }
            
            AnsiConsole.Write(table);
        }
        static void DeleteHabit(HabitDb db)
        {
            ShowHabits(db);
            var id = AnsiConsole.Ask<int>("Enter habit ID to delete:");

            var habit = db.Habits.Find(id);

            if (habit == null)
            {
                AnsiConsole.MarkupLine("[red]Habit not found![/]");
                return;
            }

            db.Habits.Remove(habit);
            db.SaveChanges();

            AnsiConsole.MarkupLine("[green]Habit deleted![/]");
        }
        static void DeleteAllHabits(HabitDb db)
        {
            db.Habits.ExecuteDelete();
        }
    }
}
