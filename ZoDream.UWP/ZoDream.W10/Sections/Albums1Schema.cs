using System;
using AppStudio.DataProviders;

namespace ZoDream.Sections
{
    /// <summary>
    /// Implementation of the Albums1Schema class.
    /// </summary>
    public class Albums1Schema : SchemaBase
    {

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string TrackList { get; set; }

        public string Year { get; set; }
    }
}
