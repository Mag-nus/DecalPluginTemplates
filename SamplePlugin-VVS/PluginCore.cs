using System;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;

/*
 * Created by Mag-nus. 8/19/2011, VVS added by Virindi-Inquisitor.
 * 
 * No license applied, feel free to use as you wish. H4CK TH3 PL4N3T? TR45H1NG 0UR R1GHT5? Y0U D3C1D3!
 * 
 * Notice how I use try/catch on every function that is called or raised by decal (by base events or user initiated events like buttons, etc...).
 * This is very important. Don't crash out your users!
 * 
 * In 2.9.6.4+ Host and Core both have Actions objects in them. They are essentially the same thing.
 * You sould use Host.Actions though so that your code compiles against 2.9.6.0 (even though I reference 2.9.6.5 in this project)
 * 
 * If you add this plugin to decal and then also create another plugin off of this sample, you will need to change the guid in
 * Properties/AssemblyInfo.cs to have both plugins in decal at the same time.
 * 
 * If you have issues compiling, remove the Decal.Adapater and VirindiViewService references and add the ones you have locally.
 * Decal.Adapter should be in C:\Games\Decal 3.0\
 * VirindiViewService should be in C:\Games\VirindiPlugins\VirindiViewService\
*/

namespace SamplePluginVVS
{
    //Attaches events from core
	[WireUpBaseEvents]

    //View (UI) handling
	[MVView("SamplePluginVVS.mainView.xml")]
    [MVWireUpControlEvents]

	// FriendlyName is the name that will show up in the plugins list of the decal agent (the one in windows, not in-game)
	// View is the path to the xml file that contains info on how to draw our in-game plugin. The xml contains the name and icon our plugin shows in-game.
	// The view here is SamplePlugin.mainView.xml because our projects default namespace is SamplePlugin, and the file name is mainView.xml.
	// The other key here is that mainView.xml must be included as an embeded resource. If its not, your plugin will not show up in-game.
	[FriendlyName("SamplePluginVVS")]
	public class PluginCore : PluginBase
	{
		/// <summary>
		/// This is called when the plugin is started up. This happens only once.
		/// </summary>
		protected override void Startup()
		{
			try
			{
				// This initializes our static Globals class with references to the key objects your plugin will use, Host and Core.
				// The OOP way would be to pass Host and Core to your objects, but this is easier.
				Globals.Init("SamplePluginVVS", Host, Core);

                //Initialize the view.
                MVWireupHelper.WireupStart(this, Host);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		/// <summary>
		/// This is called when the plugin is shut down. This happens only once.
		/// </summary>
		protected override void Shutdown()
		{
			try
			{
                //Destroy the view.
                MVWireupHelper.WireupEnd(this);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("LoginComplete", "CharacterFilter")]
		private void CharacterFilter_LoginComplete(object sender, EventArgs e)
		{
			try
			{
				Util.WriteToChat("Plugin now online. Server population: " + Core.CharacterFilter.ServerPopulation);

				Util.WriteToChat("CharacterFilter_LoginComplete");

				InitSampleList();

				// Subscribe to events here
				Globals.Core.WorldFilter.ChangeObject += new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject2);

			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("Logoff", "CharacterFilter")]
		private void CharacterFilter_Logoff(object sender, Decal.Adapter.Wrappers.LogoffEventArgs e)
		{
			try
			{
				// Unsubscribe to events here, but know that this event is not gauranteed to happen. I've never seen it not fire though.
				// This is not the proper place to free up resources, but... its the easy way. It's not proper because of above statement.
				Globals.Core.WorldFilter.ChangeObject -= new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject2);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}


		// Note that there are several ways to latch on to decals events.
		// You can use the BaseEvent attribute, or you can latch on to the same event as shown in CharacterFilter_LoginComplete, above.
		// The BaseEvent method will only work in this class as it is derived from PluginBase. You will need to use the += and -= method in your other objects.
		[BaseEvent("ChangeObject", "WorldFilter")]
		void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e)
		{
			try
			{
				// This can get very spammy so I filted it to just print on ident received
				if (e.Change == WorldChangeType.IdentReceived)
					Util.WriteToChat("WorldFilter_ChangeObject: " + e.Changed.Name + " " + e.Change);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		void WorldFilter_ChangeObject2(object sender, ChangeObjectEventArgs e)
		{
		}




		[MVControlEvent("UseSelectedItem", "Click")]
		void UseSelectedItem_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				if (Globals.Host.Actions.CurrentSelection == 0 || Globals.Core.WorldFilter[Globals.Host.Actions.CurrentSelection] == null)
				{
					Util.WriteToChat("UseSelectedItem no item selected");

					return;
				}

				Util.WriteToChat("UseSelectedItem " + Globals.Core.WorldFilter[Globals.Host.Actions.CurrentSelection].Name);

				Globals.Host.Actions.UseItem(Globals.Host.Actions.CurrentSelection, 0);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("ToggleAttack", "Click")]
		void ToggleAttack_Click(object sender, MVControlEventArgs e)
		{
			try
			{
				Util.WriteToChat("ToggleAttack");

				if (Globals.Host.Actions.CombatMode == CombatState.Peace)
					Globals.Host.Actions.SetCombatMode(CombatState.Melee);
				else
					Globals.Host.Actions.SetCombatMode(CombatState.Peace);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[MVControlEvent("DoesNothing", "Click")]
		void DoesNothing_Click(object sender, MVControlEventArgs e)
		{
			try
			{

			}
			catch (Exception ex) { Util.LogError(ex); }
		}





		[MVControlReference("SampleList")]
		private IList SampleList = null;

		private void InitSampleList()
		{
			foreach (WorldObject worldObject in Globals.Core.WorldFilter.GetByContainer(Globals.Core.CharacterFilter.Id))
			{
				IListRow row = SampleList.Add();

				row[0][1] = worldObject.Icon + 0x6000000; // Notice we're using an index of 1 for the second parameter. That's because this is a DecalControls.IconColumn column.
				row[1][0] = worldObject.Name;

				// Also note that you can create an empty column. In our mainView.xml we have:
				// <column progid="DecalControls.TextColumn" name="colF" />
				// It is column index 5 and has no size associated with it. You can use this column to store an id of an item, or other misc data that you can use
				// later to grab info about the row, or maybe its sort order, etc..

				// To clear the list you can do:
				// SampleList.Clear();


				// If we want to check if this item is equipped, we could do the following
				if (worldObject.Values(LongValueKey.EquippedSlots) > 0)
				{
				}

				// This check will pass if the object is wielded
				// Take note that someone else might be wielding this object. If you want to know if its wielded by YOU, you need to add another check.
				if (worldObject.Values(LongValueKey.Slot, -1) == -1)
				{
				}

				// You can get an items current mana, but only if the item has been id'd, otherwise it will just return 0.
				if (worldObject.HasIdData)
				{
					int currentMana = worldObject.Values(LongValueKey.CurrentMana);
				}

				// But also note that we don't know how long ago this item has been id'd. Maybe it was id'd an hour ago? The mana data would be erroneous.
				// So, we could get fresh id data for the object with the following:
				// Globals.Host.Actions.RequestId(worldObject.Id);

				// But now note that it may take a second or so to get that id data. So if we did the following:
				// Globals.Host.Actions.RequestId(worldObject.Id);
				// worldObject.Values(LongValueKey.CurrentMana) <-- This would still be the OLD information and not the new because the above statement hasn't finished.
			}
		}

		[MVControlReference("SampleListText")]
		private IStaticText SampleListText = null;

		[MVControlReference("SampleListCheckBox")]
		private ICheckBox SampleListCheckBox = null;

		[MVControlEvent("SampleListCheckBox", "Change")]
		void SampleListCheckBox_Change(object sender, MVCheckBoxChangeEventArgs e)
		{
			try
			{
				Util.WriteToChat("SampleListCheckBox_Change " + SampleListCheckBox.Checked);

				SampleListText.Text = SampleListCheckBox.Checked.ToString();
			}
			catch (Exception ex) { Util.LogError(ex); }
		}
	}
}
