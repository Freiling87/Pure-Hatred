using System;
using System.Text;

using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

using PureHatred.Entities;
using System.Collections.Generic;
using SadConsole.Input;
using PureHatred.Commands;

namespace PureHatred.UI
{
	public class SideWindow : Window
	{
        private readonly ListBox _listBox;
        private int _windowBorder = 2;

        public SideWindow(int width, int height, string title) : base(width, height)
        {
            height -= _windowBorder;
            width -= _windowBorder;

            Title = title.Align(HorizontalAlignment.Center, width);

            _listBox = new ListBox(width - _windowBorder / 2, height - _windowBorder / 2)
            {
                Position = new Point(1, 1),
                SingleClickItemExecute = true,
            };
            Add(_listBox);

            InventoryList();

            _listBox.SelectedItemExecuted += On_NodeToggled;
        }

        public void On_NodeToggled(object s, ListBox.SelectedItemEventArgs e)
        {
            List<BodyPart> visibleAnatomy = new List<BodyPart>();

            foreach (BodyPart bodyPart in GameLoop.World.Player.anatomy)
                if (bodyPart.IsNodeVisible())
                    visibleAnatomy.Add(bodyPart);

            BodyPart selection = visibleAnatomy[_listBox.SelectedIndex];

            selection.isNodeExpanded = !selection.isNodeExpanded;
            InventoryList();

            //TODO: Will probably need to maintain VisibleAnatomy within Player.
            // Ensure that the list writing method is pulling from there.
        }

        public void InventoryList()
		{
            Player player = GameLoop.World.Player;

            _listBox.Items.Clear();

            int i = 0;
            if (player.inventory.Count > 0)
                foreach (Item item in player.inventory)
                {
                    Item current = player.inventory[i++];
                    _listBox.Items.Add($"{GetNodeTreeSymbols(item)}{current.Name}");
                }

            i = 0;
            if (player.anatomy.Count > 0)
                foreach (BodyPart bodyPart in player.anatomy)
				{
                    if (bodyPart.IsNodeVisible())
					{
                        BodyPart current = player.anatomy[i++];
                        StringBuilder entry = new StringBuilder($"{GetNodeTreeSymbols(bodyPart)}{current.Name}");
                        String dataCols = $"{current.HpCurrent} / {current.HpMax.ToString().PadLeft(2)}";

                        entry.Append(dataCols.PadLeft(_listBox.Width - entry.Length));

                        _listBox.Items.Add(entry);
                    }
                }
        }

        public StringBuilder GetNodeTreeSymbols(Item item)
		{
            StringBuilder output = new StringBuilder("");
            int itemTier = item.CountParents();

            if (item.parent == null)
			{
                output.Append($"{(char)196}"); // ─ Adam Trunk
                if (item.children.Count != 0)
                    if (item.isNodeExpanded)
                        output.Append((char)194); // (-) Childsome, Expanded
                    else
                        output.Append((char)193); // (+) Childsome, Collapsed
            }
			else
			{
                for (int i = itemTier; i > 0; i--)
                    if (item.GetAncestor(i).IsLastborn())
                        output.Append(" "); // No further Uncles for that tier
                    else
                        output.Append((char)179); // │ Uncle-Branch(es)

                if (item == item.parent.children[^1])
                    output.Append((char)192); // └ Lastborn
                else
                    output.Append((char)195); // ├ Non-Lastborn

                if (item.children.Count != 0)
                    if (item.isNodeExpanded)
                        output.Append((char)194); // (-) Childsome, Expanded
                    else
                        output.Append((char)193); // (+) Childsome, Collapsed
                else
                    output.Append((char)196); // ─ Childless
            }
            return output;
		}

		//public override void Draw(TimeSpan drawTime) =>
		//	base.Draw(drawTime);

		//public override void Update(TimeSpan time) =>
		//	base.Update(time);
	}
}
