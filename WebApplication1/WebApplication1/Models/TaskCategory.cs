//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskCategory
    {
        public TaskCategory()
        {
            this.Tasks = new HashSet<Task>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
