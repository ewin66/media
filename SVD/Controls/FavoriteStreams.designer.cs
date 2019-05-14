using FutureConcepts.Media.Client.StreamViewer;

namespace FutureConcepts.Media.SVD.Controls
{
    partial class FavoriteStreams
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pStreamBrowser = new System.Windows.Forms.Panel();
            this.tvServerList = new FlickerFreeTreeView();
            this.CM_TreeViewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.pFavorites = new System.Windows.Forms.TableLayoutPanel();
            this.pStreamBrowser.SuspendLayout();
            this.CM_TreeViewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pStreamBrowser
            // 
            this.pStreamBrowser.AutoScroll = true;
            this.pStreamBrowser.BackColor = System.Drawing.Color.Black;
            this.pStreamBrowser.Controls.Add(this.tvServerList);
            this.pStreamBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pStreamBrowser.Location = new System.Drawing.Point(0, 0);
            this.pStreamBrowser.Name = "pStreamBrowser";
            this.pStreamBrowser.Padding = new System.Windows.Forms.Padding(1);
            this.pStreamBrowser.Size = new System.Drawing.Size(176, 512);
            this.pStreamBrowser.TabIndex = 0;
            // 
            // tvServerList
            // 
            this.tvServerList.BackColor = System.Drawing.Color.Black;
            this.tvServerList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvServerList.ContextMenuStrip = this.CM_TreeViewMenu;
            this.tvServerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvServerList.ForeColor = System.Drawing.Color.White;
            this.tvServerList.Location = new System.Drawing.Point(1, 1);
            this.tvServerList.Name = "tvServerList";
            this.tvServerList.ShowNodeToolTips = true;
            this.tvServerList.Size = new System.Drawing.Size(174, 510);
            this.tvServerList.TabIndex = 0;
            this.tvServerList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvServerList_ItemDrag);
            // 
            // CM_TreeViewMenu
            // 
            this.CM_TreeViewMenu.BackColor = System.Drawing.Color.DimGray;
            this.CM_TreeViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.CM_TreeViewMenu.Name = "CM_TreeViewMenu";
            this.CM_TreeViewMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.CM_TreeViewMenu.Size = new System.Drawing.Size(145, 48);
            this.CM_TreeViewMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.CM_TreeViewMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItem1.Text = "Add Remote";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItem2.Text = "Add Local";
            // 
            // pFavorites
            // 
            this.pFavorites.AutoSize = true;
            this.pFavorites.BackColor = System.Drawing.Color.Black;
            this.pFavorites.ColumnCount = 2;
            this.pFavorites.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pFavorites.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pFavorites.Dock = System.Windows.Forms.DockStyle.Top;
            this.pFavorites.Location = new System.Drawing.Point(0, 0);
            this.pFavorites.Name = "pFavorites";
            this.pFavorites.RowCount = 1;
            this.pFavorites.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pFavorites.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pFavorites.Size = new System.Drawing.Size(176, 0);
            this.pFavorites.TabIndex = 1;
            // 
            // FavoriteStreams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.pStreamBrowser);
            this.Controls.Add(this.pFavorites);
            this.Name = "FavoriteStreams";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(176, 512);
            this.pStreamBrowser.ResumeLayout(false);
            this.CM_TreeViewMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pStreamBrowser;
        private FlickerFreeTreeView tvServerList;
        private System.Windows.Forms.ContextMenuStrip CM_TreeViewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.TableLayoutPanel pFavorites;
    }
}
