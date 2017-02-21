namespace Buddy.Web.TabularQuery
{
    /// <summary>
    /// Grouping part of a tabular query
    /// The query string part looks like: group[0].field=field0group[0].dir=ascgroup[1].field=field1group[1].dir=desc ...
    /// Supported dir values:
    ///    asc
    ///    desc
    /// </summary>
    public class GroupExpression
    {
        public string Field { get; set; }
        public string Dir { get; set; }

        /*
         * [{
  aggregates: {
    FIEL1DNAME: {
      FUNCTON1NAME: FUNCTION1VALUE,
      FUNCTON2NAME: FUNCTION2VALUE
    },
    FIELD2NAME: {
      FUNCTON1NAME: FUNCTION1VALUE
    }
  },
  field: FIELDNAME, // the field by which the data items are grouped
  hasSubgroups: true, // true if there are subgroups
  items: [
    // either the subgroups or the data items
    {
      aggregates: {
        //nested group aggregates
      },
      field: NESTEDGROUPFIELDNAME,
      hasSubgroups: false,
      items: [
      // data records
      ],
      value: NESTEDGROUPVALUE
    },
    //nestedgroup2, nestedgroup3, etc.
  ],
  value: VALUE // the group key
} /* other groups */

    }
}