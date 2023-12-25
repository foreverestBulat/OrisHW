namespace PointGame
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_signIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.enterName = new System.Windows.Forms.TextBox();
            //this.listOfUsers = new System.Windows.Forms.ListBox();
            this.listOfUsers = new System.Windows.Forms.ListView();
            this.testLabel = new System.Windows.Forms.Label();
            this.color = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_signIn
            // 
            this.btn_signIn.Location = new System.Drawing.Point(307, 227);
            this.btn_signIn.Name = "btn_signIn";
            this.btn_signIn.Size = new System.Drawing.Size(94, 29);
            this.btn_signIn.TabIndex = 0;
            this.btn_signIn.Text = "Войти";
            this.btn_signIn.UseVisualStyleBackColor = true;
            this.btn_signIn.Click += new System.EventHandler(this.btn_signIn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(278, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Введите ваше имя!";
            // 
            // enterName
            // 
            this.enterName.Location = new System.Drawing.Point(294, 182);
            this.enterName.Name = "enterName";
            this.enterName.Size = new System.Drawing.Size(125, 27);
            this.enterName.TabIndex = 2;
            //var r = new Random();
            //this.enterName.ForeColor = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
            // 
            // listOfUsers
            // 
            //this.listOfUsers.FormattingEnabled = true;
            //this.listOfUsers.ItemHeight = 20;
            this.listOfUsers.Location = new System.Drawing.Point(12, 12);
            this.listOfUsers.Name = "listOfUsers";
            this.listOfUsers.Size = new System.Drawing.Size(157, 424);
            this.listOfUsers.TabIndex = 3;
            //var r = new Random();
            //this.listOfUsers.BackColor = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
            //this.listOfUsers.ForeColor = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
            // 
            // testLabel
            // 
            this.testLabel.AutoSize = true;
            this.testLabel.Location = new System.Drawing.Point(642, 31);
            this.testLabel.Name = "testLabel";
            this.testLabel.Size = new System.Drawing.Size(0, 20);
            this.testLabel.TabIndex = 4;
            // 
            // color
            // 
            this.color.AutoSize = true;
            this.color.Location = new System.Drawing.Point(642, 69);
            this.color.Name = "color";
            this.color.Size = new System.Drawing.Size(43, 20);
            this.color.TabIndex = 5;
            this.color.Text = "color";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.color);
            this.Controls.Add(this.testLabel);
            this.Controls.Add(this.listOfUsers);
            this.Controls.Add(this.enterName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_signIn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btn_signIn;
        private Label label1;
        private TextBox enterName;
        //private ListBox listOfUsers;
        private ListView listOfUsers;
        private Label testLabel;
        private Label color;
    }
}