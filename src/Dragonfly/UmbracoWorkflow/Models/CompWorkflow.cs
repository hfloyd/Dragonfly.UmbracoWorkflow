namespace Dragonfly.UmbracoWorkflow.Models
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web;

    /// <summary>Comp Workflow</summary>
    [PublishedContentModel("CompWorkflow")]
    public partial class CompWorkflow : PublishedContentModel, ICompWorkflow
    {
#pragma warning disable 0109 // new is redundant
        public new const string ModelTypeAlias = "CompWorkflow";
        public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

        public CompWorkflow(IPublishedContent content) : base(content) { }

#pragma warning disable 0109 // new is redundant
        public new static PublishedContentType GetModelContentType()
        {
            return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
        }
#pragma warning restore 0109

        //public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<CompWorkflow, TValue>> selector)
        //{
        //	return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
        //}

        ///<summary>
        /// Assigned To
        ///</summary>
        //[ImplementPropertyType("WorkflowAssignedTo")]
        public object WorkflowAssignedTo
        {
            get { return GetWorkflowAssignedTo(this); }
        }

        /// <summary>Static getter for Assigned To</summary>
        public static object GetWorkflowAssignedTo(ICompWorkflow that) { return that.GetPropertyValue("WorkflowAssignedTo"); }

        ///<summary>
        /// Current Status
        ///</summary>
        //[ImplementPropertyType("WorkflowCurrentStatus")]
        public string WorkflowCurrentStatus
        {
            get { return GetWorkflowCurrentStatus(this); }
        }

        /// <summary>Static getter for Current Status</summary>
        public static string GetWorkflowCurrentStatus(ICompWorkflow that) { return that.GetPropertyValue<string>("WorkflowCurrentStatus"); }

        ///<summary>
        /// Current Status Notes
        ///</summary>
        //[ImplementPropertyType("WorkflowCurrentStatusNotes")]
        public string WorkflowCurrentStatusNotes
        {
            get { return GetWorkflowCurrentStatusNotes(this); }
        }

        /// <summary>Static getter for Current Status Notes</summary>
        public static string GetWorkflowCurrentStatusNotes(ICompWorkflow that) { return that.GetPropertyValue<string>("WorkflowCurrentStatusNotes"); }

        ///<summary>
        /// Due Date
        ///</summary>
        //[ImplementPropertyType("WorkflowDueDate")]
        public DateTime WorkflowDueDate
        {
            get { return GetWorkflowDueDate(this); }
        }

        /// <summary>Static getter for Due Date</summary>
        public static DateTime GetWorkflowDueDate(ICompWorkflow that) { return that.GetPropertyValue<DateTime>("WorkflowDueDate"); }

        ///<summary>
        /// Exclude from Workflow Tracking
        ///</summary>
        //[ImplementPropertyType("WorkflowExcludeFromTracking")]
        public bool WorkflowExcludeFromTracking
        {
            get { return GetWorkflowExcludeFromTracking(this); }
        }

        /// <summary>Static getter for Exclude from Workflow Tracking</summary>
        public static bool GetWorkflowExcludeFromTracking(ICompWorkflow that) { return that.GetPropertyValue<bool>("WorkflowExcludeFromTracking"); }

        ///<summary>
        /// References: References to Designs, Content, etc.
        ///</summary>
        //[ImplementPropertyType("WorkflowReferences")]
        public IEnumerable<RJP.MultiUrlPicker.Models.Link> WorkflowReferences
        {
            get { return GetWorkflowReferences(this); }
        }

        /// <summary>Static getter for References</summary>
        public static IEnumerable<RJP.MultiUrlPicker.Models.Link> GetWorkflowReferences(ICompWorkflow that) { return that.GetPropertyValue<IEnumerable<RJP.MultiUrlPicker.Models.Link>>("WorkflowReferences"); }

        ///<summary>
        /// Tags
        ///</summary>
        //[ImplementPropertyType("WorkflowTags")]
        public IEnumerable<string> WorkflowTags
        {
            get { return GetWorkflowTags(this); }
        }

        /// <summary>Static getter for Tags</summary>
        public static IEnumerable<string> GetWorkflowTags(ICompWorkflow that) { return that.GetPropertyValue<IEnumerable<string>>("WorkflowTags"); }
    }
}
