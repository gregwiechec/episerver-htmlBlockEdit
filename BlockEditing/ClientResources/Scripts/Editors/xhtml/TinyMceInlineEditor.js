define([
        "dojo/_base/declare",
        "dojo/when",
        "epi-cms/contentediting/editors/TinyMCEInlineEditor",
        "./WithBlockEditMixin"
    ],
    function (
        declare,
        when,
        TinyMCEInlineEditor,
        WithBlockEditMixin
    ) {

        return declare([TinyMCEInlineEditor, WithBlockEditMixin], {
            startup: function () {
                if (this._started) {
                    return;
                }

                this.extendTinyMCE(this.settings);
                this.inherited(arguments);
            }
        });
    });