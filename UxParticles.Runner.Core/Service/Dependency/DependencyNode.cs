namespace UxParticles.Runner.Core.Service.Dependency
{
    using System;
    using System.Collections.Generic;

    public class DependencyNode
    {
        public Type Key { get; set; }

        public ICollection<DependencyNode> ParentNodes { get; set; } = new List<DependencyNode>();

        public ICollection<DependencyNodeRelationship> ChildrenNodes { get; set; } = new List<DependencyNodeRelationship>();
    }

    public class DependencyGraphs
    {
        public ICollection<DependencyNode> RootNodes { get; set; }        
    }
}