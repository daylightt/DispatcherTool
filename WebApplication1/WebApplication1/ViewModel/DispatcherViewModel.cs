using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class DispatcherViewModel
    {
        public virtual ICollection<Engineer> Engineers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<EngineerSkill> EngineerSkills { get; set; }
        public virtual ICollection<TaskCategory> Categories { get; set; }

        public Dictionary<String, List<String>> EngineerSchedule { get; set; }
        public Dictionary<String, List<String>> WeeklySchedule { get; set; }
        public Dictionary<String, List<String>> OptionalSchedule { get; set; }

    }
}