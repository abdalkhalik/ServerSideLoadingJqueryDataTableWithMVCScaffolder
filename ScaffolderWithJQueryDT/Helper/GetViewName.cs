using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ScaffolderWithJQueryDT.Helper
{
    public static class GetViewName
    {
        public static string GetViewTitle(string con, string act)
        {
            try
            {
                return act;
            }
            catch (Exception e)
            {
                return "None";
            }
        }

        public static string ToLowercaseNamingConvention(this string s, bool toLowercase)
        {
            if (toLowercase)
            {
                var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                return r.Replace(s, " ");
            }
            else
                return s;
        }
    }
}