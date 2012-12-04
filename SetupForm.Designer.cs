namespace ProjectAlphaIota
{
    partial class SetupForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ModeLabel = new System.Windows.Forms.Label();
            this.pvc = new System.Windows.Forms.RadioButton();
            this.cvc = new System.Windows.Forms.RadioButton();
            this.pvp = new System.Windows.Forms.RadioButton();
            this.DifficultyLabel = new System.Windows.Forms.Label();
            this.difficultyDropDown = new System.Windows.Forms.ComboBox();
            this.ok_button = new System.Windows.Forms.Button();
            this.close_button = new System.Windows.Forms.Button();
            this.playerColorGroup = new System.Windows.Forms.GroupBox();
            this.playerColorBlack = new System.Windows.Forms.RadioButton();
            this.playerColorRed = new System.Windows.Forms.RadioButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.playerColorGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // ModeLabel
            // 
            this.ModeLabel.AutoSize = true;
            this.ModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModeLabel.Location = new System.Drawing.Point(41, 9);
            this.ModeLabel.Name = "ModeLabel";
            this.ModeLabel.Size = new System.Drawing.Size(150, 24);
            this.ModeLabel.TabIndex = 3;
            this.ModeLabel.Text = "Choose a mode.";
            this.ModeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pvc
            // 
            this.pvc.AutoSize = true;
            this.pvc.Checked = true;
            this.pvc.Location = new System.Drawing.Point(15, 37);
            this.pvc.Name = "pvc";
            this.pvc.Size = new System.Drawing.Size(120, 17);
            this.pvc.TabIndex = 4;
            this.pvc.TabStop = true;
            this.pvc.Text = "Player Vs. Computer";
            this.pvc.UseVisualStyleBackColor = true;
            this.pvc.CheckedChanged += new System.EventHandler(this.playerDifficultyChanged);
            // 
            // cvc
            // 
            this.cvc.AutoSize = true;
            this.cvc.Location = new System.Drawing.Point(15, 60);
            this.cvc.Name = "cvc";
            this.cvc.Size = new System.Drawing.Size(136, 17);
            this.cvc.TabIndex = 5;
            this.cvc.Text = "Computer Vs. Computer";
            this.cvc.UseVisualStyleBackColor = true;
            this.cvc.CheckedChanged += new System.EventHandler(this.playerDifficultyChanged);
            // 
            // pvp
            // 
            this.pvp.AutoSize = true;
            this.pvp.Location = new System.Drawing.Point(15, 83);
            this.pvp.Name = "pvp";
            this.pvp.Size = new System.Drawing.Size(104, 17);
            this.pvp.TabIndex = 6;
            this.pvp.Text = "Player Vs. Player";
            this.pvp.UseVisualStyleBackColor = true;
            this.pvp.CheckedChanged += new System.EventHandler(this.playerDifficultyChanged);
            // 
            // DifficultyLabel
            // 
            this.DifficultyLabel.AutoSize = true;
            this.DifficultyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DifficultyLabel.Location = new System.Drawing.Point(234, 9);
            this.DifficultyLabel.Name = "DifficultyLabel";
            this.DifficultyLabel.Size = new System.Drawing.Size(193, 24);
            this.DifficultyLabel.TabIndex = 11;
            this.DifficultyLabel.Text = "Choose your difficulty.";
            // 
            // difficultyDropDown
            // 
            this.difficultyDropDown.DisplayMember = "3, Hard";
            this.difficultyDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.difficultyDropDown.FormattingEnabled = true;
            this.difficultyDropDown.Items.AddRange(new object[] {
            "1, Easy",
            "2, Medium",
            "3, Hard",
            "4, Very Hard"});
            this.difficultyDropDown.SelectedIndex = 2;
            this.difficultyDropDown.Location = new System.Drawing.Point(232, 36);
            this.difficultyDropDown.Name = "difficultyDropDown";
            this.difficultyDropDown.Size = new System.Drawing.Size(195, 21);
            this.difficultyDropDown.TabIndex = 13;
            this.difficultyDropDown.SelectedIndexChanged += new System.EventHandler(this.difficultyDropDown_SelectedIndexChanged_1);
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(94, 165);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(97, 23);
            this.ok_button.TabIndex = 14;
            this.ok_button.Text = "OK";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.OkClick);
            // 
            // close_button
            // 
            this.close_button.Location = new System.Drawing.Point(232, 165);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(99, 23);
            this.close_button.TabIndex = 15;
            this.close_button.Text = "Close";
            this.close_button.UseVisualStyleBackColor = true;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // playerColorGroup
            // 
            this.playerColorGroup.Controls.Add(this.playerColorBlack);
            this.playerColorGroup.Controls.Add(this.playerColorRed);
            this.playerColorGroup.Location = new System.Drawing.Point(232, 63);
            this.playerColorGroup.Name = "playerColorGroup";
            this.playerColorGroup.Size = new System.Drawing.Size(200, 49);
            this.playerColorGroup.TabIndex = 16;
            this.playerColorGroup.TabStop = false;
            this.playerColorGroup.Text = "Choose your color";
            // 
            // playerColorBlack
            // 
            this.playerColorBlack.AutoSize = true;
            this.playerColorBlack.Location = new System.Drawing.Point(101, 19);
            this.playerColorBlack.Name = "playerColorBlack";
            this.playerColorBlack.Size = new System.Drawing.Size(52, 17);
            this.playerColorBlack.TabIndex = 1;
            this.playerColorBlack.Text = "Black";
            this.playerColorBlack.UseVisualStyleBackColor = true;
            this.playerColorBlack.CheckedChanged += new System.EventHandler(this.playerColorChanged);
            // 
            // playerColorRed
            // 
            this.playerColorRed.AutoSize = true;
            this.playerColorRed.Checked = true;
            this.playerColorRed.Location = new System.Drawing.Point(30, 19);
            this.playerColorRed.Name = "playerColorRed";
            this.playerColorRed.Size = new System.Drawing.Size(45, 17);
            this.playerColorRed.TabIndex = 0;
            this.playerColorRed.TabStop = true;
            this.playerColorRed.Text = "Red";
            this.playerColorRed.UseVisualStyleBackColor = true;
            this.playerColorRed.CheckedChanged += new System.EventHandler(this.playerColorChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(60, 128);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 17;
            this.numericUpDown1.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(265, 128);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown2.TabIndex = 18;
            this.numericUpDown2.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 19;
            this.label1.Text = "Rows";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(229, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 20;
            this.label2.Text = "Cols";
            // 
            // SetupForm
            // 
            this.ClientSize = new System.Drawing.Size(439, 196);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.playerColorGroup);
            this.Controls.Add(this.pvp);
            this.Controls.Add(this.cvc);
            this.Controls.Add(this.pvc);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.difficultyDropDown);
            this.Controls.Add(this.DifficultyLabel);
            this.Controls.Add(this.ModeLabel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(447, 223);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(447, 223);
            this.Name = "SetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setup";
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.playerColorGroup.ResumeLayout(false);
            this.playerColorGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ModeLabel;
        private System.Windows.Forms.RadioButton pvc;
        private System.Windows.Forms.RadioButton cvc;
        private System.Windows.Forms.RadioButton pvp;
        private System.Windows.Forms.Label DifficultyLabel;
        private System.Windows.Forms.ComboBox difficultyDropDown;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button close_button;
        private System.Windows.Forms.GroupBox playerColorGroup;
        private System.Windows.Forms.RadioButton playerColorBlack;
        private System.Windows.Forms.RadioButton playerColorRed;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}