using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Nodes;
using MongoDB.Bson;
using System;
using System.Linq;

namespace Crolow.Cms.Server.Core.Extensions
{
    public static class NodeDefinitionExtension
    {
        public static NodeDefinition CreateNode(IDataObject o, string key, string name)
        {
            return new NodeDefinition
            {
                Id = o.Id,
                Key = key,
                DisplayName = name ?? key,
                EditState = EditState.New
            };
        }

        public static void SetParent(this INodeDefinition node, INodeDefinition parentNode)
        {
            if (parentNode != null)
            {
                node.Parents = Array.Empty<ObjectId>();
                if (parentNode.Parent != ObjectId.Empty)
                {
                    node.Parents = parentNode.Parents;
                }
                node.Parents = node.Parents.Append(parentNode.Id).ToArray();
            }
        }
    }
}