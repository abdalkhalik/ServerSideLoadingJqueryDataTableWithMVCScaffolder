using ScaffolderWithJQueryDT.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ScaffolderWithJqueryDT.Helper
{
    public class DataItem
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string DoB { get; set; }
    }

    public class DataTableData<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }

    public static class DataTableModels<T>
    {
        public static List<T> _data;

        private static int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
        }

        private static int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private static int SortDateTime(string s1, string s2, string sortDirection)
        {
            DateTime d1 = DateTime.Parse(s1);
            DateTime d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        // here we simulate SQL search, sorting and paging operations
        // !!!! DO NOT DO THIS IN REAL APPLICATION !!!!
        public static List<T> FilterData(ref int recordFiltered, int start, int length, string search, int sortColumn, string sortDirection)
        {
            List<T> list = _data;
            List<WhereClausePart> whereParts = new List<WhereClausePart>();

            //if(sortColumn <= 0)
            //{
            //    sortColumn += 1;
            //}

            if (!string.IsNullOrEmpty(search))
            {
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo p in properties)
                {
                    //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));
                    Type t = p.PropertyType; // t will be System.String

                    TypeConverter typeConverter = TypeDescriptor.GetConverter(t);
                    //object propValue = typeConverter.ConvertFromString(myArrayValues[i]);
                    CompareMethod MyStatus;
                    if (t.Name == "String")
                    {
                        MyStatus = (CompareMethod)Enum.Parse(typeof(CompareMethod), "Contains", true);
                    }
                    else
                    {
                        //     MyStatus = (CompareMethod)Enum.Parse(typeof(CompareMethod), "Equal", true);
                        continue;
                    }

                    whereParts.Add(new WhereClausePart(p.Name, MyStatus, search));
                }

                Func<T, bool> clause = WhereClauseCreator.CreateOrWhereClause<T>(whereParts);
                //    clauses.Add(clause);
                list = list.Where(clause).ToList();

            }

            if (!String.IsNullOrEmpty(sortDirection))
            {
                Type type = typeof(T);

                PropertyInfo[] properties = type.GetProperties();
                PropertyInfo p = properties[sortColumn];
                {
                    Type t = p.PropertyType; // t will be System.String

                    if (sortDirection == "asc")
                    {
                        list = list.OrderBy(x => TypeHelper.GetPropertyValue(x, p.Name)).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => TypeHelper.GetPropertyValue(x, p.Name)).ToList();
                    }
                }
            }


            recordFiltered = list.Count;
            // get just one page of data
            list = list.GetRange(start, Math.Min(length, list.Count - start));
            return list;
        }
    }
}