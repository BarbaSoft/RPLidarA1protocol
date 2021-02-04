namespace RPLidarA1Reader
{
    partial class RPLidarA1Reader
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cSerialPort = new System.Windows.Forms.ComboBox();
            this.panelmenu = new System.Windows.Forms.Panel();
            this.data1 = new System.Windows.Forms.DataGridView();
            this.Load = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cMode = new System.Windows.Forms.ComboBox();
            this.bStart = new System.Windows.Forms.Button();
            this.tserial = new System.Windows.Forms.TextBox();
            this.bConnect = new System.Windows.Forms.Button();
            this.tBaudrate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tSample = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Nr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Soglia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RealTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.config = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelmenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.data1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cSerialPort
            // 
            this.cSerialPort.FormattingEnabled = true;
            this.cSerialPort.Location = new System.Drawing.Point(91, 10);
            this.cSerialPort.Name = "cSerialPort";
            this.cSerialPort.Size = new System.Drawing.Size(95, 21);
            this.cSerialPort.TabIndex = 0;
            // 
            // panelmenu
            // 
            this.panelmenu.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panelmenu.Controls.Add(this.data1);
            this.panelmenu.Controls.Add(this.Load);
            this.panelmenu.Controls.Add(this.Save);
            this.panelmenu.Controls.Add(this.label3);
            this.panelmenu.Controls.Add(this.cMode);
            this.panelmenu.Controls.Add(this.bStart);
            this.panelmenu.Controls.Add(this.tserial);
            this.panelmenu.Controls.Add(this.bConnect);
            this.panelmenu.Controls.Add(this.tBaudrate);
            this.panelmenu.Controls.Add(this.label2);
            this.panelmenu.Controls.Add(this.label1);
            this.panelmenu.Controls.Add(this.cSerialPort);
            this.panelmenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelmenu.Location = new System.Drawing.Point(0, 0);
            this.panelmenu.Name = "panelmenu";
            this.panelmenu.Size = new System.Drawing.Size(800, 401);
            this.panelmenu.TabIndex = 1;
            // 
            // data1
            // 
            this.data1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.data1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nr,
            this.Tipo,
            this.Soglia,
            this.RealTime,
            this.config});
            this.data1.Location = new System.Drawing.Point(15, 113);
            this.data1.Name = "data1";
            this.data1.Size = new System.Drawing.Size(460, 265);
            this.data1.TabIndex = 11;
            this.data1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellClick);
            this.data1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellEndEdit);
            this.data1.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellLeave);
            // 
            // Load
            // 
            this.Load.Location = new System.Drawing.Point(713, 50);
            this.Load.Name = "Load";
            this.Load.Size = new System.Drawing.Size(75, 23);
            this.Load.TabIndex = 10;
            this.Load.Text = "Load data";
            this.Load.UseVisualStyleBackColor = true;
            this.Load.Click += new System.EventHandler(this.On_Load_Data);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(627, 50);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 9;
            this.Save.Text = "Save data";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.On_Save_Data);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(221, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Quality";
            // 
            // cMode
            // 
            this.cMode.FormattingEnabled = true;
            this.cMode.Items.AddRange(new object[] {
            "Standard",
            "Express Scan",
            "Best"});
            this.cMode.Location = new System.Drawing.Point(277, 53);
            this.cMode.Name = "cMode";
            this.cMode.Size = new System.Drawing.Size(107, 21);
            this.cMode.TabIndex = 7;
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(521, 50);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(75, 23);
            this.bStart.TabIndex = 6;
            this.bStart.Text = "Start Scan";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.OnStart);
            // 
            // tserial
            // 
            this.tserial.Location = new System.Drawing.Point(224, 12);
            this.tserial.Name = "tserial";
            this.tserial.ReadOnly = true;
            this.tserial.Size = new System.Drawing.Size(564, 20);
            this.tserial.TabIndex = 5;
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(422, 50);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(75, 23);
            this.bConnect.TabIndex = 4;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.On_Connect);
            // 
            // tBaudrate
            // 
            this.tBaudrate.Location = new System.Drawing.Point(91, 53);
            this.tBaudrate.Name = "tBaudrate";
            this.tBaudrate.Size = new System.Drawing.Size(95, 20);
            this.tBaudrate.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baud";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Serial Port";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tSample);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 401);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 29);
            this.panel1.TabIndex = 2;
            // 
            // tSample
            // 
            this.tSample.AutoSize = true;
            this.tSample.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tSample.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tSample.Location = new System.Drawing.Point(88, 7);
            this.tSample.Name = "tSample";
            this.tSample.Size = new System.Drawing.Size(15, 16);
            this.tSample.TabIndex = 4;
            this.tSample.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Sample / s";
            // 
            // Nr
            // 
            this.Nr.HeaderText = "Nr.";
            this.Nr.Name = "Nr";
            // 
            // Tipo
            // 
            this.Tipo.HeaderText = "Tipo Allarme";
            this.Tipo.Name = "Tipo";
            // 
            // Soglia
            // 
            this.Soglia.HeaderText = "Soglia Allarme";
            this.Soglia.Name = "Soglia";
            // 
            // RealTime
            // 
            this.RealTime.HeaderText = "Valore Attuale";
            this.RealTime.Name = "RealTime";
            // 
            // config
            // 
            this.config.HeaderText = "config";
            this.config.Name = "config";
            this.config.Visible = false;
            // 
            // RPLidarA1Reader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 430);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelmenu);
            this.Name = "RPLidarA1Reader";
            this.Text = "RPLidar Reader";
            this.panelmenu.ResumeLayout(false);
            this.panelmenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.data1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cSerialPort;
        private System.Windows.Forms.Panel panelmenu;
        private System.Windows.Forms.TextBox tBaudrate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.TextBox tserial;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tSample;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Load;
        private System.Windows.Forms.DataGridView data1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Soglia;
        private System.Windows.Forms.DataGridViewTextBoxColumn RealTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn config;
    }
}

