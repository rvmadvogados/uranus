using System;
using System.Reflection;
using System.Web.Mvc;

namespace Uranus.Suite.App_Code
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString AssemblyVersion(this HtmlHelper helper)
        {
            var version = String.Format("Versão {0}.{1}.{2}", typeof(Uranus.Suite.MvcApplication).Assembly.GetName().Version.Major.ToString(), typeof(Uranus.Suite.MvcApplication).Assembly.GetName().Version.Minor.ToString(), typeof(Uranus.Suite.MvcApplication).Assembly.GetName().Version.Build.ToString());
            return MvcHtmlString.Create(version);
        }

        public static MvcHtmlString AssemblyCopyright(this HtmlHelper helper)
        {
            var copyright = String.Format("{0}. Todos os direitos reservados.", ((AssemblyCopyrightAttribute)typeof(Uranus.Suite.MvcApplication).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright);
            return MvcHtmlString.Create(copyright);
        }

        public static MvcHtmlString ActionImageLink(this HtmlHelper helper, string name, string action, string routeValues, string imagePath, object attributes)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            // build the <img> tag
            var imageBuilder = new TagBuilder("img");
            imageBuilder.GenerateId(name);
            imageBuilder.MergeAttribute("src", url.Content(imagePath));
            string imageHtml = imageBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imageHtml; // include the <img> tag inside
            anchorBuilder.AddCssClass(attributes.ToString());
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper helper, string name, string imagePath, object attributes)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            // build the <img> tag
            var imageBuilder = new TagBuilder("img");
            imageBuilder.GenerateId(name);
            imageBuilder.MergeAttribute("src", url.Content(imagePath));
            imageBuilder.AddCssClass(attributes.ToString());
            string imageHtml = imageBuilder.ToString(TagRenderMode.SelfClosing);

            return MvcHtmlString.Create(imageHtml);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper helper, string name, string action, string routeValues, string text, string style, object attributes)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.GenerateId(name);
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.MergeAttribute("style", style);
            anchorBuilder.AddCssClass(attributes.ToString());
            anchorBuilder.SetInnerText(text);
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }
}