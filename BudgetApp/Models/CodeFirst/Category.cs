﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BudgetApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        public string Name { get; set; }

        public virtual Household Household { get; set; }

    }
}