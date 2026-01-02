namespace WinFormsAppMicrophone
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
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            trackBar1 = new TrackBar();
            button2 = new Button();
            comboBox1 = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            linkLabel1 = new LinkLabel();
            label4 = new Label();
            textBox2 = new TextBox();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(305, 150);
            button1.Name = "button1";
            button1.Size = new Size(159, 23);
            button1.TabIndex = 0;
            button1.Text = "Подключиться";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(268, 111);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(257, 23);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(254, 264);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 2;
            label1.Text = "Состояние:";
            // 
            // trackBar1
            // 
            trackBar1.LargeChange = 1;
            trackBar1.Location = new Point(220, 216);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(325, 45);
            trackBar1.TabIndex = 3;
            trackBar1.Value = 50;
            // 
            // button2
            // 
            button2.Location = new Point(305, 179);
            button2.Name = "button2";
            button2.Size = new Size(159, 23);
            button2.TabIndex = 4;
            button2.Text = "Отсоедениться ";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(268, 70);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(257, 23);
            comboBox1.TabIndex = 5;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 114);
            label2.Name = "label2";
            label2.Size = new Size(127, 15);
            label2.TabIndex = 6;
            label2.Text = "Введите IP устройства";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(137, 73);
            label3.Name = "label3";
            label3.Size = new Size(125, 15);
            label3.TabIndex = 7;
            label3.Text = "Выберете устройство";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(339, 422);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(162, 15);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://vb-audio.com/Cable/";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 422);
            label4.Name = "label4";
            label4.Size = new Size(324, 15);
            label4.TabIndex = 9;
            label4.Text = "Установите vb audio драйвера для работы с программой.";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(268, 32);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(257, 23);
            textBox2.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(135, 35);
            label5.Name = "label5";
            label5.Size = new Size(125, 15);
            label5.TabIndex = 11;
            label5.Text = "IP вашего устройства";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(textBox2);
            Controls.Add(label4);
            Controls.Add(linkLabel1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(comboBox1);
            Controls.Add(button2);
            Controls.Add(trackBar1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private TrackBar trackBar1;
        private Button button2;
        private ComboBox comboBox1;
        private Label label2;
        private Label label3;
        private LinkLabel linkLabel1;
        private Label label4;
        private TextBox textBox2;
        private Label label5;
    }
}
