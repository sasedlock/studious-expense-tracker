namespace ExpenseTracker.Repository.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class ExpenseGroupStatus : ExpenseGroupModel
    {
        public ExpenseGroupStatus()
        {
            ExpenseGroups = new HashSet<ExpenseGroup>();
        }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<ExpenseGroup> ExpenseGroups { get; set; }
    }
}
