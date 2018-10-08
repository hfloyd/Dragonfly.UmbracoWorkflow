namespace Dragonfly.UmbracoWorkflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Dragonfly.NetModels;
    using Dragonfly.Umbraco7Helpers;
    using Dragonfly.UmbracoWorkflow.Models;
    using Newtonsoft.Json;
    using Umbraco.Web.WebApi;

    // [IsBackOffice]
    // /Umbraco/backoffice/Api/WorkflowApi <-- UmbracoAuthorizedApiController

    [IsBackOffice]
    public class WorkflowApiController : UmbracoAuthorizedApiController
    {
        #region Content Workflow

        ///// /Umbraco/backoffice/Api/WorkflowApi/Workflow
        //[System.Web.Http.AcceptVerbs("GET")]
        //public HttpResponseMessage Workflow()
        //{
        //    return Workflow("");
        //}

        /// /Umbraco/backoffice/Api/WorkflowApi/Workflow?TagFilter=xxx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage Workflow(string TagFilter = "")
        {
            var returnSB = new StringBuilder();
            var fancyFormat = true;

            var pvPath = "~/Views/Partials/Workflow/WorkflowAsHtmlTable.cshtml";

            //FIND NODES TO DISPLAY
            var allNodes = WorkflowHelper.GetNodesWithWorkflowData(true).Where(n => !n.WorkflowExcludeFromTracking).ToList();
            
            if (Dragonfly.NetHelpers.Files.FileExists(pvPath))
            {
                IEnumerable<ICompWorkflow> filteredNodes = allNodes;
                bool isFiltered = false;
                var filtersList = new Dictionary<string, string>();

                if (TagFilter != "")
                {
                    isFiltered = true;
                    filteredNodes = filteredNodes.Where(n => n.WorkflowTags.Contains(TagFilter));
                    filtersList.Add("Tag", TagFilter); 
                }
            
                //VIEW DATA 
                var viewData = new ViewDataDictionary();
                viewData.Model = filteredNodes;
                viewData.Add("AllNodesCount", allNodes.Count);
                viewData.Add("IsFiltered", isFiltered);
                viewData.Add("FiltersList", filtersList);

                //RENDER
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData,
                        HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            else
            {
                //Use hard-coded builder
                returnSB = BuildWorkflowTable(TagFilter, allNodes);
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        private StringBuilder BuildWorkflowTable(string TagFilter, List<ICompWorkflow> AllNodes)
        {
            var returnSB = new StringBuilder();

            returnSB.AppendLine(HtmlStart());
            returnSB.AppendLine("<h1>Content Workflow Status</h1>");

            returnSB.AppendLine(string.Format("<p>Total Pages in Content Workflow: {0}</p>", AllNodes.Count()));

            IEnumerable<ICompWorkflow> filteredNodes = AllNodes;
            bool isFiltered = false;

            if (TagFilter != "")
            {
                isFiltered = true;
                filteredNodes = filteredNodes.Where(n => n.WorkflowTags.Contains(TagFilter));
                returnSB.AppendLine(string.Format("<p>Displaying {0} pages tagged with '{1}'.</p>", filteredNodes.Count(), TagFilter));
            }



            if (isFiltered)
            {
                returnSB.AppendLine("<p><b><a class=\"btn btn-warning\" href=\"Workflow\">Remove Filters and Show All</a></b></p>");
            }

            var tableStart = @"
                <table  id=""umbracodata"" class=""table table-striped table-bordered table-hover"" cellspacing=""0"" width=""100%""> 
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>Actions</th>  
                        <th>Parent</th>
                        <th>Node Name</th>
                        <th>Current Status</th>
                        <th>Status Notes</th>
                        <th>Assigned To</th>
                        <th>Due Date</th>
                        <th>Tags</th>
                        <th>References</th>
                        <th>Create Date</th>
                        <th>Update Date</th>
                        <th>DocType</th>
                      <!--  <th>NodeID</th> -->
                    </tr>
                    </thead>
                    <tbody>
                    ";

            var tableEnd = @"</tbody></table>";

            var tableData = new StringBuilder();
            var counter = 0;

            foreach (var workflowNode in filteredNodes)
            {
                counter++;

                tableData.AppendLine("<tr>");
                tableData.AppendLine($"<td>{counter}</td>");

                tableData.AppendLine($"<td>");
                tableData.AppendLine(
                    $"<a href=\"/umbraco#/content/content/edit/{workflowNode.Id}\" target=\"_blank\">Edit</a>");
                if (workflowNode.Url != "")
                {
                    tableData.AppendLine($" | <a href=\"{workflowNode.Url}\" target=\"_blank\">View</a> ");
                }
                tableData.AppendLine(string.Format("</td>"));

                if (workflowNode.Parent != null)
                {
                    tableData.AppendLine($"<td>{workflowNode.Parent.Name}</td>");
                }
                else
                {
                    tableData.AppendLine($"<td>---</td>");

                }
                tableData.AppendLine($"<td>{workflowNode.Name}</td>");

                tableData.AppendLine($"<td>{workflowNode.WorkflowCurrentStatus}</td>");
                tableData.AppendLine($"<td>{workflowNode.WorkflowCurrentStatusNotes}</td>");

                var assignedUser = WorkflowHelper.GetUser(workflowNode.WorkflowAssignedTo);
                if (assignedUser != null)
                {
                    tableData.AppendLine($"<td>{assignedUser.Name}</td>");
                }
                else
                {
                    tableData.AppendLine($"<td></td>");
                }

                if (workflowNode.WorkflowDueDate != DateTime.MinValue)
                {
                    tableData.AppendLine($"<td>{workflowNode.WorkflowDueDate}</td>");
                }
                else
                { tableData.AppendLine($"<td></td>"); }

                if (workflowNode.WorkflowTags != null && workflowNode.WorkflowTags.Any())
                {
                    tableData.AppendLine($"<td>");
                    foreach (var tag in workflowNode.WorkflowTags)
                    {
                        var tagUrl = Dragonfly.NetHelpers.Url.AppendQueryStringToUrl(Request.RequestUri, "FilterTag", tag);
                        var tagHtml = $"<a href=\"{tagUrl}\"><span class=\"badge\">{tag}</span></a>";
                        tableData.AppendLine(tagHtml);
                    }
                    tableData.AppendLine($"</td>");
                }
                else
                { tableData.AppendLine($"<td></td>"); }

                if (workflowNode.WorkflowReferences != null && workflowNode.WorkflowReferences.Any())
                {
                    var linksHtml = WorkflowHelper.FormatLinks(workflowNode.WorkflowReferences);
                    tableData.AppendLine($"<td>{linksHtml}</td>");
                }
                else
                { tableData.AppendLine($"<td></td>"); }

                tableData.AppendLine($"<td>{workflowNode.CreateDate}</td>");
                tableData.AppendLine($"<td>{workflowNode.UpdateDate}</td>");
                tableData.AppendLine($"<td>{workflowNode.DocumentTypeAlias}</td>");
                // tableData.AppendLine(string.Format("<td>{0}</td>", workflowNode.Id));

                tableData.AppendLine("</tr>");
            }

            returnSB.AppendLine(tableStart);
            returnSB.Append(tableData);
            returnSB.AppendLine(tableEnd);
            returnSB.AppendLine(HtmlEnd());

            return returnSB;
        }


        #endregion
        
        #region Fancy Formatting & Tables Data

        private string HtmlStart(string CustomStyles = "")
        {
            var pageStart = $@"<!DOCTYPE html>
                <html>
                <head>
                    <link href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" rel=""stylesheet""/>
                    <link href=""https://cdnjs.cloudflare.com/ajax/libs/datatables/1.10.12/css/dataTables.bootstrap.min.css"" rel=""stylesheet""/>

                    <style>
                        {CustomStyles}
                    </style>
                    <!--<script src=""/scripts/snippet-javascript-console.min.js?v=1""></script>-->
                </head>
                <body>                
                    <div class=""container"">
            
               ";

            return pageStart;
        }

        private string HtmlEnd()
        {
            var pageStart = @"
            </div>
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js""></script>
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/datatables/1.10.12/js/jquery.dataTables.min.js""></script>
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/datatables/1.10.12/js/dataTables.bootstrap.min.js""></script>
                <script type=""text/javascript"">
                    $(document).ready(function() {
                      $('#umbracodata').DataTable();
                    });
                </script>
            </body>
            </html>
               ";

            return pageStart;
        }

        private string TdActions(int NodeId, string NodeUrl)
        {
            var tableData = new StringBuilder();

            tableData.AppendLine(string.Format("<td>"));
            tableData.AppendLine(string.Format(
                "<a href=\"/umbraco#/content/content/edit/{0}\" target=\"_blank\">Edit</a>", NodeId));
            if (NodeUrl != "")
            {
                tableData.AppendLine(string.Format(" | <a href=\"{0}\" target=\"_blank\">View</a> ",
                    NodeUrl));
            }

            tableData.AppendLine(string.Format("</td>"));

            return tableData.ToString();
        }

        private string TdTagData(IEnumerable<string> TagData, bool FancyFormat, string TagName)
        {
            var tableData = new StringBuilder();

            if (TagData != null && TagData.Any())
            {
                tableData.AppendLine(string.Format("<td>"));
                if (FancyFormat)
                {
                    foreach (var tag in TagData)
                    {
                        var tagHtml = "";

                        var tagUrl = Dragonfly.NetHelpers.Url.AppendQueryStringToUrl(Request.RequestUri, TagName, tag);
                        tagHtml = $"<a href=\"{tagUrl}\"><span class=\"badge\">{tag}</span></a> ";

                        tableData.AppendLine(tagHtml);
                    }
                }
                else
                {
                    var tagsString = string.Join(", ", TagData);
                    tableData.AppendLine(tagsString);
                }

                tableData.AppendLine(string.Format("</td>"));
            }
            else
            { tableData.AppendLine(string.Format("<td></td>")); }

            return tableData.ToString();
        }

        private string TdDateData(DateTime DateData, bool FancyFormat)
        {
            var tableData = new StringBuilder();

            if (DateData != DateTime.MinValue)
            {
                tableData.AppendLine(string.Format("<td>{0}</td>", DateData));
            }
            else
            { tableData.AppendLine(string.Format("<td></td>")); }

            return tableData.ToString();
        }

        private string TdStringData(string DataText, bool FancyFormat)
        {
            var tableData = new StringBuilder();

            tableData.AppendLine($"<td>{DataText}</td>");

            return tableData.ToString();
        }

        private string TdStringData(int DataText, bool FancyFormat)
        {
            return TdStringData(DataText.ToString(), FancyFormat);
        }

        private string TdStringData(IHtmlString DataText, bool FancyFormat)
        {
            return TdStringData(DataText.ToString(), FancyFormat);
        }

        //private string TdXX(bool FancyFormat)
        //{
        //    var tableData = new StringBuilder();


        //    return tableData.ToString();
        //}

        #endregion

        #region Tests & Examples
        /// /Umbraco/backoffice/Api/WorkflowApi/Test
        [System.Web.Http.AcceptVerbs("GET")]
        public bool Test()
        {
            //LogHelper.Info<WorkflowApi>("Test STARTED/ENDED");
            return true;
        }

        /// /Umbraco/backoffice/Api/WorkflowApi/ExampleReturnHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage ExampleReturnHtml()
        {
            var returnSB = new StringBuilder();

            returnSB.AppendLine("<h1>Hello! This is HTML</h1>");
            returnSB.AppendLine("<p>Use this type of return when you want to exclude &lt;XML&gt;&lt;/XML&gt; tags from your output and don\'t want it to be encoded automatically.</p>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/WorkflowApi/ExampleReturnJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage ExampleReturnJson()
        {
            var returnSB = new StringBuilder();

            var testData = new StatusMessage(true, "This is a test object so you can see JSON!");
            string json = JsonConvert.SerializeObject(testData);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        #endregion
    }
}
