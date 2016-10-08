using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppStudio.DataProviders;
using ZoDream.Sections;

namespace ZoDream.ViewModels
{
    class ViewModelFactory
    {
        public static ListViewModel NewList<TSchema>(Section<TSchema> section, int visibleItems = 0) where TSchema : SchemaBase
        {
            return new ListViewModel<TSchema>(section, visibleItems);
        }

        public static DetailViewModel NewDetail<TSchema>(Section<TSchema> section) where TSchema : SchemaBase
        {
            return new DetailViewModel<TSchema>(section);
        }

        public static DetailViewModel NewDetail<TSchema, TRelatedSchema>(Section<TSchema> section, Section<TRelatedSchema> relatedSection) where TSchema : SchemaBase where TRelatedSchema : SchemaBase
        {
            return new DetailRelatedViewModel<TSchema>(section, NewList(relatedSection));
        }

        public static GroupedListViewModel NewListGrouped<TSchema>(Section<TSchema> section) where TSchema : SchemaBase
        {
            return new GroupedListViewModel<TSchema>(section);
        }
    }
}
