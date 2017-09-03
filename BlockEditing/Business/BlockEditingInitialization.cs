using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Core.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace BlockEditing.Business
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class BlockEditingInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Intercept<ContentFragmentFactory>(
                (locator, fragmentParser) => new CustomContentFragmentFactory());
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }

    public class CustomContentFragmentFactory : ContentFragmentFactory
    {
        private readonly IContentRepository _contentRepository;
        private readonly DisplayOptions _displayOptions;
        private readonly IPublishedStateAssessor _publishedStateAssessor;
        private readonly IContentAccessEvaluator _contentAccessEvaluator;
        private readonly IContextModeResolver _contextModeResolver;

        public CustomContentFragmentFactory() : this(
            ServiceLocator.Current.GetInstance<IContentRepository>(),
            ServiceLocator.Current.GetInstance<DisplayOptions>(),
            ServiceLocator.Current.GetInstance<IPublishedStateAssessor>(),
            ServiceLocator.Current.GetInstance<IContentAccessEvaluator>(),
            ServiceLocator.Current.GetInstance<IPermanentLinkMapper>(),
            ServiceLocator.Current.GetInstance<IContextModeResolver>())
        {
        }

        public CustomContentFragmentFactory(IContentRepository contentRepository, DisplayOptions displayOptions,
            IPublishedStateAssessor publishedStateAssessor, IContentAccessEvaluator contentAccessEvaluator,
            IPermanentLinkMapper permanentLinkMapper, IContextModeResolver contextModeResolver) : base(
            contentRepository, displayOptions, publishedStateAssessor, contentAccessEvaluator, permanentLinkMapper,
            contextModeResolver)
        {
            _contentRepository = contentRepository;
            _displayOptions = displayOptions;
            _publishedStateAssessor = publishedStateAssessor;
            _contentAccessEvaluator = contentAccessEvaluator;
            _contextModeResolver = contextModeResolver;
        }

        public override ContentFragment CreateContentFragment(ContentReference contentLink, Guid contentGuid, string tag,
            ISecuredFragmentMarkupGenerator securedFragmentMarkupGenerator, IDictionary<string, object> attributes)
        {
            var contentFragment = base.CreateContentFragment(contentLink, contentGuid, tag,
                securedFragmentMarkupGenerator, attributes);

            return new CustomContentFragment((IContentLoader) this._contentRepository, securedFragmentMarkupGenerator,
                this._displayOptions, this._publishedStateAssessor, this._contextModeResolver,
                this._contentAccessEvaluator, attributes)
            {
                ContentGuid = contentFragment.ContentGuid,
                ContentLink = contentFragment.ContentLink,
                Tag = tag
            };
        }
    }

    public class CustomContentFragment : ContentFragment, IStringFragment
    {
        public CustomContentFragment(IContentLoader contentLoader,
            ISecuredFragmentMarkupGenerator securedFragmentMarkupGenerator, DisplayOptions displayOptions,
            IPublishedStateAssessor publishedStateAssessor, IContextModeResolver contextModeResolver,
            IContentAccessEvaluator contentAccessEvaluator, IDictionary<string, object> attributes) :
            base(
                contentLoader, securedFragmentMarkupGenerator, displayOptions, publishedStateAssessor,
                contextModeResolver, contentAccessEvaluator, attributes)
        {
        }

        public CustomContentFragment(IContentLoader contentLoader, IContextModeResolver contextModeResolver) :
           base(contentLoader, null, null, null, contextModeResolver, null, null)
        {
        }

        public new string GetEditFormat()
        {
            var editFormat = base.GetEditFormat();
            editFormat = editFormat.Replace("</div>", "<a data-type=\"context-link\" data-contentlink=\"" + this.ContentLink.ToReferenceWithoutVersion() + "\">Edit</a></div>");
            return editFormat;
        }
    }
}