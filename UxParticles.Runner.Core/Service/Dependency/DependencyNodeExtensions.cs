namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;

    public static class DependencyNodeExtensions
    {
        public static IDictionary<Type, DependencyNode> BeginGraph(IStaticJobDependencyMapper mapper)
        {
            var graph = new Dictionary<Type, DependencyNode>();
            var rootNode  = new DependencyNode { Key = mapper.Source };
            rootNode.AddChild(mapper, graph);

            return graph;
        }

        public static void AddChild(this DependencyNode node, IStaticJobDependencyMapper mapper, IDictionary<Type,DependencyNode> nodesByType)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (nodesByType == null)
            {
                throw new ArgumentNullException(nameof(nodesByType));
            }

            DependencyNode childNode = null;
            if (!nodesByType.TryGetValue(mapper.Destination, out childNode))
            {
                childNode = new DependencyNode { Key = mapper.Destination };
                nodesByType.Add(mapper.Destination, childNode);
            }

            node.ChildrenNodes.Add(new DependencyNodeRelationship() { Mapper = mapper, To = childNode });
            childNode.ParentNodes.Add(node);
        }
    }
}