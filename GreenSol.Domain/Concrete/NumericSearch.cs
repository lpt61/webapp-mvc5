using GreenSol.Domain.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace GreenSol.Domain.Concrete
{
    public class NumericSearch : AbstractSearch
    {
        public int? SearchTerm { get; set; }

        public NumericComparators Comparator { get; set; }

        protected override Expression BuildExpression(MemberExpression property)
        {
            if (!this.SearchTerm.HasValue)
            {
                return null;
            }

            Expression searchExpression = this.GetFilterExpression(property);

            return searchExpression;
        }
        
        private Expression GetFilterExpression(MemberExpression property)
        {
            var rightOperand = Expression.Constant(this.SearchTerm.Value);
            var leftOperand = property;

            switch (this.Comparator)
            {                
                //case NumericComparators.Less:
                //    return Expression.LessThan(property, Expression.Constant(this.SearchTerm.Value));
                
                //If the SearchTerm.Value = 8,
                //the compiler will try to convert the rightOperand to int which makes the expression failed to excute,
                //so I convert it explicitly to the type of the leftOperand (decimal, in this case)
                case NumericComparators.LessOrEqual:
                    return Expression. LessThanOrEqual(leftOperand, Expression.Convert(rightOperand, typeof(decimal)));
                //case NumericComparators.Equal:
                //    return Expression.Equal(property, Expression.Constant(this.SearchTerm.Value));
                case NumericComparators.GreaterOrEqual:
                    return Expression.GreaterThanOrEqual(leftOperand, Expression.Convert(rightOperand, typeof(decimal)));
                //case NumericComparators.Greater:
                //    return Expression.GreaterThan(property, Expression.Constant(this.SearchTerm.Value));
                default:
                    throw new InvalidOperationException("Comparator not supported.");
            }
        }
    }

    public enum NumericComparators
    {
        //[Display(Name = "<")]
        //Less,

        //[Display(Name = "<=")]
        LessOrEqual,

        //[Display(Name = "==")]
        //Equal,

        //[Display(Name = ">=")]
        GreaterOrEqual,

        //[Display(Name = ">")]
        //Greater
    }
}
