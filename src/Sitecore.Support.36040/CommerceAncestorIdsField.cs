using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Engine.Connect.DataProvider;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Search.ComputedFields;
using Sitecore.ContentSearch;
using Sitecore.Diagnostics;
using Sitecore.Data;

namespace Sitecore.Support.Commerce.Engine.Connect.Search.ComputedFields
{
    /// <summary>
    /// Computed field that calculates a commerce item's ancestor IDs.
    /// </summary>
    public class CommerceAncestorIdsField : BaseCommerceComputedField
    {
        /// <summary>
        /// The list of valid tempaltes that this applies to
        /// </summary>
        private static readonly IEnumerable<ID> _validTemplates = new List<ID>
        {
            CommerceConstants.KnownTemplateIds.CommerceProductTemplate,
            CommerceConstants.KnownTemplateIds.CommerceCategoryTemplate,
        };

        /// <summary>
        /// Gets the list of valid templates for this computed value
        /// </summary>
        protected override IEnumerable<ID> ValidTemplates
        {
            get
            {
                return _validTemplates;
            }
        }

        /// <summary>
        /// Computed the value for the specfic indexable
        /// </summary>
        /// <param name="indexable">The indexable item</param>
        /// <returns>The computed value</returns>
        public override object ComputeValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, nameof(indexable));
            var computedValue = new List<ID>();

            var validatedItem = GetValidatedItem(indexable);
            if (validatedItem != null)
            {
                var repository = new CatalogRepository();
                var entity = repository.GetEntity(validatedItem.ID.Guid.ToString());
                if (entity != null)
                {
                    var parentCategoryString = entity["ParentCategoryList"].Value<string>();
                    if (!string.IsNullOrWhiteSpace(parentCategoryString))
                    {
                        var parentCategoryList = parentCategoryString.Split('|');
                        computedValue.AddRange(parentCategoryList.Select(id => ID.Parse(id)));
                    }
                }
            }

            return computedValue;
        }
    }
}