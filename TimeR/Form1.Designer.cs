namespace TimeR
{
    partial class form_main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_main));
            this.optionsPage = new System.Windows.Forms.TabPage();
            this.shutdownOptions = new System.Windows.Forms.GroupBox();
            this.opt_autostart = new System.Windows.Forms.CheckBox();
            this.opt_shutdown = new System.Windows.Forms.RadioButton();
            this.opt_force = new System.Windows.Forms.CheckBox();
            this.opt_reboot = new System.Windows.Forms.RadioButton();
            this.opt_energy = new System.Windows.Forms.RadioButton();
            this.timerPage = new System.Windows.Forms.TabPage();
            this.timeClock = new System.Windows.Forms.TextBox();
            this.countdownClock = new System.Windows.Forms.TextBox();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.button_close = new System.Windows.Forms.Button();
            this.optionsPage.SuspendLayout();
            this.shutdownOptions.SuspendLayout();
            this.timerPage.SuspendLayout();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsPage
            // 
            this.optionsPage.Controls.Add(this.shutdownOptions);
            this.optionsPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.optionsPage.Location = new System.Drawing.Point(4, 20);
            this.optionsPage.Name = "optionsPage";
            this.optionsPage.Padding = new System.Windows.Forms.Padding(3);
            this.optionsPage.Size = new System.Drawing.Size(125, 115);
            this.optionsPage.TabIndex = 1;
            this.optionsPage.Text = "Options";
            this.optionsPage.UseVisualStyleBackColor = true;
            // 
            // shutdownOptions
            // 
            this.shutdownOptions.Controls.Add(this.opt_autostart);
            this.shutdownOptions.Controls.Add(this.opt_shutdown);
            this.shutdownOptions.Controls.Add(this.opt_force);
            this.shutdownOptions.Controls.Add(this.opt_reboot);
            this.shutdownOptions.Controls.Add(this.opt_energy);
            this.shutdownOptions.Location = new System.Drawing.Point(6, 6);
            this.shutdownOptions.Name = "shutdownOptions";
            this.shutdownOptions.Size = new System.Drawing.Size(113, 101);
            this.shutdownOptions.TabIndex = 5;
            this.shutdownOptions.TabStop = false;
            this.shutdownOptions.Text = "Shutdown Options";
            // 
            // opt_autostart
            // 
            this.opt_autostart.AutoSize = true;
            this.opt_autostart.Location = new System.Drawing.Point(52, 81);
            this.opt_autostart.Name = "opt_autostart";
            this.opt_autostart.Size = new System.Drawing.Size(68, 17);
            this.opt_autostart.TabIndex = 5;
            this.opt_autostart.Text = "Autostart";
            this.opt_autostart.UseVisualStyleBackColor = true;
            // 
            // opt_shutdown
            // 
            this.opt_shutdown.AutoSize = true;
            this.opt_shutdown.Checked = true;
            this.opt_shutdown.Location = new System.Drawing.Point(6, 16);
            this.opt_shutdown.Name = "opt_shutdown";
            this.opt_shutdown.Size = new System.Drawing.Size(73, 17);
            this.opt_shutdown.TabIndex = 2;
            this.opt_shutdown.TabStop = true;
            this.opt_shutdown.Text = "Shutdown";
            this.opt_shutdown.UseVisualStyleBackColor = true;
            // 
            // opt_force
            // 
            this.opt_force.AutoSize = true;
            this.opt_force.Checked = true;
            this.opt_force.CheckState = System.Windows.Forms.CheckState.Checked;
            this.opt_force.Location = new System.Drawing.Point(3, 81);
            this.opt_force.Name = "opt_force";
            this.opt_force.Size = new System.Drawing.Size(53, 17);
            this.opt_force.TabIndex = 1;
            this.opt_force.Text = "Force";
            this.opt_force.UseVisualStyleBackColor = true;
            // 
            // opt_reboot
            // 
            this.opt_reboot.AutoSize = true;
            this.opt_reboot.Location = new System.Drawing.Point(6, 57);
            this.opt_reboot.Name = "opt_reboot";
            this.opt_reboot.Size = new System.Drawing.Size(60, 17);
            this.opt_reboot.TabIndex = 4;
            this.opt_reboot.Text = "Reboot";
            this.opt_reboot.UseVisualStyleBackColor = true;
            // 
            // opt_energy
            // 
            this.opt_energy.AutoSize = true;
            this.opt_energy.Location = new System.Drawing.Point(6, 37);
            this.opt_energy.Name = "opt_energy";
            this.opt_energy.Size = new System.Drawing.Size(86, 17);
            this.opt_energy.TabIndex = 3;
            this.opt_energy.Text = "Save Energy";
            this.opt_energy.UseVisualStyleBackColor = true;
            // 
            // timerPage
            // 
            this.timerPage.BackColor = System.Drawing.Color.Black;
            this.timerPage.Controls.Add(this.timeClock);
            this.timerPage.Controls.Add(this.countdownClock);
            this.timerPage.Controls.Add(this.button_stop);
            this.timerPage.Controls.Add(this.button_start);
            this.timerPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.timerPage.Location = new System.Drawing.Point(4, 20);
            this.timerPage.Name = "timerPage";
            this.timerPage.Padding = new System.Windows.Forms.Padding(3);
            this.timerPage.Size = new System.Drawing.Size(125, 115);
            this.timerPage.TabIndex = 0;
            this.timerPage.Text = "Timer";
            // 
            // timeClock
            // 
            this.timeClock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeClock.BackColor = System.Drawing.Color.Black;
            this.timeClock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeClock.Cursor = System.Windows.Forms.Cursors.Default;
            this.timeClock.Font = new System.Drawing.Font("Letter Gothic Std", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeClock.ForeColor = System.Drawing.SystemColors.Window;
            this.timeClock.Location = new System.Drawing.Point(59, 6);
            this.timeClock.Name = "timeClock";
            this.timeClock.ReadOnly = true;
            this.timeClock.Size = new System.Drawing.Size(63, 14);
            this.timeClock.TabIndex = 3;
            this.timeClock.Text = "00:00:00";
            this.timeClock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // countdownClock
            // 
            this.countdownClock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.countdownClock.BackColor = System.Drawing.Color.Black;
            this.countdownClock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.countdownClock.Font = new System.Drawing.Font("Letter Gothic Std", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countdownClock.ForeColor = System.Drawing.SystemColors.Window;
            this.countdownClock.Location = new System.Drawing.Point(5, 21);
            this.countdownClock.Name = "countdownClock";
            this.countdownClock.Size = new System.Drawing.Size(117, 30);
            this.countdownClock.TabIndex = 0;
            this.countdownClock.Text = "00:00:00";
            this.countdownClock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.countdownClock.TextChanged += new System.EventHandler(this.countdown_textchange);
            this.countdownClock.Leave += new System.EventHandler(this.countdown_leave);
            // 
            // button_stop
            // 
            this.button_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_stop.BackgroundImage = global::TimeR.Properties.Resources.icon_stop;
            this.button_stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_stop.FlatAppearance.BorderSize = 0;
            this.button_stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_stop.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_stop.Location = new System.Drawing.Point(63, 53);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(62, 62);
            this.button_stop.TabIndex = 2;
            this.button_stop.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // button_start
            // 
            this.button_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_start.BackgroundImage = global::TimeR.Properties.Resources.icon_play;
            this.button_start.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_start.FlatAppearance.BorderSize = 0;
            this.button_start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_start.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_start.Location = new System.Drawing.Point(0, 53);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(62, 62);
            this.button_start.TabIndex = 1;
            this.button_start.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.timerPage);
            this.tabs.Controls.Add(this.optionsPage);
            this.tabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Drawing.Point(5, 2);
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(133, 139);
            this.tabs.TabIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tab_index_changed);
            // 
            // button_close
            // 
            this.button_close.BackColor = System.Drawing.Color.Gainsboro;
            this.button_close.BackgroundImage = global::TimeR.Properties.Resources.icon_close;
            this.button_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_close.FlatAppearance.BorderSize = 0;
            this.button_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_close.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_close.Location = new System.Drawing.Point(127, 15);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(15, 15);
            this.button_close.TabIndex = 2;
            this.button_close.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button_close.UseVisualStyleBackColor = false;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(157, 162);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_main";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.optionsPage.ResumeLayout(false);
            this.shutdownOptions.ResumeLayout(false);
            this.shutdownOptions.PerformLayout();
            this.timerPage.ResumeLayout(false);
            this.timerPage.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage optionsPage;
        private System.Windows.Forms.RadioButton opt_reboot;
        private System.Windows.Forms.RadioButton opt_energy;
        private System.Windows.Forms.RadioButton opt_shutdown;
        private System.Windows.Forms.CheckBox opt_force;
        private System.Windows.Forms.TabPage timerPage;
        private System.Windows.Forms.TextBox countdownClock;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.TextBox timeClock;
        private System.Windows.Forms.GroupBox shutdownOptions;
        private System.Windows.Forms.CheckBox opt_autostart;
    }
}

