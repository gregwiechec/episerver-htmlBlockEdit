using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using EPiServer.Web;

namespace BlockEditing.Business
{
    [EditorDescriptorRegistration(TargetType = typeof(XhtmlString), EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public class CustomXhtmlStringEditorDescriptor : XhtmlStringEditorDescriptor
    {
        public override void ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            this.EditorConfiguration["blockTemplate"] = GetBlockEditTemplate();

            this.ClientEditingClass = "alloy/editors/xhtml/TinyMCEEditor";
            base.ModifyMetadata(metadata, attributes);
            metadata.CustomEditorSettings["uiType"] = "alloy/editors/xhtml/TinyMCEInlineEditor";
        }

        private string GetBlockEditTemplate()
        {
            var contentFragment = new CustomContentFragment(ServiceLocator.Current.GetInstance<IContentLoader>(),
                    ServiceLocator.Current.GetInstance<IContextModeResolver>())
                {ContentLink = ContentReference.RootPage};
            var editFormat = contentFragment.GetEditFormat();
            editFormat = editFormat.Replace("data-contentlink=\"1\"", "data-contentlink=\"{contentLink}\"");
            editFormat = editFormat.Replace("data-contentname=\"Root\"", "data-contentname=\"{name}\"");
            editFormat = editFormat.Replace(">Root<", ">{name}<");
            return editFormat;
        }
    }
}