using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace HabitLogger
{
    internal class Habbit
    {
        private int Id { get; set; }
        private string Name { get; set; }
        private int Quantity { get; set; }
        private DateTime Date { get; set; }
    }
}
