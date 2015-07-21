using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Resources;
using Sitecore.Rules;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.Personalize;
using Sitecore.Shell.Applications.Dialogs.Testing;
using Sitecore.Shell.Applications.Layouts.DeviceEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace AdvancedDeviceEditorForm
{
    /// <summary>
    /// Represents the Device Editor form.
    /// </summary>
    [UsedImplicitly]
    public class AdvancedDeviceEditorForm : DialogForm
    {
        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>The change button.</value>
        protected Button btnChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the edit.
        /// </summary>
        /// <value>The edit button.</value>
        protected Button btnEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the remove.
        /// </summary>
        /// <value>The Remove button.</value>
        protected Button btnRemove
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        /// <value>The controls.</value>
        public ArrayList Controls
        {
            get
            {
                return (ArrayList)Context.ClientPage.ServerProperties["Controls"];
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                Context.ClientPage.ServerProperties["Controls"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the device ID.
        /// </summary>
        /// <value>The device ID.</value>
        public string DeviceID
        {
            get
            {
                return StringUtil.GetString(Context.ClientPage.ServerProperties["DeviceID"]);
            }
            set
            {
                Assert.ArgumentNotNullOrEmpty(value, "value");
                Context.ClientPage.ServerProperties["DeviceID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>The layout.</value>
        protected TreePicker Layout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the move down.
        /// </summary>
        /// <value>The Move Down button.</value>
        protected Button MoveDown
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the move up.
        /// </summary>
        /// <value>The Move Up button.</value>
        protected Button MoveUp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the personalize button control.
        /// </summary>
        /// <value>The personalize button control.</value>
        protected Button Personalize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Edit placeholder button.
        /// </summary>
        /// <value>The Edit placeholder button.</value>
        protected Button phEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phRemove button.
        /// </summary>
        /// <value>Remove place holder button.</value>
        protected Button phRemove
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the placeholders.
        /// </summary>
        /// <value>The placeholders.</value>
        protected Scrollbox Placeholders
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the renderings.
        /// </summary>
        /// <value>The renderings.</value>
        protected Scrollbox Renderings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get
            {
                return MainUtil.GetInt(Context.ClientPage.ServerProperties["SelectedIndex"], -1);
            }
            set
            {
                Context.ClientPage.ServerProperties["SelectedIndex"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        /// <value>The test button.</value>
        protected Button Test
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        /// <value>The unique id.</value>
        public string UniqueId
        {
            get
            {
                return StringUtil.GetString(Context.ClientPage.ServerProperties["PlaceholderUniqueID"]);
            }
            set
            {
                Assert.ArgumentNotNullOrEmpty(value, "value");
                Context.ClientPage.ServerProperties["PlaceholderUniqueID"] = value;
            }
        }

        public AdvancedDeviceEditorForm()
        {
        }

        /// <summary>
        /// Adds the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:add", true)]
        [UsedImplicitly]
        protected void Add(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!args.IsPostBack)
            {
                SelectRenderingOptions selectRenderingOption = new SelectRenderingOptions()
                {
                    ShowOpenProperties = true,
                    ShowPlaceholderName = true,
                    PlaceholderName = string.Empty
                };
                SelectRenderingOptions item = selectRenderingOption;
                string str = Registry.GetString("/Current_User/SelectRendering/Selected");
                if (!string.IsNullOrEmpty(str))
                {
                    item.SelectedItem = Client.ContentDatabase.GetItem(str);
                }
                string str1 = item.ToUrlString(Client.ContentDatabase).ToString();
                SheerResponse.ShowModalDialog(str1, true);
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            {
                string[] strArrays = args.Result.Split(new char[] { ',' });
                string str2 = strArrays[0];
                string str3 = strArrays[1].Replace("-c-", ",");
                bool flag = strArrays[2] == "1";
                LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
                DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
                RenderingDefinition renderingDefinition = new RenderingDefinition()
                {
                    ItemID = str2,
                    Placeholder = str3
                };
                device.AddRendering(renderingDefinition);
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                if (flag)
                {
                    ArrayList renderings = device.Renderings;
                    if (renderings != null)
                    {
                        this.SelectedIndex = renderings.Count - 1;
                        Context.ClientPage.SendMessage(this, "device:edit");
                    }
                }
                Registry.SetString("/Current_User/SelectRendering/Selected", str2);
                return;
            }
        }

        /// <summary>
        /// Adds the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:addplaceholder", true)]
        [UsedImplicitly]
        protected void AddPlaceholder(ClientPipelineArgs args)
        {
            string str;
            if (!args.IsPostBack)
            {
                SelectPlaceholderSettingsOptions selectPlaceholderSettingsOption = new SelectPlaceholderSettingsOptions()
                {
                    IsPlaceholderKeyEditable = true
                };
                SheerResponse.ShowModalDialog(selectPlaceholderSettingsOption.ToUrlString().ToString(), "460px", "460px", string.Empty, true);
                args.WaitForPostBack();
            }
            else if (!string.IsNullOrEmpty(args.Result) && args.Result != "undefined")
            {
                LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
                DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
                Item item = SelectPlaceholderSettingsOptions.ParseDialogResult(args.Result, Client.ContentDatabase, out str);
                if (item == null || string.IsNullOrEmpty(str))
                {
                    return;
                }
                PlaceholderDefinition placeholderDefinition = new PlaceholderDefinition()
                {
                    UniqueId = ID.NewID.ToString(),
                    MetaDataItemId = item.Paths.FullPath,
                    Key = str
                };
                device.AddPlaceholder(placeholderDefinition);
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                return;
            }
        }

        /// <summary>
        /// Adds the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:change", true)]
        [UsedImplicitly]
        protected void Change(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (this.SelectedIndex < 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemID))
            {
                return;
            }
            if (!args.IsPostBack)
            {
                SelectRenderingOptions selectRenderingOption = new SelectRenderingOptions()
                {
                    ShowOpenProperties = true,
                    ShowPlaceholderName = false,
                    PlaceholderName = string.Empty,
                    SelectedItem = Client.ContentDatabase.GetItem(item.ItemID)
                };
                string str = selectRenderingOption.ToUrlString(Client.ContentDatabase).ToString();
                SheerResponse.ShowModalDialog(str, true);
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            {
                string[] strArrays = args.Result.Split(new char[] { ',' });
                item.ItemID = strArrays[0];
                bool flag = strArrays[2] == "1";
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                if (flag)
                {
                    Context.ClientPage.SendMessage(this, "device:edit");
                    return;
                }
            }
        }

        /// <summary>
        /// Changes the disable of the buttons.
        /// </summary>
        /// <param name="disable">if set to <c>true</c> buttons are disabled.</param>
        private void ChangeButtonsState(bool disable)
        {
            this.Personalize.Disabled = disable;
            this.btnEdit.Disabled = disable;
            this.btnChange.Disabled = disable;
            this.btnRemove.Disabled = disable;
            this.MoveUp.Disabled = disable;
            this.MoveDown.Disabled = disable;
            this.Test.Disabled = disable;
        }

        /// <summary>
        /// Edits the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:edit", true)]
        [UsedImplicitly]
        protected void Edit(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            RenderingParameters renderingParameter = new RenderingParameters()
            {
                Args = args,
                DeviceId = this.DeviceID,
                SelectedIndex = this.SelectedIndex,
                Item = UIUtil.GetItemFromQueryString(Client.ContentDatabase)
            };
            if (renderingParameter.Show())
            {
                this.Refresh();
            }
        }

        /// <summary>
        /// Edits the placeholder.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:editplaceholder", true)]
        [UsedImplicitly]
        protected void EditPlaceholder(ClientPipelineArgs args)
        {
            string str;
            Item item;
            if (string.IsNullOrEmpty(this.UniqueId))
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
            PlaceholderDefinition placeholder = device.GetPlaceholder(this.UniqueId);
            if (placeholder == null)
            {
                return;
            }
            if (!args.IsPostBack)
            {
                if (string.IsNullOrEmpty(placeholder.MetaDataItemId))
                {
                    item = null;
                }
                else
                {
                    item = Client.ContentDatabase.GetItem(placeholder.MetaDataItemId);
                }
                Item item1 = item;
                SelectPlaceholderSettingsOptions selectPlaceholderSettingsOption = new SelectPlaceholderSettingsOptions()
                {
                    TemplateForCreating = null,
                    PlaceholderKey = placeholder.Key,
                    CurrentSettingsItem = item1,
                    SelectedItem = item1,
                    IsPlaceholderKeyEditable = true
                };
                SelectPlaceholderSettingsOptions selectPlaceholderSettingsOption1 = selectPlaceholderSettingsOption;
                SheerResponse.ShowModalDialog(selectPlaceholderSettingsOption1.ToUrlString().ToString(), "460px", "460px", string.Empty, true);
                args.WaitForPostBack();
            }
            else if (!string.IsNullOrEmpty(args.Result) && args.Result != "undefined")
            {
                Item item2 = SelectPlaceholderSettingsOptions.ParseDialogResult(args.Result, Client.ContentDatabase, out str);
                if (item2 == null)
                {
                    return;
                }
                placeholder.MetaDataItemId = item2.Paths.FullPath;
                placeholder.Key = str;
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                return;
            }
        }

        /// <summary>
        /// Gets the layout definition.
        /// </summary>
        /// <returns>
        /// The layout definition.
        /// </returns>
        /// <contract><ensures condition="not null" /></contract>
        private static LayoutDefinition GetLayoutDefinition()
        {
            string sessionString = WebUtil.GetSessionString(AdvancedDeviceEditorForm.GetSessionHandle());
            Assert.IsNotNull(sessionString, "layout definition");
            return LayoutDefinition.Parse(sessionString);
        }

        /// <summary>
        /// Gets the session handle.
        /// </summary>
        /// <returns>
        /// The session handle string.
        /// </returns>
        private static string GetSessionHandle()
        {
            return "SC_DEVICEEDITOR";
        }

        /// <summary>
        /// Determines whether [has rendering rules] [the specified definition].
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if the definition has a defined rule with action; otherwise, <c>false</c>.</returns>
        private static bool HasRenderingRules(RenderingDefinition definition)
        {
            bool flag;
            if (definition.Rules == null)
            {
                return false;
            }
            RulesDefinition rulesDefinition = new RulesDefinition(definition.Rules.ToString());
            IEnumerable<XElement> rules = rulesDefinition.GetRules();
            if (rules == null)
            {
                return false;
            }
            using (IEnumerator<XElement> enumerator = rules.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    XElement current = enumerator.Current;
                    XElement xElement = current.Descendants("actions").FirstOrDefault<XElement>();
                    if (xElement == null || !xElement.Descendants().Any<XElement>())
                    {
                        continue;
                    }
                    flag = true;
                    return flag;
                }
                return false;
            }
            return flag;
        }

        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs" /> instance containing the event data.
        /// </param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page life cycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client post back,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                this.DeviceID = WebUtil.GetQueryString("de");
                DeviceDefinition device = AdvancedDeviceEditorForm.GetLayoutDefinition().GetDevice(this.DeviceID);
                if (device.Layout != null)
                {
                    this.Layout.Value = device.Layout;
                }
                if (Settings.Analytics.Enabled)
                {
                    this.Test.Visible = Policy.IsAllowed("Page Editor/Extended features/Testing");
                    this.Personalize.Visible = Policy.IsAllowed("Page Editor/Extended features/Personalization");
                }
                else
                {
                    this.Personalize.Visible = false;
                    this.Test.Visible = false;
                }
                this.Refresh();
                this.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            if (this.Layout.Value.Length > 0)
            {
                Item item = Client.ContentDatabase.GetItem(this.Layout.Value);
                if (item == null)
                {
                    Context.ClientPage.ClientResponse.Alert("Layout not found.");
                    return;
                }
                if (item.TemplateID == TemplateIDs.Folder || item.TemplateID == TemplateIDs.Node)
                {
                    ClientResponse clientResponse = Context.ClientPage.ClientResponse;
                    object[] displayName = new object[] { item.DisplayName };
                    clientResponse.Alert(Translate.Text("\"{0}\" is not a layout.", displayName));
                    return;
                }
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
            ArrayList renderings = device.Renderings;
            if (renderings != null && renderings.Count > 0 && this.Layout.Value.Length == 0)
            {
                Context.ClientPage.ClientResponse.Alert("You must specify a layout when you specify renderings.");
                return;
            }
            device.Layout = this.Layout.Value;
            AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
            Context.ClientPage.ClientResponse.SetDialogValue("yes");
            base.OnOK(sender, args);
        }

        /// <summary>
        /// Called when the rendering has click.
        /// </summary>
        /// <param name="uniqueId">
        /// The unique Id.
        /// </param>
        [UsedImplicitly]
        protected void OnPlaceholderClick(string uniqueId)
        {
            Assert.ArgumentNotNullOrEmpty(uniqueId, "uniqueId");
            if (!string.IsNullOrEmpty(this.UniqueId))
            {
                SheerResponse.SetStyle(string.Concat("ph_", ID.Parse(this.UniqueId).ToShortID()), "background", string.Empty);
            }
            this.UniqueId = uniqueId;
            if (!string.IsNullOrEmpty(uniqueId))
            {
                SheerResponse.SetStyle(string.Concat("ph_", ID.Parse(uniqueId).ToShortID()), "background", "#D0EBF6");
            }
            this.UpdatePlaceholdersCommandsState();
        }

        /// <summary>
        /// Called when the rendering has click.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        [UsedImplicitly]
        protected void OnRenderingClick(string index)
        {
            Assert.ArgumentNotNull(index, "index");
            if (this.SelectedIndex >= 0)
            {
                SheerResponse.SetStyle(StringUtil.GetString(this.Controls[this.SelectedIndex]), "background", string.Empty);
            }
            this.SelectedIndex = MainUtil.GetInt(index, -1);
            if (this.SelectedIndex >= 0)
            {
                SheerResponse.SetStyle(StringUtil.GetString(this.Controls[this.SelectedIndex]), "background", "#D0EBF6");
            }
            this.UpdateRenderingsCommandsState();
        }

        /// <summary>
        /// Personalizes the selected control.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:personalize", true)]
        [UsedImplicitly]
        protected void PersonalizeControl(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (this.SelectedIndex < 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemID) || string.IsNullOrEmpty(item.UniqueId))
            {
                return;
            }
            if (!args.IsPostBack)
            {
                Item itemFromQueryString = UIUtil.GetItemFromQueryString(Client.ContentDatabase);
                string str = (itemFromQueryString != null ? itemFromQueryString.Uri.ToString() : string.Empty);
                PersonalizeOptions personalizeOption = new PersonalizeOptions()
                {
                    SessionHandle = AdvancedDeviceEditorForm.GetSessionHandle(),
                    DeviceId = this.DeviceID,
                    RenderingUniqueId = item.UniqueId,
                    ContextItemUri = str
                };
                PersonalizeOptions personalizeOption1 = personalizeOption;
                SheerResponse.ShowModalDialog(personalizeOption1.ToUrlString().ToString(), "580px", "712px", string.Empty, true);
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            {
                item.Rules = XElement.Parse(args.Result);
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                return;
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        private void Refresh()
        {
            this.Renderings.Controls.Clear();
            this.Placeholders.Controls.Clear();
            this.Controls = new ArrayList();
            DeviceDefinition device = AdvancedDeviceEditorForm.GetLayoutDefinition().GetDevice(this.DeviceID);
            if (device.Renderings == null)
            {
                SheerResponse.SetOuterHtml("Renderings", this.Renderings);
                SheerResponse.SetOuterHtml("Placeholders", this.Placeholders);
                SheerResponse.Eval("if (!scForm.browser.isIE) { scForm.browser.initializeFixsizeElements(); }");
                return;
            }
            this.RenderRenderings(device, this.SelectedIndex, 0);
            this.RenderPlaceholders(device);
            this.UpdateRenderingsCommandsState();
            this.UpdatePlaceholdersCommandsState();
            SheerResponse.SetOuterHtml("Renderings", this.Renderings);
            SheerResponse.SetOuterHtml("Placeholders", this.Placeholders);
            SheerResponse.Eval("if (!scForm.browser.isIE) { scForm.browser.initializeFixsizeElements(); }");
        }

        /// <summary>
        /// Removes the specified message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [HandleMessage("device:remove")]
        [UsedImplicitly]
        protected void Remove(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            int selectedIndex = this.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                return;
            }
            if (selectedIndex < 0 || selectedIndex >= renderings.Count)
            {
                return;
            }
            renderings.RemoveAt(selectedIndex);
            if (selectedIndex >= 0)
            {
                AdvancedDeviceEditorForm deviceEditorForm = this;
                deviceEditorForm.SelectedIndex = deviceEditorForm.SelectedIndex - 1;
            }
            AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
            this.Refresh();
        }

        /// <summary>
        /// Removes the placeholder.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [HandleMessage("device:removeplaceholder")]
        [UsedImplicitly]
        protected void RemovePlaceholder(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (string.IsNullOrEmpty(this.UniqueId))
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
            PlaceholderDefinition placeholder = device.GetPlaceholder(this.UniqueId);
            if (placeholder == null)
            {
                return;
            }
            ArrayList placeholders = device.Placeholders;
            if (placeholders != null)
            {
                placeholders.Remove(placeholder);
            }
            AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
            this.Refresh();
        }

        /// <summary>
        /// Renders the placeholders.
        /// </summary>
        /// <param name="deviceDefinition">
        /// The device definition.
        /// </param>
        private void RenderPlaceholders(DeviceDefinition deviceDefinition)
        {
            Assert.ArgumentNotNull(deviceDefinition, "deviceDefinition");
            ArrayList placeholders = deviceDefinition.Placeholders;
            if (placeholders == null)
            {
                return;
            }
            foreach (PlaceholderDefinition placeholder in placeholders)
            {
                Item item = null;
                string metaDataItemId = placeholder.MetaDataItemId;
                if (!string.IsNullOrEmpty(metaDataItemId))
                {
                    item = Client.ContentDatabase.GetItem(metaDataItemId);
                }
                XmlControl webControl = Resource.GetWebControl("DeviceRendering") as XmlControl;
                Assert.IsNotNull(webControl, typeof(XmlControl));
                this.Placeholders.Controls.Add(webControl);
                ID d = ID.Parse(placeholder.UniqueId);
                if (placeholder.UniqueId == this.UniqueId)
                {
                    webControl["Background"] = "#D0EBF6";
                }
                string str = string.Concat("ph_", d.ToShortID());
                webControl["ID"] = str;
                webControl["Header"] = placeholder.Key;
                webControl["Click"] = string.Concat("OnPlaceholderClick(\"", placeholder.UniqueId, "\")");
                webControl["DblClick"] = "device:editplaceholder";
                if (item == null)
                {
                    webControl["Icon"] = "Imaging/24x24/layer_blend.png";
                }
                else
                {
                    webControl["Icon"] = item.Appearance.Icon;
                }
            }
        }

        /// <summary>
        /// Renders the specified device definition.
        /// </summary>
        /// <param name="deviceDefinition">
        /// The device definition.
        /// </param>
        /// <param name="selectedIndex">
        /// Index of the selected.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        private void RenderRenderings(DeviceDefinition deviceDefinition, int selectedIndex, int index)
        {
            Assert.ArgumentNotNull(deviceDefinition, "deviceDefinition");
            ArrayList renderings = deviceDefinition.Renderings;
            if (renderings == null)
            {
                return;
            }
            foreach (RenderingDefinition rendering in renderings)
            {
                if (rendering.ItemID == null)
                {
                    continue;
                }
                Item item = Client.ContentDatabase.GetItem(rendering.ItemID);
                XmlControl webControl = Resource.GetWebControl("DeviceRendering") as XmlControl;
                Assert.IsNotNull(webControl, typeof(XmlControl));
                HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
                htmlGenericControl.Style.Add("padding", "0");
                htmlGenericControl.Style.Add("margin", "0");
                htmlGenericControl.Style.Add("border", "0");
                htmlGenericControl.Style.Add("position", "relative");
                htmlGenericControl.Controls.Add(webControl);
                string uniqueID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("R");
                this.Renderings.Controls.Add(htmlGenericControl);
                htmlGenericControl.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("C");
                webControl["Click"] = string.Concat("OnRenderingClick(\"", index, "\")");
                webControl["DblClick"] = "device:edit";
                if (index == selectedIndex)
                {
                    webControl["Background"] = "#D0EBF6";
                }
                this.Controls.Add(uniqueID);
                if (item == null)
                {
                    webControl["ID"] = uniqueID;
                    webControl["Icon"] = "Applications/24x24/forbidden.png";
                    webControl["Header"] = "Unknown rendering";
                    webControl["Placeholder"] = string.Empty;
                    webControl["Description"] = string.Empty;

                }
                else
                {
                    webControl["ID"] = uniqueID;
                    webControl["Icon"] = item.Appearance.Icon;
                    webControl["Header"] = item.DisplayName;
                    webControl["Placeholder"] = WebUtil.SafeEncode(rendering.Placeholder);
                    webControl["Description"] = GetDescriptionValue(rendering);
                }
                if (Settings.Analytics.Enabled && rendering.Rules != null && !rendering.Rules.IsEmpty)
                {
                    int num = rendering.Rules.Elements("rule").Count<XElement>();
                    if (num > 1)
                    {
                        HtmlGenericControl str = new HtmlGenericControl("span");
                        if (num <= 9)
                        {
                            str.Attributes["class"] = "scConditionContainer";
                        }
                        else
                        {
                            str.Attributes["class"] = "scConditionContainer scLongConditionContainer";
                        }
                        str.InnerText = num.ToString();
                        htmlGenericControl.Controls.Add(str);
                    }
                }
                index++;
            }
        }

        /// <summary>
        /// Get Description Value 
        /// </summary>
        /// <param name="renderingDefinition">
        /// The RenderingDefinition.
        /// </param>
        private static string GetDescriptionValue(RenderingDefinition renderingDefinition)
        {
            Assert.ArgumentNotNull(renderingDefinition, "RenderingDefinition");
            string descriptionValue = string.Empty;
            if (renderingDefinition.Parameters != null)
            {
                NameValueCollection renderingParameters = Sitecore.Web.WebUtil.ParseUrlParameters(renderingDefinition.Parameters);
                descriptionValue = renderingParameters["Description"];
            }
            return descriptionValue;
        }

        /// <summary>
        /// Sets the definition.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private static void SetDefinition(LayoutDefinition layout)
        {
            Assert.ArgumentNotNull(layout, "layout");
            string xml = layout.ToXml();
            WebUtil.SetSessionValue(AdvancedDeviceEditorForm.GetSessionHandle(), xml);
        }

        /// <summary>
        /// The set test
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [HandleMessage("device:test", true)]
        [UsedImplicitly]
        protected void SetTest(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (this.SelectedIndex < 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            DeviceDefinition device = layoutDefinition.GetDevice(this.DeviceID);
            ArrayList renderings = device.Renderings;
            if (renderings == null)
            {
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                return;
            }
            if (!args.IsPostBack)
            {
                Item itemFromQueryString = UIUtil.GetItemFromQueryString(Client.ContentDatabase);
                SetTestDetailsOptions setTestDetailsOption = new SetTestDetailsOptions("SC_DEVICEEDITOR", itemFromQueryString.Uri.ToString(), device.ID, item.UniqueId);
                SheerResponse.ShowModalDialog(setTestDetailsOption.ToUrlString().ToString(), "520px", "695px", string.Empty, true);
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            {
                if (args.Result == "#reset#")
                {
                    item.MultiVariateTest = string.Empty;
                    AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                    this.Refresh();
                    return;
                }
                ID d = SetTestDetailsOptions.ParseDialogResult(args.Result);
                if (ID.IsNullOrEmpty(d))
                {
                    SheerResponse.Alert("Item not found.", new string[0]);
                    return;
                }
                item.MultiVariateTest = d.ToString();
                AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
                this.Refresh();
                return;
            }
        }

        /// <summary>
        /// Sorts the down.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [HandleMessage("device:sortdown")]
        [UsedImplicitly]
        protected void SortDown(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (this.SelectedIndex < 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                return;
            }
            if (this.SelectedIndex >= renderings.Count - 1)
            {
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                return;
            }
            renderings.Remove(item);
            renderings.Insert(this.SelectedIndex + 1, item);
            AdvancedDeviceEditorForm selectedIndex = this;
            selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
            AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
            this.Refresh();
        }

        /// <summary>
        /// Sorts the up.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [HandleMessage("device:sortup")]
        [UsedImplicitly]
        protected void SortUp(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (this.SelectedIndex <= 0)
            {
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                return;
            }
            renderings.Remove(item);
            renderings.Insert(this.SelectedIndex - 1, item);
            AdvancedDeviceEditorForm selectedIndex = this;
            selectedIndex.SelectedIndex = selectedIndex.SelectedIndex - 1;
            AdvancedDeviceEditorForm.SetDefinition(layoutDefinition);
            this.Refresh();
        }

        private void UpdatePlaceholdersCommandsState()
        {
            this.phEdit.Disabled = string.IsNullOrEmpty(this.UniqueId);
            this.phRemove.Disabled = string.IsNullOrEmpty(this.UniqueId);
        }

        /// <summary>
        /// Updates the state of the commands.
        /// </summary>
        private void UpdateRenderingsCommandsState()
        {
            if (this.SelectedIndex < 0)
            {
                this.ChangeButtonsState(true);
                return;
            }
            LayoutDefinition layoutDefinition = AdvancedDeviceEditorForm.GetLayoutDefinition();
            ArrayList renderings = layoutDefinition.GetDevice(this.DeviceID).Renderings;
            if (renderings == null)
            {
                this.ChangeButtonsState(true);
                return;
            }
            RenderingDefinition item = renderings[this.SelectedIndex] as RenderingDefinition;
            if (item == null)
            {
                this.ChangeButtonsState(true);
                return;
            }
            this.ChangeButtonsState(false);
            this.Personalize.Disabled = !string.IsNullOrEmpty(item.MultiVariateTest);
            this.Test.Disabled = AdvancedDeviceEditorForm.HasRenderingRules(item);
        }
    }
}
