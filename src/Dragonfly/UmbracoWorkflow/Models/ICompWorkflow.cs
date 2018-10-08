namespace Dragonfly.UmbracoWorkflow.Models
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Core.Models;

        public partial interface ICompWorkflow : IPublishedContent
        {
            /// <summary>Assigned To</summary>
            object WorkflowAssignedTo { get; }

            /// <summary>Current Status</summary>
            string WorkflowCurrentStatus { get; }

            /// <summary>Current Status Notes</summary>
            string WorkflowCurrentStatusNotes { get; }

            /// <summary>Due Date</summary>
            DateTime WorkflowDueDate { get; }

            /// <summary>Exclude from Workflow Tracking</summary>
            bool WorkflowExcludeFromTracking { get; }

            /// <summary>References</summary>
            IEnumerable<RJP.MultiUrlPicker.Models.Link> WorkflowReferences { get; }

            /// <summary>Tags</summary>
            IEnumerable<string> WorkflowTags { get; }
        }
    
}
