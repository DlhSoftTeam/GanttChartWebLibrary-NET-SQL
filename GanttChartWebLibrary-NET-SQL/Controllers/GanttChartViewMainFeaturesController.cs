using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DlhSoft.Web.Mvc;
using System.Diagnostics;
using System.Net;
using GanttChartWebLibrary_NET_SQL.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GanttChartWebLibrary_NET_SQL.Controllers.Controllers
{
    public class GanttChartViewMainFeaturesController : Controller
    {
        private static readonly DateTime date = DateTime.Today;
        private static readonly int year = date.Year, month = date.Month;

        // GET: /<controller>/
        public ActionResult Index()
        {
            return View(model: LoadData());
        }

        private List<GanttChartItem> LoadData()
        {
            // Create a new database context.
            using (var context = new DatabaseEntities())
            {
                // Prepare data items mapping them to task ID values, and pre-compute project start and finish date and times to be able to set the timeline of the view..
                var items = new List<GanttChartItem>();
                Dictionary<int, GanttChartItem> taskItemMap = new Dictionary<int, GanttChartItem>();
                foreach (Task task in context.Tasks.OrderBy(t => t.Index))
                {
                    var item = new GanttChartItem
                    {
                        Content = task.Name,
                        Indentation = task.Indentation,
                        Start = task.Start,
                        Finish = task.Finish,
                        CompletedFinish = task.Completion,
                        IsMilestone = task.IsMilestone,
                        AssignmentsContent = task.Assignments,
                        Key = task.ID
                    };
                    items.Add(item);
                    taskItemMap.Add(task.ID, item);
                }
                // Prepare predecessor data items, using the pre-established map between task ID values and the view items created in the previous step.
                foreach (Predecessor predecessor in context.Predecessors)
                {
                    GanttChartItem dependentTaskItem = taskItemMap[predecessor.DependentTaskID];
                    GanttChartItem predecessorTaskItem = taskItemMap[predecessor.PredecessorTaskID];
                    var predecessorItem = new PredecessorItem
                    {
                        Item = predecessorTaskItem,
                        DependencyType = (DependencyType)predecessor.DependencyType,
                        Key = predecessor.PredecessorTaskID
                    };
                    dependentTaskItem.Predecessors.Add(predecessorItem);
                }

                // Set the items to the view.
                return items;
            }
        }

        public ActionResult UpdateGanttChartItem(GanttChartItem item)
        {
            SaveData(item);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void SaveData(GanttChartItem item)
        {
            // Create a new database context.
            using (var context = new DatabaseEntities())
            {
                int taskID = item.Key;
                Task task = context.Tasks.Where(t => t.ID == taskID).SingleOrDefault();
                if (task == null)
                    return;
                // Update the appropriate task property values.
                task.Name = item.Content?.ToString() ?? string.Empty;
                task.Indentation = item.Indentation;
                task.Start = item.Start;
                task.Finish = item.Finish;
                task.Completion = item.CompletedFinish;
                task.IsMilestone = item.IsMilestone;
                task.Assignments = item.AssignmentsContent?.ToString() ?? string.Empty;
                // Remove predecessors that are no longer in the view.
                var predecessorsToDelete = new List<Predecessor>();
                foreach (Predecessor predecessor in task.Predecessors)
                {
                    if (!item.Predecessors.Any(p => p.Key == predecessor.PredecessorTaskID))
                        predecessorsToDelete.Add(predecessor);
                }
                foreach (Predecessor predecessor in predecessorsToDelete)
                    task.Predecessors.Remove(predecessor);
                // Add new and update existing predecessors based on the view.
                foreach (PredecessorItem predecessorItem in item.Predecessors)
                {
                    var predecessorTaskID = context.Tasks.Single(t => t.Index == predecessorItem.ItemIndex).ID;
                    var predecessor = task.Predecessors.Where(p => p.PredecessorTaskID == predecessorTaskID).SingleOrDefault();
                    if (predecessor == null) // create new predecessor if needed
                    {
                        predecessor = new Predecessor { PredecessorTaskID = predecessorTaskID };
                        task.Predecessors.Add(predecessor);
                    }
                    predecessor.DependencyType = (int)predecessorItem.DependencyType;
                }
                // Actually save changes to the database.
                context.SaveChanges();
            }
        }
    }
}
