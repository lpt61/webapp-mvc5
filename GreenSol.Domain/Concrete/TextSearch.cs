using GreenSol.Domain.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace GreenSol.Domain.Concrete
{
    public class TextSearch : AbstractSearch
    {
        public string SearchTerm { get; set; }

        //public TextComparators Comparator { get; set; }

        //The default comparator is 'Contains' 
        public TextComparators Comparator = TextComparators.Contains;


        protected override Expression BuildExpression(MemberExpression property)
         {
            if (this.SearchTerm == null)
            {
                return null;
            }

            //For ex: Seach album Name 
            //I want to search with case insentivity, so the final expression must be:
            //albums.Name.ToLower().Contains(searchString.ToLower())

            //Separate calls
            //1. Build expression: albums.Name.Tolower()
            /*
            var searchExpression1 = Expression.Call(
                property,
                typeof(string).GetMethod("ToLower", System.Type.EmptyTypes)                    
            );
            //2.Build expression: Contains(searchString.ToLower())) then append it to the first expression
            var finalExpression = Expression.Call(
                searchExpression1,

                typeof(string).GetMethod(this.Comparator.ToString(),  new[] { typeof(string) }), 
                Expression.Constant(this.SearchTerm.ToLower())
            );
            */

            //Merged call
            var searchExpression3 = Expression.Call(
                Expression.Call(
                    property,
                    typeof(string).GetMethod("ToLower", System.Type.EmptyTypes)),

                typeof(string).GetMethod(this.Comparator.ToString(), new[] { typeof(string) }),
                Expression.Constant(this.SearchTerm.ToLower())
            );

            return searchExpression3;
        }
    }

    public enum TextComparators
    {
        [Display(Name = "Contains")]
        Contains,

        //[Display(Name = "==")]
        //Equals
    }
}
