using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Data;

namespace Crolow.Cms.Server.Core.Models.Templates.Data
{
    [Template(Module = "Core", StorageKey = "RelationDefinitions", NodePath = "Core/Templating/Relations", NodeName = "Relation definitions")]
    public class DataRelationDefinition : DataObject
    {

        public bool DefaultRelation
        {
            get;
            set;
        }

        public string FieldName { get; set; }


        public DataRelationType RelationType
        {
            get;
            set;
        }


        public string RelationName
        {
            get;
            set;
        }


        public DataRelationTarget RelationTarget
        {
            get;
            set;
        }


        public DataRelationCount RelationCount
        {
            get;
            set;
        }


        public string Controller
        {
            get;
            set;
        }



        public IRelationContainer AllowedTargetTemplates
        {
            get; set;
        }


        public IRelationContainer AllowedSourceTemplates
        {
            get; set;
        }



    }

}
