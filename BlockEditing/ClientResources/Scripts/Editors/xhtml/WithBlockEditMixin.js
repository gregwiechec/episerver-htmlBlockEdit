define([
        "dojo/_base/declare",
        "dojo/_base/lang",
        "dojo/topic",
        "dojo/when",
        "dojo/Deferred",
    "epi/shell/TypeDescriptorManager"
    ],
    function (
        declare,
        lang,
        topic,
        when,
        Deferred,
        TypeDescriptorManager
    ) {
        return declare([], {
            extendTinyMCE: function (tinyMCESettings) {
                tinyMCESettings.setup = function (ed, e) {
                    ed.onClick.add(function (ed, e) {
                        if (!e.target.nodeName || e.target.getAttribute("data-type") !== "context-link") {
                            return;
                        }

                        var contentLink = e.target.getAttribute("data-contentlink");
                        topic.publish("/epi/shell/context/request", {
                            uri: "epi.cms.contentdata:///" + contentLink
                        }, {});
                    });
                }
            },
            
            postMixInProperties: function () {

                this.inherited(arguments);

                this._originalDropDataProcessor = this._dropDataProcessor;
                this._dropDataProcessor = this._customDropDataProcessor;

            },

            _customDropDataProcessor: function (dropItem) {
                when(this._tryBlockDropDataProcessor(dropItem)).then(function (result) {
                    if (!result) {
                        this._originalDropDataProcessor.call(this, dropItem);
                    }
                }.bind(this));
            },

             _tryBlockDropDataProcessor: function (dropItem) {
                 var deferred = new Deferred();

                 when(dropItem.data, function (model) {
                     var self = this,
                         ed = this.getEditor();

                     function insertHtml(html) {
                         ed.focus();
                         if (ed.execCommand("mceInsertContent", false, html)) {
                             self._onChange(ed.getContent());
                         }
                     }

                     var typeId = model.typeIdentifier;

                     var editorDropBehaviour = TypeDescriptorManager.getValue(typeId, "editorDropBehaviour");

                     if (editorDropBehaviour) {

                         if (editorDropBehaviour === 1) {
                             var blockTemplate = lang.replace(this.params.blockTemplate, {name: model.name, contentLink: dropItem.data.contentLink  });
                             insertHtml(blockTemplate);
                             deferred.resolve(true);
                             return;
                         }
                     }

                     deferred.resolve(false);
                 }.bind(this));

                 return deferred.promise;
             }
        });
    });