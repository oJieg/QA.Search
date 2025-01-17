﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QA.Search.Generic.DAL.Services;
using QA.Search.Generic.DAL.Services.Configuration;
using QA.Search.Generic.DAL.Services.Interfaces;
using QA.Search.Generic.Integration.Core.Models;
using QA.Search.Generic.Integration.Core.Models.DTO;
using QA.Search.Generic.Integration.QP.Infrastructure;
using QA.Search.Integration.Demosite.Services;

namespace QA.Search.Integration.Demosite.Views
{
    public class NewsPostView : ElasticView<GenericDataContext>
    {
        private readonly IUrlService<DemositeDataContext> _urlService;

        public override string IndexName => nameof(DemositeDataContext.NewsPosts);

        public override string ViewName => nameof(NewsPostView);

        public NewsPostView(DemositeDataContext context, ILogger logger,
            IOptions<ContextConfiguration> contextConfigurationOption,
            IOptions<GenericIndexSettings> genericIndexSettingsOption,
            IUrlService<DemositeDataContext> urlService)
            : base(context, logger, contextConfigurationOption, genericIndexSettingsOption)
        {
            _urlService = urlService;
        }

        protected override IQueryable Query => ((DemositeDataContext)Db).NewsPosts;

        public override async Task<JObject[]> LoadAsync(LoadParameters loadParameters, CancellationToken token)
        {
            JObject[] documents = await base.LoadAsync(loadParameters, token);

            foreach (JObject document in documents)
            {
                int? catrgoryId = document["category"]?.Value<int>();

                if (catrgoryId == null)
                {
                    throw new ArgumentNullException(nameof(catrgoryId), "News with empty category. Fix that in QP and restart indexing.");
                }

                int abstractItemId = await ((DemositeDataContext)Db).NewsCategories
                    .Where(x => x.ContentItemID == catrgoryId)
                    .Select(x => x.ItemId)
                    .SingleAsync(token);

                string url = await _urlService.GetUrlToPageByIdAsync(abstractItemId, token);

                document[genericIndexSettings.SearchUrlField] = _urlService.UrlToJArray(
                        genericIndexSettings.SearchUrlField,
                        $"{url}/details/{document["contentitemid"]?.Value<int>()}");
            }

            return documents;
        }
    }
}
