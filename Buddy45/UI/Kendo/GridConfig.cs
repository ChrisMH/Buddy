using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Buddy.Utility;

namespace Buddy.UI.Kendo
{
    public class GridConfig
    {
        public List<GridColumn> Columns { get; set; }
    }

    public class GridColumn
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Width { get; set; }
        public string Format { get; set; }
        public string Aggregate { get; set; }
        public string FooterHeader { get; set; }
        public bool Hidden { get; set; }
    }

    public class GridAttribute : Attribute
    {
        public readonly string Title;
        public readonly string Width;
        public readonly string Format;
        public readonly string Aggregate;
        public readonly string FooterHeader;
        public readonly string Type;
        public readonly bool Hidden;

        public GridAttribute(string title, string width, string format = null, 
            string aggregate = null, string footerHeader = null, string type = null, bool hidden = false)
        {
            Title = title;
            Width = width;
            Format = format;
            Aggregate = aggregate;
            FooterHeader = footerHeader;
            Type = type;
            Hidden = hidden;
        }
    }

    public static class JsGridExtensions
    {
        public static List<GridColumn> GetColumns(this IEnumerable<PropertyInfo> properties)
        {
            var result = new List<GridColumn>();

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttributes(true).FirstOrDefault(p => p is GridAttribute) as GridAttribute;
                if(attr == null)
                    continue;

                result.Add(new GridColumn
                {
                    Title = attr.Title,
                    Width = attr.Width,
                    Format = attr.Format,
                    Aggregate = attr.Aggregate,
                    FooterHeader = attr.FooterHeader,
                    Field = property.Name.ToLowerCamelCase(),
                    Type = string.IsNullOrWhiteSpace(attr.Type) ? GetJsType(property) : attr.Type,
                    Hidden = attr.Hidden
                });
            }

            return result;
        }

        public static string GetJsType(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
                return "string";
            
            if(property.PropertyType == typeof(bool))
                return "boolean";

            if (property.PropertyType == typeof(DateTime))
                return "date";


            return "number";
        }
    }


}
