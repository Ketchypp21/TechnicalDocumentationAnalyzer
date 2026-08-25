namespace TechnicalDocumentationAnalyzer;

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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        topPanel = new System.Windows.Forms.Panel();
        recordsCountLabel = new System.Windows.Forms.Label();
        fileNameLabel = new System.Windows.Forms.Label();
        searchTextBox = new System.Windows.Forms.TextBox();
        filterValueComboBox = new System.Windows.Forms.ComboBox();
        filterColumnComboBox = new System.Windows.Forms.ComboBox();
        resetFiltersButton = new System.Windows.Forms.Button();
        valueLabel = new System.Windows.Forms.Label();
        columnLabel = new System.Windows.Forms.Label();
        searchLabel = new System.Windows.Forms.Label();
        loadButton = new System.Windows.Forms.Button();
        documentsGrid = new System.Windows.Forms.DataGridView();
        topPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)documentsGrid).BeginInit();
        SuspendLayout();
        // 
        // topPanel
        // 
        topPanel.Controls.Add(recordsCountLabel);
        topPanel.Controls.Add(fileNameLabel);
        topPanel.Controls.Add(searchTextBox);
        topPanel.Controls.Add(filterValueComboBox);
        topPanel.Controls.Add(filterColumnComboBox);
        topPanel.Controls.Add(resetFiltersButton);
        topPanel.Controls.Add(valueLabel);
        topPanel.Controls.Add(columnLabel);
        topPanel.Controls.Add(searchLabel);
        topPanel.Controls.Add(loadButton);
        topPanel.Dock = System.Windows.Forms.DockStyle.Top;
        topPanel.Location = new System.Drawing.Point(0, 0);
        topPanel.Name = "topPanel";
        topPanel.Padding = new System.Windows.Forms.Padding(10);
        topPanel.Size = new System.Drawing.Size(1184, 110);
        topPanel.TabIndex = 0;
        // 
        // recordsCountLabel
        // 
        recordsCountLabel.Location = new System.Drawing.Point(265, 23);
        recordsCountLabel.Name = "recordsCountLabel";
        recordsCountLabel.Size = new System.Drawing.Size(100, 19);
        recordsCountLabel.TabIndex = 13;
        recordsCountLabel.Text = "Записей: 0";
        // 
        // fileNameLabel
        // 
        fileNameLabel.Location = new System.Drawing.Point(159, 23);
        fileNameLabel.Name = "fileNameLabel";
        fileNameLabel.Size = new System.Drawing.Size(100, 19);
        fileNameLabel.TabIndex = 12;
        fileNameLabel.Text = "Файл не выбран";
        // 
        // searchTextBox
        // 
        searchTextBox.Location = new System.Drawing.Point(71, 66);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new System.Drawing.Size(182, 23);
        searchTextBox.TabIndex = 11;
        // 
        // filterValueComboBox
        // 
        filterValueComboBox.FormattingEnabled = true;
        filterValueComboBox.Location = new System.Drawing.Point(584, 66);
        filterValueComboBox.Name = "filterValueComboBox";
        filterValueComboBox.Size = new System.Drawing.Size(186, 23);
        filterValueComboBox.TabIndex = 10;
        // 
        // filterColumnComboBox
        // 
        filterColumnComboBox.FormattingEnabled = true;
        filterColumnComboBox.Location = new System.Drawing.Point(322, 66);
        filterColumnComboBox.Name = "filterColumnComboBox";
        filterColumnComboBox.Size = new System.Drawing.Size(187, 23);
        filterColumnComboBox.TabIndex = 9;
        // 
        // resetFiltersButton
        // 
        resetFiltersButton.Location = new System.Drawing.Point(789, 59);
        resetFiltersButton.Name = "resetFiltersButton";
        resetFiltersButton.Size = new System.Drawing.Size(130, 35);
        resetFiltersButton.TabIndex = 7;
        resetFiltersButton.Text = "Сбросить";
        resetFiltersButton.UseVisualStyleBackColor = true;
        // 
        // valueLabel
        // 
        valueLabel.AutoSize = true;
        valueLabel.Location = new System.Drawing.Point(515, 72);
        valueLabel.Name = "valueLabel";
        valueLabel.Size = new System.Drawing.Size(63, 15);
        valueLabel.TabIndex = 5;
        valueLabel.Text = "Значение:";
        // 
        // columnLabel
        // 
        columnLabel.AutoSize = true;
        columnLabel.Location = new System.Drawing.Point(259, 72);
        columnLabel.Name = "columnLabel";
        columnLabel.Size = new System.Drawing.Size(57, 15);
        columnLabel.TabIndex = 3;
        columnLabel.Text = "Колонка:";
        // 
        // searchLabel
        // 
        searchLabel.AutoSize = true;
        searchLabel.Location = new System.Drawing.Point(23, 72);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new System.Drawing.Size(45, 15);
        searchLabel.TabIndex = 1;
        searchLabel.Text = "Поиск:";
        // 
        // loadButton
        // 
        loadButton.Location = new System.Drawing.Point(23, 13);
        loadButton.Name = "loadButton";
        loadButton.Size = new System.Drawing.Size(130, 35);
        loadButton.TabIndex = 0;
        loadButton.Text = "Загрузить CSV";
        loadButton.UseVisualStyleBackColor = true;
        // 
        // documentsGrid
        // 
        documentsGrid.AllowUserToAddRows = false;
        documentsGrid.AllowUserToDeleteRows = false;
        documentsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        documentsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        documentsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        documentsGrid.Location = new System.Drawing.Point(0, 110);
        documentsGrid.Name = "documentsGrid";
        documentsGrid.ReadOnly = true;
        documentsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        documentsGrid.Size = new System.Drawing.Size(1184, 571);
        documentsGrid.TabIndex = 1;
        documentsGrid.Text = "dataGridView1";
        documentsGrid.VirtualMode = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1184, 681);
        Controls.Add(documentsGrid);
        Controls.Add(topPanel);
        MinimumSize = new System.Drawing.Size(1000, 600);
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "Анализ технической документации";
        topPanel.ResumeLayout(false);
        topPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)documentsGrid).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label recordsCountLabel;

    private System.Windows.Forms.Label fileNameLabel;

    private System.Windows.Forms.ComboBox filterColumnComboBox;
    private System.Windows.Forms.ComboBox filterValueComboBox;
    private System.Windows.Forms.TextBox searchTextBox;

    private System.Windows.Forms.Button resetFiltersButton;

    private System.Windows.Forms.Label valueLabel;

    private System.Windows.Forms.Label columnLabel;

    private System.Windows.Forms.Label searchLabel;

    private System.Windows.Forms.Button loadButton;

    private System.Windows.Forms.DataGridView documentsGrid;

    private System.Windows.Forms.Panel topPanel;

    #endregion
}