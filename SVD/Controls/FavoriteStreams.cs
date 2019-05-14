using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Client.SourceDiscovery;
using FutureConcepts.Media.Client.StreamViewer;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.SourceDiscoveryCommon;
using FutureConcepts.Tools;
using FutureConcepts.Settings;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class FavoriteStreams : DefaultUserControl
    {
        public FavoriteStreams()
        {
            InitializeComponent();
        }

        private List<SourceDiscoveryPlugin> _discoveryPlugins;

        /// <summary>
        /// initializes the query daemon, and loads favorite sources
        /// </summary>
        public void Initialize()
        {
            MediaApplicationSettings settings = new MediaApplicationSettings();
            settings.LoadSettings();
            _discoveryPlugins = new List<SourceDiscoveryPlugin>();
            SourceDiscoveryManager discoveryManager = SourceDiscoveryManager.Instance;
            SourceDiscoveryConfiguration sourceDiscoveryConfiguration = SourceDiscoveryConfiguration.LoadFromFile();
            foreach (SourceDiscoveryDefinition sourceDiscoveryDefinition in sourceDiscoveryConfiguration.Items)
            {
                try
                {
                    SourceDiscoveryPlugin plugin = discoveryManager.GetPlugin(sourceDiscoveryDefinition);
                    plugin.GroupChanged += new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupChanged);
                    plugin.GroupOnline += new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOnline);
                    plugin.GroupOffline += new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOffline);
                    plugin.Start();
                    _discoveryPlugins.Add(plugin);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception encountered while attempting to load source discovery protocol " + sourceDiscoveryDefinition.Name);
                    ErrorLogger.DumpToDebug(e);
                }
            }
            LoadFavorites();
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// shuts down the query daemon
        /// </summary>
        public void Shutdown()
        {
            foreach (SourceDiscoveryPlugin plugin in _discoveryPlugins)
            {
                plugin.Stop();
                plugin.GroupChanged -= new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupChanged);
                plugin.GroupOnline -= new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOnline);
                plugin.GroupOffline -= new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOffline);
            }
        }

        private void SourceDiscoveryPlugin_GroupChanged(object sender, GroupEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupChanged), new object[] { sender, e });
                return;
            }
            Debug.WriteLine("SourceDiscoveryPlugin_GroupInfoChanged");
            try
            {
                tvServerList.BeginUpdate();
                SourceDiscoveryGroup group = e.Group;
                if (group != null)
                {
                    String name = group.Name;
                    if (name != null)
                    {
                        TreeNode node = FindTreeNode(name);
                        if (node != null)
                        {
                            expandedState.Clear();
                            CacheExpansion(node);

                            TreeNode newNode = CreateGroupNode(group);
                            RemoveNode(node);
                            InsertNode(newNode, node.Index);
                            ApplyCachedExpansion(newNode);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
            finally
            {
                tvServerList.EndUpdate();
            }
        }

        private void SourceDiscoveryPlugin_GroupOnline(object sender, GroupEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOnline), new object[] { sender, e });
                return;
            }
            Debug.WriteLine("SourceDiscoveryPlugin_GroupOnline");
            if (e.Group.Name.Equals(FavoriteServer.ServerIP))
            {
                LoadFavorites();
            }
            try
            {
                tvServerList.BeginUpdate();
                AddNode(CreateGroupNode(e.Group));
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
            finally
            {
                tvServerList.EndUpdate();
            }
        }

        private void SourceDiscoveryPlugin_GroupOffline(object sender, GroupEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<GroupEventArgs>(SourceDiscoveryPlugin_GroupOffline), new object[] { sender, e });
                return;
            }
            Debug.WriteLine("SourceDiscoveryPlugin_GroupOffline");
            try
            {
                tvServerList.BeginUpdate();
                SourceDiscoveryGroup group = e.Group;
                if (group != null)
                {
                    String name = group.Name;
                    if (name != null)
                    {
                        TreeNode node = FindTreeNode(name);
                        if (node != null)
                        {
                            expandedState.Clear();
                            CacheExpansion(node);
                            RemoveNode(node);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
            finally
            {
                tvServerList.EndUpdate();
            }
        }

        #region Drag/Drop

        /// <summary>
        /// Package data from a Thumbnail and start the drag operation.
        /// </summary>
        /// <param name="sender">thumbnail they are trying to drag</param>
        void Thumbnail_DragStarted(object sender, MouseEventArgs e)
        {
            try
            {
                Thumbnail thumb = sender as Thumbnail;
                if ((thumb != null) && (DragDropManager.Data == null))
                {
                    DragDropManager.Data = new ConnectionChainDescriptor(thumb.ServerInfo, thumb.SourceInfo);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Package data from the tree view and start the drag operation
        /// </summary>
        /// <param name="sender">the treeview</param>
        /// <param name="e">the TreeNode involved</param>
        private void tvServerList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode tn = e.Item as TreeNode;
            if (tn != null)
            {
                if (tn.Tag is StreamSourceInfo)
                {
                    DragDropManager.Data = new ConnectionChainDescriptor(GetNodeServerChain(tn),
                                                                         tn.Tag as StreamSourceInfo);
                }
                else if (tn.Tag is ServerInfo || tn.Tag is SourceDiscoveryGroup)
                {
                    TreeNode sourceNode = GetFirstSourceNode(tn);
                    if (sourceNode != null) //if sourceNode is null, then there's nothing to connect to
                    {
                        DragDropManager.Data = new ConnectionChainDescriptor(GetNodeServerChain(sourceNode),
                                                                             sourceNode.Tag as StreamSourceInfo);
                    }
                }
                //else we can't drag -- error!
            }
        }

        /// <summary>
        /// Recursively walk up the nodes until we hit a topmost server node
        /// </summary>
        /// <param name="tn">tree node to start at</param>
        /// <returns>the ServerInfo associated with the specified TreeNode. Or null on error</returns>
        private List<ServerInfo> GetNodeServerChain(TreeNode tn)
        {
            List<ServerInfo> path = new List<ServerInfo>();
            BuildServerPath(path, tn);
            return path;
        }

        private void BuildServerPath(List<ServerInfo> path, TreeNode tn)
        {
            if (tn.Tag is ServerInfo)
            {
                path.Insert(0, tn.Tag as ServerInfo);
            }
            else if (tn.Tag is SourceDiscoveryGroup)
            {
                SourceDiscoveryGroup group = tn.Tag as SourceDiscoveryGroup;
                path.Insert(0, group.ServerInfo);
            }
            if(tn.Parent != null)
            {
                BuildServerPath(path, tn.Parent);
            }
        }

        /// <summary>
        /// Recursively walk down the nodes until we hit a source node
        /// </summary>
        /// <param name="tn">tree node to start at</param>
        /// <returns>returns first encountered StreamSourceInfo in the tree. Or null on error.</returns>
        private TreeNode GetFirstSourceNode(TreeNode tn)
        {
            if (tn.Tag is StreamSourceInfo)
            {
                return tn;
            }
            else if (tn.Nodes.Count > 0)
            {
                return GetFirstSourceNode(tn.Nodes[0]);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Favorite Server / Thumbnails

        private Favorites _favoriteServer;
        /// <summary>
        /// Reference to the Favorites collection that has the favorite sources and server
        /// </summary>
        private Favorites FavoriteServer
        {
            get
            {
                if (_favoriteServer == null)
                {
                    try
                    {
                        String favoritesPath = ClientPathMapper.SVDConfig("favorites.xml");
                        if (File.Exists(favoritesPath) == false)
                        {
                            favoritesPath = @"app_data\favorites.xml";
                        }
                        _favoriteServer = Favorites.LoadFromFile(favoritesPath);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.DumpToDebug(ex);
                        _favoriteServer = new Favorites();
                        _favoriteServer.Items = new List<Favorites.FavoritesItem>();
                    }
                }
                return _favoriteServer;
            }
        }

        /// <summary>
        /// Reads the favorites from the disk and displays the icons.
        /// </summary>
        /// <remarks>throws no exceptions</remarks>
        public void LoadFavorites()
        {
            if (pFavorites.InvokeRequired)
            {
                pFavorites.BeginInvoke(new Action(LoadFavorites));
                return;
            }

            try
            {
                pFavorites.Controls.Clear();
                pFavorites.Visible = false;

                if (FavoriteServer.ServerAddress == null)
                {
                    return;
                }

                using (Client.ServerConfig serverConfig = new Client.ServerConfig(FavoriteServer.ServerAddress))
                {
                    ServerInfo serverInfo = serverConfig.GetServerInfo();
                    foreach (Favorites.FavoritesItem i in FavoriteServer.Items)
                    {
                        try
                        {
                            StreamSourceInfo sourceInfo = serverInfo.StreamSources.FindSource(i.SourceName);
                            Thumbnail st = new Thumbnail();
                            st.ServerInfo = serverInfo;
                            st.SourceInfo = sourceInfo;
                            if (i.IconFilename != null)
                            {
                                st.IconFilename = i.IconFilename;
                            }
                            st.Cursor = Cursors.Hand;
                            st.DragStarted += new MouseEventHandler(Thumbnail_DragStarted);
                            DragDropManager.RegisterNonTarget(st);
                            pFavorites.Controls.Add(st);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Couldn't load source: " + i.SourceName + " > " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Debug.WriteLine("Can't contact favorite server (" + FavoriteServer.ServerAddress + "): " + ex.Message);
            }

            pFavorites.Visible = (pFavorites.Controls.Count > 0);
        }

        /// <summary>
        /// Disables the shown icons
        /// </summary>
        private void DisableFavorites()
        {
            if (pFavorites.InvokeRequired)
            {
                pFavorites.Invoke(new Action(DisableFavorites));
                return;
            }

            foreach (Thumbnail c in pFavorites.Controls)
            {
                c.Enabled = false;
            }
        }

        #endregion

        #region Tree View Helpers

        /// <summary>
        /// Locates a top-level tree node that refers to the given server address
        /// </summary>
        /// <param name="serverAddress">server address to find</param>
        /// <returns>Finds the tree node requested. Returns null if none could be found</returns>
        private TreeNode FindTreeNode(string serverAddress)
        {
            if (this.InvokeRequired)
            {
                return (TreeNode)this.Invoke(new Func<string, TreeNode>(FindTreeNode), new object[] { serverAddress });
            }

            foreach (TreeNode node in tvServerList.Nodes)
            {
                if (node.ToolTipText == serverAddress)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a TreeNode for the given server info
        /// </summary>
        /// <param name="serverInfo">server info to create a tree-node for.</param>
        /// <returns>a TreeNode with this server's info, and all of its sources under it as nodes</returns>
        private TreeNode CreateServerNode(ServerInfo serverInfo)
        {
            TreeNode serverNode = new TreeNode();
            serverNode.Text = serverInfo.ServerName;
            serverNode.ToolTipText = serverInfo.ServerAddress;
            serverNode.Tag = serverInfo;

            if (serverInfo.StreamSources.Count > 0)
            {
                foreach (StreamSourceInfo sourceInfo in serverInfo.StreamSources.Items)
                {
                    TreeNode sourceNode = new TreeNode();
                    sourceNode.Text = sourceInfo.Description;
                    sourceNode.ToolTipText = sourceInfo.SourceName;
                    sourceNode.Tag = sourceInfo;
                    serverNode.Nodes.Add(sourceNode);
                }
            }

            if (serverInfo.OriginServers != null)
            {
                foreach (ServerInfo i in serverInfo.OriginServers)
                {
                    TreeNode originServerNode = CreateServerNode(i);
                    serverNode.Nodes.Add(originServerNode);
                }
            }

            if (serverNode.Nodes.Count == 0)
            {
                serverNode.Nodes.Add(new TreeNode("<No Available Sources>") { Tag = "BOGUS" });
            }

            return serverNode;
        }

        /// <summary>
        /// Creates a TreeNode for the given group info
        /// </summary>
        /// <param name="Group">group info to create a tree-node for.</param>
        /// <returns>a TreeNode with this group's info, and all of its sources under it as nodes</returns>
        private TreeNode CreateGroupNode(SourceDiscoveryGroup group)
        {
            TreeNode result = new TreeNode();
            result.Text = group.Name;
            result.ToolTipText = group.Name;
            if (group.ServerInfo != null)
            {
                result.Tag = group.ServerInfo;
            }
            else
            {
                result.Tag = group;
            }

            if (group.Sources.Count > 0)
            {
                foreach (StreamSourceInfo source in group.Sources)
                {
                    TreeNode sourceNode = new TreeNode();
                    sourceNode.Text = source.Description;
//                    sourceNode.ToolTipText = source.ClientURL;
                    sourceNode.Tag = source;
                    result.Nodes.Add(sourceNode);
                }
            }
            return result;
        }

        /// <summary>
        /// Adds an item to the TreeView. Thread safe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">the Node property is the TreeNode added</param>
        private void AddNode(TreeNode newNode)
        {
            if (tvServerList.InvokeRequired)
            {
                tvServerList.Invoke(new Action<TreeNode>(AddNode), new object[] { newNode });
                return;
            }
            tvServerList.Nodes.Add(newNode);
        }

        /// <summary>
        /// Inserts a TreeNode in the tvServerList at the indicated index. Thread safe.
        /// </summary>
        /// <param name="newNode">TreeNode to insert</param>
        /// <param name="index">index to insert at</param>
        private void InsertNode(TreeNode newNode, int index)
        {
            if (tvServerList.InvokeRequired)
            {
                tvServerList.Invoke(new Action<TreeNode, int>(InsertNode), new object[] { newNode, index });
                return;
            }

            tvServerList.Nodes.Insert(index, newNode);
        }

        /// <summary>
        /// Removes the given TreeNode from the tvServerList. Thread safe.
        /// </summary>
        /// <param name="nodeToDelete">TreeNode to remove</param>
        private void RemoveNode(TreeNode nodeToDelete)
        {
            if (tvServerList.InvokeRequired)
            {
                tvServerList.Invoke(new Action<TreeNode>(RemoveNode), new object[] { nodeToDelete });
                return;
            }
            if (nodeToDelete != null)
            {
                tvServerList.Nodes.Remove(nodeToDelete);
            }
        }

        /// <summary>
        /// Adds a subnode to a node. Thread Safe.
        /// </summary>
        /// <param name="parent">parent node</param>
        /// <param name="subNode">sub node to add</param>
        private void AddSubNode(TreeNode parent, TreeNode subNode)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<TreeNode, TreeNode>(AddSubNode), new object[] { parent, subNode });
                return;
            }
            parent.Nodes.Add(subNode);
        }

        /// <summary>
        /// Implements manual adding of servers
        /// </summary>
        private void CM_TreeViewMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                ToolStripItem item = e.ClickedItem;
//                if (item.Text == "Add Remote")
//                {
//                    GetIP getIP = new GetIP();
//                    if (getIP.ShowDialog() == DialogResult.OK)
//                    {
//                        queryDaemon.AddServerManually(getIP.IP);
//                    }
//                }
//                else if (item.Text == "Add Local")
//                {
//                    queryDaemon.AddServerManually("127.0.0.1");
//                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show(@"Can't add server", ex.Message);
            }
        }

        #region Used to save/restore TreeNode Expanded state

        /// <summary>
        /// Used to cache expanded state of a TreeNode
        /// the List contains an index-path of those nodes that are expanded.
        /// </summary>
        List<List<int>> expandedState = new List<List<int>>();

        /// <summary>
        /// Caches the Expanded state of this node and all of its children
        /// </summary>
        /// <param name="node">node to cache expanded state of</param>
        private void CacheExpansion(TreeNode node)
        {
            if (tvServerList.InvokeRequired)
            {
                tvServerList.Invoke(new Action<TreeNode>(CacheExpansion), node);
                return;
            }

            if (node.IsExpanded)
            {
                List<int> nodePath = new List<int>();
                CalculateNodePath(node, ref nodePath);
                expandedState.Add(nodePath);
                foreach (TreeNode n in node.Nodes)
                {
                    CacheExpansion(n);
                }
            }
        }

        /// <summary>
        /// Calculates a node-path for the given node
        /// </summary>
        /// <param name="node">TreeNode to calculate index path for</param>
        /// <param name="nodePath">resulting node path. Pass in an empty list initially.</param>
        private void CalculateNodePath(TreeNode node, ref List<int> nodePath)
        {
            if (node != null)
            {
                nodePath.Insert(0, node.Index);
                CalculateNodePath(node.Parent, ref nodePath);
            }
        }

        /// <summary>
        /// Applies any cached expansion info to the given node.
        /// </summary>
        /// <param name="newNode">Node to expand as cached</param>
        private void ApplyCachedExpansion(TreeNode newNode)
        {
            if (tvServerList.InvokeRequired)
            {
                tvServerList.Invoke(new Action<TreeNode>(ApplyCachedExpansion), newNode);
                return;
            }

            foreach (List<int> path in expandedState)
            {
                TreeNode cur = tvServerList.Nodes[path[0]];
                cur.Expand();
                for (int i = 1; i < path.Count; i++)
                {
                    try
                    {
                        cur = cur.Nodes[path[i]];
                        cur.Expand();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Backing Store

        /// <summary>
        /// This class is used to load, and could be used to save, the Favorites.xml file in the app_data.
        /// </summary>
        [Serializable]
        public class Favorites
        {
            /// <summary>
            /// The ServerAddress specified in the file, may be an IP or hostname
            /// </summary>
            [XmlAttribute]
            public string ServerAddress { get; set; }

            /// <summary>
            /// Returns the string representation of the IP pointed to by ServerAddress
            /// returns null if no address is specified
            /// </summary>
            [XmlIgnore]
            public string ServerIP
            {
                get
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.ServerAddress))
                        {
                            IPHostEntry entry = Dns.GetHostEntry(this.ServerAddress);
                            return entry.AddressList[0].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            [XmlElement]
            public List<Favorites.FavoritesItem> Items { get; set; }

            /// <summary>
            /// A single source/icon favorite item.
            /// </summary>
            [Serializable]
            public class FavoritesItem
            {
                /// <summary>
                /// The source that this favorite will connect to
                /// </summary>
                [XmlAttribute]
                public string SourceName { get; set; }

                /// <summary>
                /// Path to the icon to display for this source
                /// </summary>
                [XmlAttribute]
                public string IconFilename { get; set; }
            }

            /// <summary>
            /// Loads the Favorite Server info from the given file
            /// </summary>
            /// <param name="filename">file to load from</param>
            public static Favorites LoadFromFile(string filename)
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException(@"Couldn't find favorites definition file!", filename);
                }

                XmlTextReader r = null;
                try
                {
                    XmlSerializer s = new XmlSerializer(typeof(Favorites));
                    r = new XmlTextReader(filename);
                    return (Favorites)s.Deserialize(r);
                }
                finally
                {
                    if (r != null)
                    {
                        r.Close();
                    }
                }
            }

            /// <summary>
            /// Writes the object to the given filename.
            /// </summary>
            /// <param name="filename">path to the file to write to.</param>
            public void SaveToFile(string filename)
            {
                XmlTextWriter w = null;
                try
                {
                    XmlSerializer s = new XmlSerializer(typeof(Favorites));
                    w = new XmlTextWriter(filename, null);
                    s.Serialize(w, this);
                }
                finally
                {
                    if (w != null)
                    {
                        w.Close();
                    }
                }
            }
        }

        #endregion
    }

    internal class FlickerFreeTreeView : TreeView
    {
        public FlickerFreeTreeView()
            : base()
        {
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
