using SadConsole.Controls;
using SadConsole;
using SadConsole.Themes;
using System.Runtime.CompilerServices;

/* Edit < > endcaps: http://sadconsole.com/api/SadConsole.Themes.ButtonTheme.html
 * We can use EndCharacterLeft as the Tree node
 * 
 * 
 */

namespace PureHatred.UI
{
	public class TreeWindowTier : ButtonBase
	{
		

		public TreeWindowTier(int width, int height = 1, int indent = 0, bool hasParent = true, bool hasChild = true, bool hasSibling = true) : base(width, height)
		{
			this.textAlignment = HorizontalAlignment.Left;
		}

		private int DetermineNodeSymbol()
		{
			return 0;
		}

		private enum NodeSymbol
		{
			//Layer hollow glyph with + or - if collapsed or expanded
			Adam = 0,
			ParentFirst = 1,
			ParentMiddle = 2,
			ParentLast = 3,
			ChildFirst = 4,
			ChildMiddle = 5,
			ChildLast = 6
		}
	}
}
