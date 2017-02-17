using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ScaffolderWithJQueryDT.Helper
{
    public enum CompareMethod
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    /// <summary>
    /// This class represents one part of a complex where statement
    /// </summary>
    public class WhereClausePart
    {
        public WhereClausePart()
        { }

        public WhereClausePart(string propertyName, CompareMethod compareMethod, object compareValue)
        {
            PropertyName = propertyName;
            CompareMethod = compareMethod;
            CompareValue = compareValue;
        }

        public string PropertyName { get; set; }
        public object CompareValue { get; set; }
        public CompareMethod CompareMethod { get; set; }
    }

    public static class WhereClauseCreator
    {
        private static readonly MethodInfo containsMethod;
        private static readonly MethodInfo endsWithMethod;
        private static readonly MethodInfo startsWithMethod;
        private static readonly MethodInfo equalWithMethod;

        static WhereClauseCreator()
        {
            containsMethod = typeof(WhereClauseCreator).GetMethod("Contains", BindingFlags.NonPublic | BindingFlags.Static);
            endsWithMethod = typeof(WhereClauseCreator).GetMethod("EndsWith", BindingFlags.NonPublic | BindingFlags.Static);
            startsWithMethod = typeof(WhereClauseCreator).GetMethod("StartsWith", BindingFlags.NonPublic | BindingFlags.Static);
            equalWithMethod = typeof(WhereClauseCreator).GetMethod("StartsWith", BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Creates a Func(T,bool) that can be used as part of a Where statement in Linq
        /// The comparission statements will be linked using an or operator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static Func<T, bool> CreateOrWhereClause<T>(IEnumerable<WhereClausePart> whereClause)
        {
            ParameterExpression inParameter = Expression.Parameter(typeof(T));
            Expression whereStatement = null;

            // foreach part of the where clause create the compare expression then or it with the current where statement
            foreach (WhereClausePart part in whereClause)
            {
                Expression propertyEqualStatement = CompareExpression<T>(inParameter, part);
                if (whereStatement == null)
                {
                    whereStatement = propertyEqualStatement;
                }
                else
                {
                    whereStatement = Expression.OrElse(whereStatement, propertyEqualStatement);
                }
            }

            if (whereStatement == null)
            {
                whereStatement = Expression.Constant(true);
            }
            return Expression.Lambda<Func<T, bool>>(whereStatement, inParameter).Compile();
        }

        public static Func<T, bool> CreateAndWhereClause<T>(IEnumerable<WhereClausePart> whereClause)
        {
            ParameterExpression inParameter = Expression.Parameter(typeof(T));
            Expression whereStatement = null;

            foreach (WhereClausePart part in whereClause)
            {
                Expression propertyEqualStatement = CompareExpression<T>(inParameter, part);
                if (whereStatement == null)
                {
                    whereStatement = propertyEqualStatement;
                }
                else
                {
                    whereStatement = Expression.AndAlso(whereStatement, propertyEqualStatement);
                }
            }
            if (whereStatement == null)
            {
                whereStatement = Expression.Constant(true);
            }
            return Expression.Lambda<Func<T, bool>>(whereStatement, inParameter).Compile();
        }

        /// <summary>
        /// Creates an expression where it compares a property to a particular value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inParameter"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private static Expression CompareExpression<T>(ParameterExpression inParameter, WhereClausePart part)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(part.PropertyName);
            // based on the comparission method we change the Expression we return back
            switch (part.CompareMethod)
            {
                case CompareMethod.Equal:
                    return Expression.Equal(Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.GreaterThan:
                    return Expression.GreaterThan(Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.LessThan:
                    return Expression.LessThan(Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.LessThanOrEqual:
                    return Expression.LessThanOrEqual(Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.Contains:
                    return Expression.Call(containsMethod,
                        Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.StartsWith:
                    return Expression.Call(startsWithMethod,
                    Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));
                case CompareMethod.EndsWith:
                    return Expression.Call(endsWithMethod,
                        Expression.Property(inParameter, propertyInfo),
                        Expression.Constant(part.CompareValue));

            }
            return null;
        }

        /// <summary>
        /// I've created a wrapper around Contains so that I don't need to check for null in the linq expression 
        /// </summary>
        /// <param name="testString"></param>
        /// <param name="containsValue"></param>
        /// <returns></returns>
        private static bool Contains(string testString, string containsValue)
        {
            if (testString != null)
            {
                return testString.Contains(containsValue);
            }
            return false;
        }


        /// <summary>
        /// I've created a wrapper around EndsWith so that I don't need to check for null in the linq expression 
        /// </summary>
        /// <param name="testString"></param>
        /// <param name="endsWithValue"></param>
        /// <returns></returns>
        private static bool EndsWith(string testString, string endsWithValue)
        {
            if (testString != null)
            {
                return testString.EndsWith(endsWithValue);
            }
            return false;
        }

        /// <summary>
        /// I've created a wrapper around StartsWith so that I don't need to check for null in the linq expression 
        /// </summary>
        /// <param name="testString"></param>
        /// <param name="startsWithValue"></param>
        /// <returns></returns>
        private static bool StartsWith(string testString, string startsWithValue)
        {
            if (testString != null)
            {
                return testString.StartsWith(startsWithValue);
            }

            return false;
        }
    }

    public static class TypeHelper
    {
        public static object GetPropertyValue(object obj, string name)
        {
            return obj == null ? null : obj.GetType()
                                           .GetProperty(name)
                                           .GetValue(obj, null);
        }
    }

    public class GridViewModel<T>
    {
        public List<T> productList { get; set; }
        public int recordsTotal { get; set; }
    }
}