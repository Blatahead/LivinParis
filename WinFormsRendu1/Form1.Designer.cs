namespace WinFormsRendu1
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
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            caselabelEstConnexe = new TableLayoutPanel();
            label2 = new Label();
            label8 = new Label();
            labelEstPondere = new Label();
            labelEstConnexe = new Label();
            caselistParcoursLargeur = new TableLayoutPanel();
            label3 = new Label();
            listParcoursLargeur = new ListBox();
            caselistParcoursProfondeur = new TableLayoutPanel();
            label4 = new Label();
            listParcoursProfondeur = new ListBox();
            pictureBox1 = new PictureBox();
            tableLayoutPanel6 = new TableLayoutPanel();
            caselabelOrdreGraphe = new TableLayoutPanel();
            label5 = new Label();
            labelOrdreGraphe = new Label();
            caselabelTailleGraphe = new TableLayoutPanel();
            label6 = new Label();
            labelTailleGraphe = new Label();
            caseLabelEstOriente = new TableLayoutPanel();
            label7 = new Label();
            LabelEstOriente = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            caselabelEstConnexe.SuspendLayout();
            caselistParcoursLargeur.SuspendLayout();
            caselistParcoursProfondeur.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tableLayoutPanel6.SuspendLayout();
            caselabelOrdreGraphe.SuspendLayout();
            caselabelTailleGraphe.SuspendLayout();
            caseLabelEstOriente.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(1518, 85);
            label1.TabIndex = 0;
            label1.Text = "Association Karaté";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel6, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 19.25926F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80.74074F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 159F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.Size = new Size(1524, 641);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 509F));
            tableLayoutPanel2.Controls.Add(caselabelEstConnexe, 0, 0);
            tableLayoutPanel2.Controls.Add(caselistParcoursLargeur, 1, 0);
            tableLayoutPanel2.Controls.Add(caselistParcoursProfondeur, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 445);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(1518, 153);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // caselabelEstConnexe
            // 
            caselabelEstConnexe.ColumnCount = 2;
            caselabelEstConnexe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelEstConnexe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelEstConnexe.Controls.Add(label2, 0, 0);
            caselabelEstConnexe.Controls.Add(label8, 0, 1);
            caselabelEstConnexe.Controls.Add(labelEstPondere, 1, 1);
            caselabelEstConnexe.Controls.Add(labelEstConnexe, 1, 0);
            caselabelEstConnexe.Dock = DockStyle.Fill;
            caselabelEstConnexe.Location = new Point(3, 3);
            caselabelEstConnexe.Name = "caselabelEstConnexe";
            caselabelEstConnexe.RowCount = 2;
            caselabelEstConnexe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            caselabelEstConnexe.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            caselabelEstConnexe.Size = new Size(498, 147);
            caselabelEstConnexe.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(243, 69);
            label2.TabIndex = 0;
            label2.Text = "Le graphe est connexe";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(3, 69);
            label8.Name = "label8";
            label8.Size = new Size(243, 78);
            label8.TabIndex = 1;
            label8.Text = "Le graphe est pondéré";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelEstPondere
            // 
            labelEstPondere.AutoSize = true;
            labelEstPondere.Dock = DockStyle.Fill;
            labelEstPondere.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelEstPondere.Location = new Point(252, 69);
            labelEstPondere.Name = "labelEstPondere";
            labelEstPondere.Size = new Size(243, 78);
            labelEstPondere.TabIndex = 2;
            labelEstPondere.Text = "label9";
            labelEstPondere.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelEstConnexe
            // 
            labelEstConnexe.AutoSize = true;
            labelEstConnexe.Dock = DockStyle.Fill;
            labelEstConnexe.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelEstConnexe.Location = new Point(252, 0);
            labelEstConnexe.Name = "labelEstConnexe";
            labelEstConnexe.Size = new Size(243, 69);
            labelEstConnexe.TabIndex = 3;
            labelEstConnexe.Text = "label9";
            labelEstConnexe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // caselistParcoursLargeur
            // 
            caselistParcoursLargeur.ColumnCount = 1;
            caselistParcoursLargeur.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselistParcoursLargeur.Controls.Add(label3, 0, 0);
            caselistParcoursLargeur.Controls.Add(listParcoursLargeur, 0, 1);
            caselistParcoursLargeur.Dock = DockStyle.Fill;
            caselistParcoursLargeur.Location = new Point(507, 3);
            caselistParcoursLargeur.Name = "caselistParcoursLargeur";
            caselistParcoursLargeur.RowCount = 2;
            caselistParcoursLargeur.RowStyles.Add(new RowStyle(SizeType.Percent, 48.97959F));
            caselistParcoursLargeur.RowStyles.Add(new RowStyle(SizeType.Percent, 51.02041F));
            caselistParcoursLargeur.Size = new Size(498, 147);
            caselistParcoursLargeur.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(492, 72);
            label3.TabIndex = 0;
            label3.Text = "Parcours en largeur";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listParcoursLargeur
            // 
            listParcoursLargeur.ColumnWidth = 30;
            listParcoursLargeur.Dock = DockStyle.Fill;
            listParcoursLargeur.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listParcoursLargeur.FormattingEnabled = true;
            listParcoursLargeur.HorizontalScrollbar = true;
            listParcoursLargeur.IntegralHeight = false;
            listParcoursLargeur.Location = new Point(3, 75);
            listParcoursLargeur.MultiColumn = true;
            listParcoursLargeur.Name = "listParcoursLargeur";
            listParcoursLargeur.ScrollAlwaysVisible = true;
            listParcoursLargeur.Size = new Size(492, 69);
            listParcoursLargeur.TabIndex = 1;
            // 
            // caselistParcoursProfondeur
            // 
            caselistParcoursProfondeur.ColumnCount = 1;
            caselistParcoursProfondeur.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselistParcoursProfondeur.Controls.Add(label4, 0, 0);
            caselistParcoursProfondeur.Controls.Add(listParcoursProfondeur, 0, 1);
            caselistParcoursProfondeur.Dock = DockStyle.Fill;
            caselistParcoursProfondeur.Location = new Point(1011, 3);
            caselistParcoursProfondeur.Name = "caselistParcoursProfondeur";
            caselistParcoursProfondeur.RowCount = 2;
            caselistParcoursProfondeur.RowStyles.Add(new RowStyle(SizeType.Percent, 49.6598625F));
            caselistParcoursProfondeur.RowStyles.Add(new RowStyle(SizeType.Percent, 50.3401375F));
            caselistParcoursProfondeur.Size = new Size(504, 147);
            caselistParcoursProfondeur.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 0);
            label4.Name = "label4";
            label4.Size = new Size(498, 73);
            label4.TabIndex = 0;
            label4.Text = "Parcours en profondeur";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listParcoursProfondeur
            // 
            listParcoursProfondeur.ColumnWidth = 30;
            listParcoursProfondeur.Dock = DockStyle.Fill;
            listParcoursProfondeur.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listParcoursProfondeur.FormattingEnabled = true;
            listParcoursProfondeur.HorizontalScrollbar = true;
            listParcoursProfondeur.IntegralHeight = false;
            listParcoursProfondeur.Location = new Point(3, 76);
            listParcoursProfondeur.MultiColumn = true;
            listParcoursProfondeur.Name = "listParcoursProfondeur";
            listParcoursProfondeur.ScrollAlwaysVisible = true;
            listParcoursProfondeur.Size = new Size(498, 68);
            listParcoursProfondeur.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.Capture_d_écran_2023_06_29_174735;
            pictureBox1.Location = new Point(3, 88);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1518, 351);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 3;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 526F));
            tableLayoutPanel6.Controls.Add(caselabelOrdreGraphe, 0, 0);
            tableLayoutPanel6.Controls.Add(caselabelTailleGraphe, 1, 0);
            tableLayoutPanel6.Controls.Add(caseLabelEstOriente, 2, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 604);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(1518, 34);
            tableLayoutPanel6.TabIndex = 3;
            // 
            // caselabelOrdreGraphe
            // 
            caselabelOrdreGraphe.ColumnCount = 2;
            caselabelOrdreGraphe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelOrdreGraphe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelOrdreGraphe.Controls.Add(label5, 0, 0);
            caselabelOrdreGraphe.Controls.Add(labelOrdreGraphe, 1, 0);
            caselabelOrdreGraphe.Dock = DockStyle.Fill;
            caselabelOrdreGraphe.Location = new Point(3, 3);
            caselabelOrdreGraphe.Name = "caselabelOrdreGraphe";
            caselabelOrdreGraphe.RowCount = 1;
            caselabelOrdreGraphe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            caselabelOrdreGraphe.Size = new Size(490, 28);
            caselabelOrdreGraphe.TabIndex = 0;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(239, 28);
            label5.TabIndex = 0;
            label5.Text = "Ordre du graphe :";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelOrdreGraphe
            // 
            labelOrdreGraphe.AutoSize = true;
            labelOrdreGraphe.Dock = DockStyle.Fill;
            labelOrdreGraphe.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelOrdreGraphe.Location = new Point(248, 0);
            labelOrdreGraphe.Name = "labelOrdreGraphe";
            labelOrdreGraphe.Size = new Size(239, 28);
            labelOrdreGraphe.TabIndex = 1;
            labelOrdreGraphe.Text = "label9";
            labelOrdreGraphe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // caselabelTailleGraphe
            // 
            caselabelTailleGraphe.ColumnCount = 2;
            caselabelTailleGraphe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelTailleGraphe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caselabelTailleGraphe.Controls.Add(label6, 0, 0);
            caselabelTailleGraphe.Controls.Add(labelTailleGraphe, 1, 0);
            caselabelTailleGraphe.Dock = DockStyle.Fill;
            caselabelTailleGraphe.Location = new Point(499, 3);
            caselabelTailleGraphe.Name = "caselabelTailleGraphe";
            caselabelTailleGraphe.RowCount = 1;
            caselabelTailleGraphe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            caselabelTailleGraphe.Size = new Size(490, 28);
            caselabelTailleGraphe.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(3, 0);
            label6.Name = "label6";
            label6.Size = new Size(239, 28);
            label6.TabIndex = 0;
            label6.Text = "Taille du graphe :";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTailleGraphe
            // 
            labelTailleGraphe.AutoSize = true;
            labelTailleGraphe.Dock = DockStyle.Fill;
            labelTailleGraphe.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTailleGraphe.Location = new Point(248, 0);
            labelTailleGraphe.Name = "labelTailleGraphe";
            labelTailleGraphe.Size = new Size(239, 28);
            labelTailleGraphe.TabIndex = 1;
            labelTailleGraphe.Text = "label9";
            labelTailleGraphe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // caseLabelEstOriente
            // 
            caseLabelEstOriente.ColumnCount = 2;
            caseLabelEstOriente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caseLabelEstOriente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            caseLabelEstOriente.Controls.Add(label7, 0, 0);
            caseLabelEstOriente.Controls.Add(LabelEstOriente, 1, 0);
            caseLabelEstOriente.Dock = DockStyle.Fill;
            caseLabelEstOriente.Location = new Point(995, 3);
            caseLabelEstOriente.Name = "caseLabelEstOriente";
            caseLabelEstOriente.RowCount = 1;
            caseLabelEstOriente.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            caseLabelEstOriente.Size = new Size(520, 28);
            caseLabelEstOriente.TabIndex = 2;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Fill;
            label7.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(3, 0);
            label7.Name = "label7";
            label7.Size = new Size(254, 28);
            label7.TabIndex = 0;
            label7.Text = "Le graphe est orienté :";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelEstOriente
            // 
            LabelEstOriente.AutoSize = true;
            LabelEstOriente.Dock = DockStyle.Fill;
            LabelEstOriente.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LabelEstOriente.Location = new Point(263, 0);
            LabelEstOriente.Name = "LabelEstOriente";
            LabelEstOriente.Size = new Size(254, 28);
            LabelEstOriente.TabIndex = 1;
            LabelEstOriente.Text = "label9";
            LabelEstOriente.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1524, 641);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            caselabelEstConnexe.ResumeLayout(false);
            caselabelEstConnexe.PerformLayout();
            caselistParcoursLargeur.ResumeLayout(false);
            caselistParcoursLargeur.PerformLayout();
            caselistParcoursProfondeur.ResumeLayout(false);
            caselistParcoursProfondeur.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tableLayoutPanel6.ResumeLayout(false);
            caselabelOrdreGraphe.ResumeLayout(false);
            caselabelOrdreGraphe.PerformLayout();
            caselabelTailleGraphe.ResumeLayout(false);
            caselabelTailleGraphe.PerformLayout();
            caseLabelEstOriente.ResumeLayout(false);
            caseLabelEstOriente.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel caselabelEstConnexe;
        private Label label2;
        private TableLayoutPanel caselistParcoursLargeur;
        private Label label3;
        private PictureBox pictureBox1;
        private TableLayoutPanel caselistParcoursProfondeur;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel caselabelOrdreGraphe;
        private Label label5;
        private TableLayoutPanel caselabelTailleGraphe;
        private TableLayoutPanel caseLabelEstOriente;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label labelOrdreGraphe;
        private Label labelTailleGraphe;
        private Label LabelEstOriente;
        private Label labelEstPondere;
        private Label labelEstConnexe;
        private ListBox listParcoursLargeur;
        private ListBox listParcoursProfondeur;
    }
}
