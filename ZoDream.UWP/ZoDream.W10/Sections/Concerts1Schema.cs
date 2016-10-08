using System;
using AppStudio.DataProviders;

namespace ZoDream.Sections
{
    /// <summary>
    /// Implementation of the Concerts1Schema class.
    /// </summary>
    public class Concerts1Schema : SchemaBase
    {

        public string City { get; set; }

        public string Room { get; set; }

        public DateTime? Time { get; set; }

        public string MonthText { get; set; }
    }
}
