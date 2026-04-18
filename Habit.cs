using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace HabitLogger
{
    internal class Habit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
