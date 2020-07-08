using SadConsole.Controls;

using Microsoft.Xna.Framework;
using SadConsole.Themes;

/*
 * "I think you may have some design conflicts. The code on line 84* was designed for use with the cursor shifting the content of the console to detect a virtual "we've added some content" and enable the scroll bar to navigate within that buffer. But really you're taking a set of items and printing them out, so you don't need to dynamically (line by line) grow or shrink the buffer. You already know your size based on the items"
 *		* L84: if (_ScrollingConsole.TimesShiftedUp != 0 | _ScrollingConsole.Cursor.Position.Y >= _ScrollingConsole.ViewPort.Height + _scrollPosition)
 *		
 *	"With the item theme system too, you can display lines based on the type of object passed in the item
 *	The theme lets you just take the data, like if you had Title,Level,HasChildren and then could compose the item based on that data. You can always just stuff everything into a string and send that to the list box as an alternative"
 *	https://github.com/SadConsole/SadConsole/blob/master/src/SadConsole/Themes/ListBoxTheme.cs#L280
 *	
 *	"but you can see from the color item code. The ITEM theme isn't the same as CONTROL theme
	this is specific to the lsitbox and the listbox uses it to draw the individual item to the listbox
	its similar to control theme but its specific to the listbox"

Q: Are the themes updated or set upon loading? Like if I update the item's HP will the theme recalculate if I based its color on a % of HP?
A: only if you designed your theme to handle that. like the color theme. That one reads the color data and prints with a foreground/background based on the color.
when the listbox becomes dirty, it will redraw itself which processes each item through the theme

 */
namespace PureHatred.UI
{
	public class TreeViewMenu : ListBox
	{
		private ScrollBarTheme scrollBarTheme;

		public TreeViewMenu(int width, int height, ListBoxItemTheme itemTheme = null) : base(width, height, itemTheme)
		{
		}

		
	}
}
