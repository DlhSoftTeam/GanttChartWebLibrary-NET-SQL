//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GanttChartWebLibrary_NET_SQL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Predecessor
    {
        public int DependentTaskID { get; set; }
        public int PredecessorTaskID { get; set; }
        public int DependencyType { get; set; }
    
        public virtual Task DependentTask { get; set; }
        public virtual Task Task { get; set; }
    }
}