using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Data;
using DynamicForms.LayoutTree;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Templates.Data
{
    [Template(Module = "Core", StorageKey = "Templates", NodePath = "Core/Templating/Templates", NodeName = "Templates")]
    public class DataTemplate : DataObject
    {
        public string Name
        {
            get;
            set;
        }

        public string NodeNameTransformation { get; set; }
        public string DefaultNodePath { get; set; }

        public bool Embedded
        {
            get;
            set;
        }

        public bool Indexable
        {
            get;
            set;
        }

        public bool Versionable
        {
            get;
            set;
        }

        public string DefaultType
        {
            get;
            set;
        }

        public List<ObjectId> ParentTemplates
        {
            get; set;
        }

        public IRelationContainer FieldRelationDefinitions
        {
            get; set;
        }

        public IRelationContainer DataRelationDefinitions
        {
            get; set;
        }

        public IRelationContainer Icon
        {
            get; set;
        }

        public FormDefinition Layout { get; set; }

        [BsonIgnore]
        public List<IEntityContainer<DataRelationDefinition>> FieldRelations { get; set; }

        [BsonIgnore]
        public List<IEntityContainer<DataRelationDefinition>> DataRelations { get; set; }

        [BsonIgnore]
        public List<IEntityContainer<DataTemplateSection>> Sections { get; set; }

    }
}