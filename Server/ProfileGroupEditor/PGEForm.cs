using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    public partial class PGEForm : Form
    {
        private string[] commandLineArgs;

        public PGEForm(string[] args)
        {
            InitializeComponent();

            DirectoryInfo dirInfo = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location);
            openFileDialog.InitialDirectory = dirInfo.FullName + @"\app_data\Profiles";
            saveFileDialog.InitialDirectory = openFileDialog.InitialDirectory;

            commandLineArgs = args;
        }

        private ProfileGroup _myGroup = null;
        /// <summary>
        /// The originally loaded or created ProfileGroup
        /// </summary>
        protected ProfileGroup ProfileGroup
        {
            get
            {
                return _myGroup;
            }
            set
            {
                if (value == null)
                {
                    value = new ProfileGroup();
                    value.Name = "New Profile Group";
                }

                _myGroup = value;
                _workingGroup = (ProfileGroup)value.Clone();

                UpdateTitleBar();

                tvGroup.Nodes.Clear();

                AddProfileGroupNode(WorkingProfileGroup);               
            }
        }

        /// <summary>
        /// Updates the title bar to contain the name of the current file, if any
        /// </summary>
        private void UpdateTitleBar()
        {
            string title = "";
            if (saveFileDialog.FileName != "")
            {
                title = Path.GetFileName(saveFileDialog.FileName);
                if (IsModified)
                {
                    title += " *";
                }
                title += " - ";
            }

            this.Text = title + (Application.ProductName + " " + Application.ProductVersion);
        }

        private ProfileGroup _workingGroup = null;
        /// <summary>
        /// The "modified" ProfileGroup
        /// </summary>
        protected ProfileGroup WorkingProfileGroup
        {
            get { return _workingGroup; }
            set { _workingGroup = value; }
        }

        /// <summary>
        /// Returns true if the Working Profile Group has been modified.
        /// </summary>
        protected bool IsModified
        {
            get
            {
                return (WorkingProfileGroup != ProfileGroup);
            }
        }

        /// <summary>
        /// Determines if the user cares about destroying the working profile
        /// </summary>
        /// <returns>
        /// returns true if we can destroy, or false if not
        /// </returns>
        private bool CanDestroyWorkingProfileGroup()
        {
            return CanDestroyWorkingProfileGroup(String.Empty);
        }
        private bool CanDestroyWorkingProfileGroup(string customMessage)
        {
            if (customMessage == null)
            {
                customMessage = String.Empty;
            }

            if ((IsModified) && (WorkingProfileGroup.Items.Count > 0))
            {
                if (customMessage != String.Empty)
                {
                    customMessage += Environment.NewLine + Environment.NewLine;
                }
                
                DialogResult save = MessageBox.Show(this, customMessage + "The current working profile group has not been saved! Would you like to save it before continuing?", "Save before continue?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (save == DialogResult.Yes)
                {
                    return SaveFile(saveFileDialog.FileName, WorkingProfileGroup);
                }
                else if (save == DialogResult.No)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Called when the form loads.
        /// If commandLineArgs has been set, it will attempt to load a file at that location.
        /// If no such file exists, or there are no arguments, a new profile group will be started.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (commandLineArgs.Length == 1)
            {
                if (File.Exists(commandLineArgs[0]))
                {
                    if (OpenFile(commandLineArgs[0]))
                    {
                        return;
                    }
                }
            }

            ProfileGroup = null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CanDestroyWorkingProfileGroup();
        }

        #region File IO

        /// <summary>
        /// saves a file.
        /// </summary>
        /// <param name="filename">"" to ask user to select a new file, or provide a filename to save directly</param>
        /// <param name="saveMe">The Profile group to save.</param>
        /// <returns>returns true if the user saved, or false if the user canceled.</returns>
        private bool SaveFile(string filename, ProfileGroup saveMe)
        {
            if (saveMe.Items.Count == 0)
            {
                ShowErrorCheckMessage("A Profile Group must contain one or more Profiles.", saveMe.Name, true);
                return false;
            }

            if (!ErrorCheckProfileGroup(saveMe))
            {
                return false;
            }

            if (filename == "")
            {
                saveFileDialog.FileName = groupEditor.ProfileGroup.Name + ".xml";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    filename = saveFileDialog.FileName;
                }
                else
                {
                    saveFileDialog.FileName = "";
                    return false;
                }
            }

            try
            {
                saveMe.SaveToFile(filename);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                "An error occurred while saving the Profile Group!" + Environment.NewLine + Environment.NewLine + ex.Message,
                                "Error while saving!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                saveFileDialog.FileName = "";
                return false;
            }
        }

        /// <summary>
        /// Opens a given file
        /// </summary>
        /// <param name="filename">path of file to open. if successful, this path will be cached for saving and opening</param>
        /// <returns>returns true if the operation was successful, false if it failed.</returns>
        private bool OpenFile(string filename)
        {
            try
            {
                ProfileGroup temp = ProfileGroup.LoadFromFile(filename);
                try
                {
                    ProfileGroup = temp;
                    saveFileDialog.FileName = openFileDialog.FileName = filename;
                    return true;
                }
                catch (Exception loadEx)
                {
                    MessageBox.Show(this, "An error occurred while loading the file! A blank Profile Group will be loaded instead." + Environment.NewLine + Environment.NewLine + loadEx.Message, "Error loading file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    saveFileDialog.FileName = "";
                    ProfileGroup = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "An error occurred while reading the file!" + Environment.NewLine + Environment.NewLine + ex.ToString(), "Error reading file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        #endregion

        #region Menu Handling

        /// <summary>
        /// Creates a new profile group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mFileNew_Click(object sender, EventArgs e)
        {
            if (CanDestroyWorkingProfileGroup())
            {
                ProfileGroup = null;
                saveFileDialog.FileName = "";
                UpdateTitleBar();
            }
        }

        /// <summary>
        /// Guides the user through opening a file
        /// </summary>
        private void mFileOpen_Click(object sender, EventArgs e)
        {
            if (CanDestroyWorkingProfileGroup())
            {
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    OpenFile(openFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Saves the file. If the file has been saved before, the last filename is used.
        /// </summary>
        private void mFileSave_Click(object sender, EventArgs e)
        {
            if ((saveFileDialog.FileName != "") && (Path.GetFileNameWithoutExtension(saveFileDialog.FileName) != WorkingProfileGroup.Name))
            {
                if (DialogResult.Yes == MessageBox.Show(this, "The Profile Group Name and the filename are not the same. For Video Server, it is neccesary that the file name and Profile Group Name be identical." + Environment.NewLine + Environment.NewLine + "Would you like to use the Profile Group Name for the file name?", "Profile Group Name not equal to filename!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    saveFileDialog.FileName = "";
                }
            }

            if (SaveFile(saveFileDialog.FileName, WorkingProfileGroup))
            {
                ProfileGroup = (ProfileGroup)WorkingProfileGroup.Clone();
            }
        }

        /// <summary>
        /// Saves the file. Always prompts for a new file name.
        /// </summary>
        private void mFileSaveAs_Click(object sender, EventArgs e)
        {
            if (SaveFile("", WorkingProfileGroup))
            {
                ProfileGroup = (ProfileGroup)WorkingProfileGroup.Clone();
            }
        }

        /// <summary>
        /// Closes the form, which triggers application exit.
        /// </summary>
        private void mFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// called when the Edit->Copy menu item is selected
        /// </summary>
        private void mEditCopy_Click(object sender, EventArgs e)
        {
            //CopyToClipboard(tvGroup.SelectedNode);
            if (tvGroup.SelectedNode.Level == 0)
            {
                CopyToClipboard(WorkingProfileGroup);
            }
            else
            {
                CopyToClipboard(WorkingProfileGroup[tvGroup.SelectedNode.Index]);
            }
        }

        /// <summary>
        /// Called when Edit->Paste menu item is selected
        /// </summary>
        private void mEditPaste_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        /// <summary>
        /// Called when the Copy menu item is selected from the context menu
        /// </summary>
        private void mTVContextCopy_Click(object sender, EventArgs e)
        {
            //CopyToClipboard(ContextNode);

            if(ContextNode.Level == 0)
            {
                CopyToClipboard(WorkingProfileGroup);
            }
            else
            {
                CopyToClipboard(WorkingProfileGroup[ContextNode.Index]);
            }
        }

        /// <summary>
        /// Called when the Paste menu item is selected from the context menu
        /// </summary>
        private void mTVContextPaste_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        /// <summary>
        /// Duplicates the selected profile
        /// </summary>
        private void mEditDuplicate_Click(object sender, EventArgs e)
        {
            if (tvGroup.SelectedNode.Level > 0)
            {
                DuplicateProfile(WorkingProfileGroup[tvGroup.SelectedNode.Index]);
            }
        }

        /// <summary>
        /// Called when the Duplicate menu item is selected from the context menu
        /// </summary>
        private void mTVContextDuplicate_Click(object sender, EventArgs e)
        {
            if (ContextNode.Level > 0)
            {
                DuplicateProfile(WorkingProfileGroup[ContextNode.Index]);
            }
        }

        /// <summary>
        /// Serializes any object to a string
        /// </summary>
        /// <typeparam name="T">the Type of the parameter "obj"</typeparam>
        /// <param name="obj">object to serialize</param>
        /// <returns>a string that is the XML-serialized version of the parameter "obj"</returns>
        private string SerializeToString<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);

            serializer.Serialize(stream, obj);

            return new UTF8Encoding().GetString(stream.ToArray());
        }

        /// <summary>
        /// Deserializes an XML string and outputs the result to the specified target object.
        /// </summary>
        /// <typeparam name="T">the type of the target object for output</typeparam>
        /// <param name="xmlString">string to de-serialize</param>
        /// <param name="target">object to output to.</param>
        private void DeserializeFromString<T>(string xmlString, out T target)
        {
            if (xmlString == null)
            {
                throw new ArgumentNullException("xmlString");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(new UTF8Encoding().GetBytes(xmlString));
            target = (T)serializer.Deserialize(stream);
        }

        #endregion

        #region Error Checking

        /// <summary>
        /// Checks an entire profile group for errors
        /// </summary>
        /// <param name="g">the group to check</param>
        /// <returns>true if no errors were found, false if errors were encountered</returns>
        private bool ErrorCheckProfileGroup(ProfileGroup g)
        {
            if (DisableErrorChecking)
            {
                return true;
            }

            if (String.IsNullOrEmpty(g.Name))
            {
                ShowErrorCheckMessage("You must specify a name for the Profile Group!", g.Name, true);
                ShowErrorNode(GetTreeNode(g.Name));
                return false;
            }

            if (!ErrorCheckNameTokens(g.Name, true))
            {
                ShowErrorNode(GetTreeNode(g.Name));
                return false;
            }

            if (String.IsNullOrEmpty(g.DefaultProfileName) &&
                (groupEditor.ProfileGroup.Items.Count > 0))
            {
                ShowErrorCheckMessage("You must select a Default Profile!", g.Name, true);
                ShowErrorNode(GetTreeNode(g.Name));
                return false;
            }

            foreach (Profile p in g.Items)
            {
                if (!ErrorCheckProfile(p))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks a specified profile for errors
        /// </summary>
        /// <param name="p">profile to check</param>
        /// <returns>returns true if the profile is OK, or false if the profile has a problem</returns>
        private bool ErrorCheckProfile(Profile p)
        {
            if (p == null)
            {
                throw new ArgumentNullException("p", "The profile to check cannot be null!");
            }

            if (DisableErrorChecking)
            {
                return true;
            }

            TreeNode profNode = GetTreeNode(p.Name);

            if (String.IsNullOrEmpty(p.Name))
            {
                ShowErrorCheckMessage("The Profile name cannot be blank.", p.Name);
                ShowErrorNode(profNode);
                return false;
            }

            if (!ErrorCheckNameTokens(p.Name, false))
            {
                ShowErrorNode(profNode);
                return false;
            }

            //check for duplicate names
            if (profNode != null)
            {
                if(DuplicateProfileNameExists(profNode.Parent, p, true))
                {
                    ShowErrorCheckMessage("Every profile must have a unique name! Please change this profile's name.", p.Name);
                    ShowErrorNode(profNode);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks a name string for illegal tokens
        /// </summary>
        /// <param name="name">name to check</param>
        /// <param name="isAGroupName">true if name is the name of a group</param>
        /// <returns>returns true if name string is clean, or false if it contains one or more of the illegal tokens</returns>
        private bool ErrorCheckNameTokens(string name, bool isAGroupName)
        {

            if (name.Contains(":") ||
                name.ToLower().Contains("custom"))
            {
                ShowErrorCheckMessage("The following tokens are reserved and cannot be in any name." +
                                      Environment.NewLine +
                                      ":, custom",
                                      name,
                                      isAGroupName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Displays the treenode that contains errors the user needs to correct
        /// </summary>
        /// <param name="n">node to display</param>
        private void ShowErrorNode(TreeNode n)
        {
            DisableErrorChecking = true;
            tvGroup.SelectedNode = n;
            DisableErrorChecking = false;
        }

        private bool _disableErrorChecking = false;
        /// <summary>
        /// This property determines if error checking should occur or not
        /// </summary>
        public bool DisableErrorChecking
        {
            get { return _disableErrorChecking; }
            set { _disableErrorChecking = value; }
        }

        /// <summary>
        /// determines if a profile with the same name as a given profile exists
        /// </summary>
        /// <param name="parent">The parent tree node to look under.</param>
        /// <param name="p">profile to check</param>
        /// <param name="profileIsInParent">If this is true, the profile is in the parent, if false, the profile has not been added to the parent yet.</param>
        /// <returns>true if exists, false if not</returns>
        private bool DuplicateProfileNameExists(TreeNode parent, Profile p, bool profileIsInParent)
        {
            TreeNode profNode = null;
            if (profileIsInParent)
            {
                profNode = GetTreeNode(p.Name, parent.Nodes);
            }

            foreach (TreeNode n in parent.Nodes)
            {
                if (((profNode == null) ? true : (n != profNode)) &&
                    (n.Text == p.Name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shows a message box concerning a profile error check
        /// </summary>
        /// <param name="msg">message to display</param>
        /// <param name="name">name of the profile</param>
        private void ShowErrorCheckMessage(string msg, string name)
        {
            ShowErrorCheckMessage(msg, name, false);
        }
        /// <summary>
        /// Shows a message box concerning an error check
        /// </summary>
        /// <param name="msg">message to display</param>
        /// <param name="name">name of the profile or profile group</param>
        /// <param name="isAGroupMessage">true if this message regards a group, or false if it regards a single profile.</param>
        private void ShowErrorCheckMessage(string msg, string name, bool isAGroupMessage)
        {
            MessageBox.Show(this, msg, 
                "Error in Profile " + (isAGroupMessage ? "Group " : "") + ((name != "") ? "\"" + name + "\"" : ""),
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Profile Group Browser

        /// <summary>
        /// Adds a ProfileGroup to the tree!
        /// </summary>
        /// <param name="g"></param>
        private void AddProfileGroupNode(ProfileGroup g)
        {
            TreeNode parent = tvGroup.Nodes.Add(g.Name);
            parent.Tag = g;
            foreach (Profile p in _myGroup.Items)
            {
                AddProfileNode(parent, p);
            }
            parent.Expand();

            tvGroup.SelectedNode = parent;
        }

        /// <summary>
        /// Adds a profile sub-node to a profile group node
        /// </summary>
        /// <param name="parent">index of the parent node</param>
        /// <param name="p">profile to add</param>
        /// <returns>returns the TreeNode created for this profile.</returns>
        private TreeNode AddProfileNode(TreeNode parent, Profile p)
        {
            TreeNode child = parent.Nodes.Add(p.Name);
            //child.Tag = p;
            return child;
        }

        /// <summary>
        /// Copies the object associated with a given tree node to the clipboard
        /// </summary>
        /// <param name="treeNode">tree node to copy from</param>
        private void CopyToClipboard(object item)
        {
            try
            {
                if (item is ProfileGroup)
                {
                    Clipboard.SetText(SerializeToString(item as ProfileGroup));
                }
                else if(item is Profile)
                {
                    Clipboard.SetText(SerializeToString(item as Profile));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                "Could not copy the selected profile or group." + Environment.NewLine + Environment.NewLine + ex.Message,
                                "Error copying to clipboard!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Pastes a ProfileGroup or Profile from the clipboard into the working profile group
        /// </summary>
        private void PasteFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    //check for possible profile group
                    if (text.Contains("ProfileGroup"))
                    {
                        //attempt to de-serialize
                        ProfileGroup output;
                        DeserializeFromString(text, out output);
                        //if we made it this far, ask if we can effectively Open a document
                        if (CanDestroyWorkingProfileGroup("You are about to paste a new Profile Group, which will cause the current one to close."))
                        {
                            saveFileDialog.FileName = "";
                            ProfileGroup = output;
                        }
                    }
                    //check for possible Profile
                    else if (text.Contains("Profile"))
                    {
                        //attempt to deserialize
                        Profile output;
                        DeserializeFromString(text, out output);

                        //if such a profile already exists, change the name
                        if (DuplicateProfileNameExists(GetTreeNode(WorkingProfileGroup.Name), output, false))
                        {
                            output.Name += (++_addProfileCount).ToString();
                        }

                        //add the profile
                        DisableErrorChecking = true;
                        WorkingProfileGroup.Items.Add(output);
                        tvGroup.SelectedNode = AddProfileNode((tvGroup.SelectedNode.Level == 0) ?
                                                           tvGroup.SelectedNode : tvGroup.SelectedNode.Parent,
                                                           output);
                        DisableErrorChecking = false;
                    }
                    //probably nothing we can parse
                    else
                    {
                        MessageBox.Show(this, "Clipboard does not contain recognized XML.", @"Can't Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(this, "Clipboard does not contain text.", "Nothing to Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                                @"Could not paste the clipboard's contents." + Environment.NewLine + Environment.NewLine + ex.Message,
                                "Error while pasting!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Moves the specified TreeNode, contained in the given ProfileGroup a certain number of indicies.
        /// </summary>
        /// <param name="moveMe">TreeNode being moved</param>
        /// <param name="group">ProfileGroup that contains the Profile represented by the moving TreeNode</param>
        /// <param name="indicies">direction and number of indicies to move. Negative = up, Positive = down</param>
        private void MoveProfileNode(TreeNode moveMe, ProfileGroup group, int indicies)
        {
            try
            {
                TreeNode parent = moveMe.Parent;

                Profile p = group.Items[moveMe.Index];

                parent.Nodes.RemoveAt(moveMe.Index);
                group.Items.RemoveAt(moveMe.Index);

                group.Items.Insert(moveMe.Index + indicies, p);
                parent.Nodes.Insert(moveMe.Index + indicies, moveMe);
            }
            finally
            {
                tvGroup.SelectedNode = moveMe;
            }
        }

        /// <summary>
        /// Duplicates the profile associated with the given tree node
        /// </summary>
        /// <param name="treeNode">tree node to duplicate</param>
        private void DuplicateProfile(Profile p)
        {
            if (p != null)
            {
                Profile clone = (Profile)p.Clone();
                clone.Name += (++_addProfileCount).ToString();
                WorkingProfileGroup.Items.Add(clone);
                tvGroup.SelectedNode = AddProfileNode(GetTreeNode(WorkingProfileGroup.Name), clone);
            }
        }

        /// <summary>
        /// Removes a given tree node from the Working Profile Group
        /// </summary>
        /// <param name="treeNode">tree node to remove</param>
        private void RemoveProfile(TreeNode treeNode)
        {
            if (treeNode.Level == 0)
            {
                return;
            }

            if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to remove the Profile \"" + treeNode.Text + "\"? This action cannot be un-done.", "Remove Profile?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                WorkingProfileGroup.Items.RemoveAt(treeNode.Index);
                treeNode.Parent.Nodes.Remove(treeNode);
            }
        }

        /// <summary>
        /// Finds the first TreeNode matching a given name, case insensitive. Looks in all of the Nodes in
        /// the main tree view control for the application.
        /// </summary>
        /// <param name="name">Name to find</param>
        /// <returns>the TreeNode with the specified name, or null if not found</returns>
        private TreeNode GetTreeNode(string name)
        {
            return GetTreeNode(name, tvGroup.Nodes);
        }

        /// <summary>
        /// Finds the first TreeNode matching a given name, case insensitive
        /// </summary>
        /// <param name="name">name of TreeNode to locate</param>
        /// <param name="start">collection to begin searching in</param>
        /// <returns>the TreeNode with the specified name, or null if not found</returns>
        private TreeNode GetTreeNode(string name, TreeNodeCollection start)
        {
            foreach (TreeNode child in start)
            {
                if (child.Text.ToLower() == name.ToLower())
                {
                    return child;
                }
                else
                {
                    TreeNode n = GetTreeNode(name, child.Nodes);
                    if (n != null)
                    {
                        return n;
                    }
                }
            }
            return null;
        }

/*
        /// <summary>
        /// Returns the TreeNode with the specified object for its Tag
        /// </summary>
        /// <param name="o">object to look for</param>
        /// <returns>the TreeNode with Tag == o. Returns null if not found.</returns>
        private TreeNode GetTreeNode(object o)
        {
            try
            {
                foreach (TreeNode parent in tvGroup.Nodes)
                {
                    if (o is ProfileGroup)
                    {
                        if (((ProfileGroup)parent.Tag) == ((ProfileGroup)o))
                        {
                            return parent;
                        }
                    }
                    else if(o is Profile)
                    {
                        foreach (TreeNode child in parent.Nodes)
                        {
                            if (((Profile)child.Tag) == ((Profile)o))
                            {
                                return child;
                            }
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
 */

        private void tvGroup_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (tvGroup.SelectedNode == null)
            {
                return;
            }

            if (e.Action == TreeViewAction.Expand)
            {
                return;
            }

            //do ProfileGroup-checking
            if (tvGroup.SelectedNode.Level == 0)
            {
                //if (!ErrorCheckProfileGroup(tvGroup.SelectedNode.Tag as ProfileGroup))
                if(!ErrorCheckProfileGroup(WorkingProfileGroup))
                {
                    e.Cancel = true;
                }
            }
            else
            {
                //if (!ErrorCheckProfile(tvGroup.SelectedNode.Tag as Profile))
                if(!ErrorCheckProfile(WorkingProfileGroup[tvGroup.SelectedNode.Index]))
                {
                    e.Cancel = true;
                }
            }
        }

        private void tvGroup_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnRemoveProfile.Enabled = ((tvGroup.SelectedNode.Level != 0) || (tvGroup.Nodes.Count > 1));
            btnMoveUp.Enabled = ((tvGroup.SelectedNode.Level != 0) && (tvGroup.SelectedNode.Index > 0));
            btnMoveDown.Enabled = ((tvGroup.SelectedNode.Level != 0) && (tvGroup.SelectedNode.Index < (tvGroup.SelectedNode.Parent.Nodes.Count - 1)));

            //show new screen
            if (e.Node.Level == 0)
            {
                //ShowGroupEditor(e.Node.Tag as ProfileGroup);
                ShowGroupEditor(WorkingProfileGroup);

                mEditCopy.Enabled = true;
                mEditCopy.Text = "&Copy Profile Group";
                mEditDuplicate.Enabled = false;
            }
            else
            {
                //ShowProfileEditor(e.Node.Tag as Profile);
                ShowProfileEditor(WorkingProfileGroup[e.Node.Index]);

                mEditCopy.Enabled = true;
                mEditCopy.Text = "&Copy Profile";
                mEditDuplicate.Enabled = true;
            }
        }

        /// <summary>
        /// Displays the overall group editor.
        /// </summary>
        /// <param name="group">group to display in the editor</param>
        private void ShowGroupEditor(ProfileGroup group)
        {
            this.profileEditor.Dock = DockStyle.Top;
            this.profileEditor.Height = 0;

            this.groupEditor.Dock = DockStyle.Fill;

            this.groupEditor.ProfileGroup = group;
        }

        /// <summary>
        /// Displays the profile editor
        /// </summary>
        /// <param name="profile">profile to display in the editor</param>
        private void ShowProfileEditor(Profile profile)
        {
            this.groupEditor.Dock = DockStyle.Top;
            this.groupEditor.Height = 0;

            this.profileEditor.Dock = DockStyle.Fill;

            this.profileEditor.Profile = profile;
        }

        private int _addProfileCount = -1;

        /// <summary>
        /// called when the Add Profile button is clicked
        /// </summary>
        private void btnAddProfile_Click(object sender, EventArgs e)
        {
            Profile n = new Profile();
            n.Name = "New Profile" + ((++_addProfileCount > 0) ? _addProfileCount.ToString() : "");
            WorkingProfileGroup.Items.Add(n);

            if (WorkingProfileGroup.Items.Count == 1)
            {
                WorkingProfileGroup.DefaultProfileName = n.Name;
            }

            TreeNode parent = GetTreeNode(WorkingProfileGroup.Name);
            tvGroup.SelectedNode = AddProfileNode(parent, n);
            parent.Expand();
            
        }

        /// <summary>
        /// called when the Remove Profile button is clicked
        /// </summary>
        private void btnRemoveProfile_Click(object sender, EventArgs e)
        {
            RemoveProfile(tvGroup.SelectedNode);
        }

        /// <summary>
        /// Called when the Delete item is selected from the context menu
        /// </summary>
        private void mTVContextDelete_Click(object sender, EventArgs e)
        {
            RemoveProfile(ContextNode);
        }

        /// <summary>
        /// called when the Move Up button is called
        /// </summary>
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (tvGroup.SelectedNode.Level == 0)
            {
                return;
            }

            if (tvGroup.SelectedNode.Index < 1)
            {
                return;
            }

            MoveProfileNode(tvGroup.SelectedNode, WorkingProfileGroup, -1);
        }

        /// <summary>
        /// Called when the Move Up context menu item is clicked
        /// </summary>
        private void mTVContextMoveUp_Click(object sender, EventArgs e)
        {
            if (ContextNode.Level == 0)
            {
                return;
            }

            if (ContextNode.Index < 1)
            {
                return;
            }

            MoveProfileNode(ContextNode, WorkingProfileGroup, -1);
        }

        /// <summary>
        /// called when the Move Down button is called.
        /// </summary>
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (tvGroup.SelectedNode.Level == 0)
            {
                return;
            }

            if (tvGroup.SelectedNode.Index > (tvGroup.SelectedNode.Parent.Nodes.Count - 2))
            {
                return;
            }

            MoveProfileNode(tvGroup.SelectedNode, WorkingProfileGroup, +1);
        }

        /// <summary>
        /// called when the Move Down context menu item is clicked.
        /// </summary>
        private void mTVContextMoveDown_Click(object sender, EventArgs e)
        {
            if (ContextNode.Level == 0)
            {
                return;
            }

            if (ContextNode.Index > (ContextNode.Parent.Nodes.Count - 2))
            {
                return;
            }

            MoveProfileNode(ContextNode, WorkingProfileGroup, +1);
        }

        private TreeNode _contextNode = null;
        /// <summary>
        /// Contains a reference to the node being selected by a right-click/context-click
        /// Is null if no context click is occurring
        /// </summary>
        protected TreeNode ContextNode
        {
            get { return _contextNode; }
            set { _contextNode = value; }
        }

        /// <summary>
        /// Called when a node is clicked.
        /// </summary>
        private void tvGroup_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Level == 0)
                {
                    mTVContextAdd.Enabled = true;
                    mTVContextDelete.Enabled = false;

                    mTVContextMoveUp.Enabled = false;
                    mTVContextMoveDown.Enabled = false;

                    mTVContextCopy.Enabled = true;
                    mTVContextCopy.Text = "&Copy Profile Group";
                    mTVContextPaste.Enabled = true;

                    mTVContextDuplicate.Enabled = false;
                }
                else
                {
                    mTVContextAdd.Enabled = true;
                    mTVContextDelete.Enabled = true;

                    mTVContextMoveUp.Enabled = ((e.Node.Parent.Nodes.Count > 1) && (e.Node.Index > 0));
                    mTVContextMoveDown.Enabled = ((e.Node.Parent.Nodes.Count > 1) && (e.Node.Index < e.Node.Parent.Nodes.Count - 1));

                    mTVContextCopy.Enabled = true;
                    mTVContextCopy.Text = "&Copy Profile";
                    mTVContextPaste.Enabled = true;

                    mTVContextDuplicate.Enabled = true;
                }

                ContextNode = e.Node;

                mTVContext.Show(tvGroup.PointToScreen(e.Location));
            }
        }

        private void groupEditor_ProfileGroupNameChanged(object sender, EventArgs e)
        {
            tvGroup.SelectedNode.Text = groupEditor.ProfileGroup.Name;
        }

        private void profileEditor_ProfileNameChanged(object sender, EventArgs e)
        {
            //keep the Default updated if neccesary
            if (WorkingProfileGroup.DefaultProfileName == tvGroup.SelectedNode.Text)
            {
                WorkingProfileGroup.DefaultProfileName = profileEditor.Profile.Name;
            }

            tvGroup.SelectedNode.Text = profileEditor.Profile.Name;
        }

        #endregion
    }
}