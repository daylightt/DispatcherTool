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
    
    public partial class Task
    {
        public int Id { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> CustomerAccount { get; set; }
        public Nullable<int> TaskCategory { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<bool> isOpen { get; set; }
        public string SkillsRequired { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Engineer Engineer { get; set; }
        public virtual Status Status1 { get; set; }
        public virtual Task Task1 { get; set; }
        public virtual Task Task2 { get; set; }
        public virtual TaskCategory TaskCategory1 { get; set; }
    }
}
