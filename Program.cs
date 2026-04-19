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
            displayAppTitle();
            using var db = new HabitDb();
            db.Database.EnsureCreated();
            while(true)
            {
                MainMenu(db);
            }
        }
        static void displayAppTitle()
        {
            var appName = new FigletText("Habit Logger")
        {
            Color = Color.Blue,
            Justification = Justify.Center
        };
        
        AnsiConsole.Write(appName);
        AnsiConsole.WriteLine();
        }
        static void MainMenu(HabitDb db)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Choose an option")
                .AddChoices("Add a new habit", "See all habits", "Edit a habit", "Delete a habit", "Delete all habits","Exit"));

            switch (choice)
            {
                case "Add a new habit":
                    Console.Clear();
                    AddHabit(db);
                    break;
                case "See all habits":
                    Console.Clear();
                    ShowHabits(db);
                    break;
                case "Edit a habit":
                    Console.Clear();
                    EditHabit(db);
                    break;
                case "Delete a habit":
                    Console.Clear();
                    DeleteHabit(db);
                    break;
                case "Delete all habits":
                    Console.Clear();
                    if (AnsiConsole.Confirm("[red]This option will delete everything,Continue with Deletion[/]?"))
                    {
                        DeleteAllHabits(db);
                        AnsiConsole.MarkupLine("[green]All habits have been deleted successfully[/]");
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
            if (!db.Habits.Any())
            {
                AnsiConsole.MarkupLine("[red]No habits found![/]");
                return;
            }
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
        static void EditHabit(HabitDb db)
        {
            if (!db.Habits.Any())
            {
                AnsiConsole.MarkupLine("[red]No habits found![/]");
                return;
            }

            var habit = AnsiConsole.Prompt(
                new SelectionPrompt<Habit>()
                    .Title("Select a habit to edit:")
                    .UseConverter(h => $"{h.Id} - {h.Name}")
                    .AddChoices(db.Habits.ToList())
            );

            var field = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                .Title("Select [green]fields[/] to edit:")
                .AddChoices("Name", "Quantity", "Date")
            );

            if (field.Contains("Name"))
            {
                var newName = AnsiConsole.Ask<string>("Enter new name:");
                if (!string.IsNullOrWhiteSpace(newName))
                    habit.Name = newName;
            }

            if (field.Contains("Quantity"))
            {
                habit.Quantity = AnsiConsole.Ask<int>("Enter new quantity:");
            }

            if (field.Contains("Date"))
            {
                habit.Date = AnsiConsole.Ask<DateTime>("Enter new date:");
            }

            db.SaveChanges();

            AnsiConsole.MarkupLine("[green]Habit updated![/]");
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
