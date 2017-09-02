define([
        "dojo/_base/declare",
        "dojo/when",
        "epi-cms/contentediting/editors/TinyMCEEditor",
        "./WithBlockEditMixin"
    ],
    function (
        declare,
        when,
        TinyMCEEditor,
        WithBlockEditMixin
    ) {

        return declare([TinyMCEEditor, WithBlockEditMixin], {

            startup: function () {
                if (this._started) {
                    return;
                }

                this.extendTinyMCE(this.settings);
                this.inherited(arguments);
            }
        });
    });