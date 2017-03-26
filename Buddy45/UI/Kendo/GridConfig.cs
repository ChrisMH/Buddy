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
        /// <summary>
        /// The column's data field
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The column title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The column data type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The action that should be taken when clicking on the column
        /// 
        /// Typical values: hyperlink, click
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Width of the column
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Format for column data
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Text alignment for the column values
        /// 
        /// Typical values: left, center, right
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// Aggregate calculation for column data
        /// 
        /// Typical values: count, sum, average
        /// </summary>
        public string Aggregate { get; set; }

        /// <summary>
        /// Header for the aggregate footer
        /// </summary>
        public string FooterHeader { get; set; }
        
        /// <summary>
        /// Whether the column should be shown
        /// </summary>
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
        public readonly string Action;
        public readonly string Align;
        public readonly bool Hidden;

        public GridAttribute(string title = null, 
            string width = null, 
            string format = null, 
            string aggregate = null, 
            string footerHeader = null, 
            string type = null, 
            string action = null,
            string align = null,
            bool hidden = false)
        {
            Title = title;
            Width = width;
            Format = format;
            Aggregate = aggregate;
            FooterHeader = footerHeader;
            Type = type;
            Action = action;
            Align = align;
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
                    Action = attr.Action,
                    Align = attr.Align,
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
