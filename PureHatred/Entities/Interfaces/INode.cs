using System.Collections.Generic;

namespace PureHatred.Entities.Interfaces
{
	public interface INode
	{
        List<INode> children { get; set; }
		public bool isNodeExpanded { get; set; }
		public INode parent { get; set; }
		public Actor owner { get; set; }

        INode GetAncestor(int generations) // 0 returns self
        {
            INode ancestor = this;

            for (int i = 0; i < generations; i++)
                ancestor = ancestor.parent;

            return ancestor;
        }

        List<INode> GetAncestors()
        {
            List<INode> ancestry = new List<INode>();
            INode context = this;

            while (context.parent != null)
            {
                ancestry.Add(context.parent);
                context = context.parent;
            }

            return ancestry;
        }

        List<INode> GetDescendants()
        {
            List<INode> descendants = new List<INode>();

            // TODO

            return descendants;
        }

        bool IsNodeVisible() // TODO: Can Linq a oneliner here
        {
            foreach (INode ancestor in GetAncestors())
                if (!ancestor.isNodeExpanded)
                    return false;
            return true;
        }

        Actor GetOwner() =>
            (Actor)GetAncestor(CountParents() + 1);

        bool IsLastborn() =>
            parent == null || this == parent.children[^1];

        int CountParents() =>
            (parent != null) ? 1 + parent.CountParents() : 0;

		// TODO: The above three should only be recalculated when the Item is grafted/removed, etc.
	}
}
