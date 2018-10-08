namespace Dragonfly.UmbracoWorkflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Dragonfly.Umbraco7Helpers;
    using Dragonfly.UmbracoWorkflow.Models;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Membership;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    public static class WorkflowHelper
    {
        private static UmbracoHelper umbHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
        private static IContentService umbContentService = ApplicationContext.Current.Services.ContentService;
        private static IUserService umbUserService = ApplicationContext.Current.Services.UserService;

        #region Get Nodes
        public static IEnumerable<ICompWorkflow> GetNodesWithWorkflowData(bool ExcludeNonWorkflowDoctypes)
        {
            var nodesList = new List<ICompWorkflow>();
            var topLevelNodes = umbContentService.GetRootContent().OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopNodes(thisNode));
            }

            if (ExcludeNonWorkflowDoctypes)
            {
                var testProp = "WorkflowExcludeFromTracking";
                var workflowNodes = nodesList.Where(n => n.HasProperty(testProp));
                return workflowNodes;
            }
            else
            {
                return nodesList;
            }
        }

        internal static List<ICompWorkflow> LoopNodes(IContent ThisNode)
        {
            var nodesList = new List<ICompWorkflow>();

            //Add current node, then loop for children
            try
            {
                var iPub = ThisNode.ToPublishedContent();
                ICompWorkflow content = new CompWorkflow(iPub);
                nodesList.Add(content);

                if (ThisNode.Children().Any())
                {
                    foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
                    {
                        nodesList.AddRange(LoopNodes(childNode));
                    }
                }
            }
            catch (Exception e)
            {
                //skip
            }

            return nodesList;
        }
        #endregion

        public static IProfile GetUser(object UserId)
        {
            if (UserId != null)
            {
                int userId;
                var isValidId = Int32.TryParse(UserId.ToString(), out userId);
                if (isValidId)
                {
                    var user = umbUserService.GetProfileById(userId);
                    if (user != null)
                    {
                        return user;
                    }
                }
            }

            //else
            return null;
        }

        #region HTML

        //private IHtmlString FormatTagData(IEnumerable<string> TagData, bool FancyFormat, string TagName, Dictionary<string, string> TagClasses)
        //{
        //    var tableData = new StringBuilder();

        //    if (TagData != null && TagData.Any())
        //    {
        //        tableData.AppendLine(string.Format("<td>"));
        //        if (FancyFormat)
        //        {
        //            foreach (var tag in TagData)
        //            {
        //                var tagHtml = "";
        //                var tagClass = "label label-default";

        //                var tagUrl = Dragonfly.NetHelpers.Url.AppendQueryStringToUrl(Request.RequestUri, TagName, tag);

        //                if (TagClasses != null)
        //                {
        //                    var match = TagClasses.ContainsKey(tag) ? TagClasses[tag] : "";
        //                    if (match != "")
        //                    {
        //                        tagClass = $"label label-{match}";
        //                    }
        //                }

        //                tagHtml = $"<a href=\"{tagUrl}\"><span class=\"{tagClass}\">{tag}</span></a> ";

        //                tableData.AppendLine(tagHtml);
        //            }
        //        }
        //        else
        //        {
        //            var tagsString = string.Join(", ", TagData);
        //            tableData.AppendLine(tagsString);
        //        }

        //        tableData.AppendLine(string.Format("</td>"));
        //    }
        //    else
        //    { tableData.AppendLine(string.Format("<td></td>")); }

        //    return tableData.ToString();
        //}

        public static IHtmlString FormatLinks(IEnumerable<RJP.MultiUrlPicker.Models.Link> Links)
        {
            var html = new StringBuilder();

            foreach (var link in Links)
            {
                html.AppendFormat("<a href=\"{0}\" target=\"{1}\">{2}</a>", link.Url, "_blank", link.Name);

                if (Links.IndexOf(link) < Links.Count())
                {
                    html.Append("<br/>");
                }
            }

            return new HtmlString(html.ToString());
        }

        public static IHtmlString FormatDragonflyLinks(IEnumerable<Dragonfly.UmbracoModels.Link> Links)
        {
            var html = new StringBuilder();

            foreach (var link in Links)
            {
                html.AppendFormat("<a href=\"{0}\" target=\"{1}\">{2}</a>", link.Url, "_blank", link.Title);

                if (Links.IndexOf(link) < Links.Count())
                {
                    html.Append("<br/>");
                }
            }

            return new HtmlString(html.ToString());
        }
        #endregion

    }
}
