using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenSol.UI.HtmlHelpers
{
    public static class AutoCompleteHelper
    {
        public static MvcHtmlString AutoCompleteFor<TModel,TValue>(this HtmlHelper html, Expression<Func<TModel, TValue>> expression, string source, string value)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder tag = new TagBuilder("input");
            Func<TModel, TValue> method = expression.Compile();
            tag.MergeAttribute("data-autocomplete-source", source);
            tag.MergeAttribute("value", value);  
          
            result.Append(tag.ToString());
            
            return MvcHtmlString.Create(result.ToString());
        }
    }
}