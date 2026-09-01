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
        encodingComboBox = new System.Windows.Forms.ComboBox();
        encodingLabel = new System.Windows.Forms.Label();
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
        mainTabControl = new System.Windows.Forms.TabControl();
        dataTabPage = new System.Windows.Forms.TabPage();
        statisticsTabPage = new System.Windows.Forms.TabPage();
        statisticsSplitContainer = new System.Windows.Forms.SplitContainer();
        statisticsGrid = new System.Windows.Forms.DataGridView();
        statisticsChartPanel = new System.Windows.Forms.Panel();
        statisticsTopPanel = new System.Windows.Forms.Panel();
        statisticsDetailsLabel = new System.Windows.Forms.Label();
        statisticsModeComboBox = new System.Windows.Forms.ComboBox();
        statisticsModeLabel = new System.Windows.Forms.Label();
        statisticsFilterLabel = new System.Windows.Forms.Label();
        statisticsEmptyLabel = new System.Windows.Forms.Label();
        statisticsUniqueLabel = new System.Windows.Forms.Label();
        statisticsTotalLabel = new System.Windows.Forms.Label();
        statisticsColumnComboBox = new System.Windows.Forms.ComboBox();
        statisticsColumnLabel = new System.Windows.Forms.Label();
        topPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)documentsGrid).BeginInit();
        mainTabControl.SuspendLayout();
        dataTabPage.SuspendLayout();
        statisticsTabPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)statisticsSplitContainer).BeginInit();
        statisticsSplitContainer.Panel1.SuspendLayout();
        statisticsSplitContainer.Panel2.SuspendLayout();
        statisticsSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)statisticsGrid).BeginInit();
        statisticsTopPanel.SuspendLayout();
        SuspendLayout();
        // 
        // topPanel
        // 
        topPanel.Controls.Add(encodingComboBox);
        topPanel.Controls.Add(encodingLabel);
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
        // encodingComboBox
        // 
        encodingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        encodingComboBox.FormattingEnabled = true;
        encodingComboBox.Location = new System.Drawing.Point(98, 19);
        encodingComboBox.Name = "encodingComboBox";
        encodingComboBox.Size = new System.Drawing.Size(411, 23);
        encodingComboBox.TabIndex = 15;
        // 
        // encodingLabel
        // 
        encodingLabel.AutoSize = true;
        encodingLabel.Location = new System.Drawing.Point(23, 22);
        encodingLabel.Name = "encodingLabel";
        encodingLabel.Size = new System.Drawing.Size(69, 15);
        encodingLabel.TabIndex = 14;
        encodingLabel.Text = "Кодировка:";
        // 
        // recordsCountLabel
        // 
        recordsCountLabel.AutoSize = true;
        recordsCountLabel.Location = new System.Drawing.Point(757, 22);
        recordsCountLabel.Name = "recordsCountLabel";
        recordsCountLabel.Size = new System.Drawing.Size(65, 15);
        recordsCountLabel.TabIndex = 13;
        recordsCountLabel.Text = "Записей: 0";
        // 
        // fileNameLabel
        // 
        fileNameLabel.AutoSize = true;
        fileNameLabel.Location = new System.Drawing.Point(651, 22);
        fileNameLabel.Name = "fileNameLabel";
        fileNameLabel.Size = new System.Drawing.Size(97, 15);
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
        loadButton.Location = new System.Drawing.Point(515, 12);
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
        documentsGrid.Location = new System.Drawing.Point(3, 3);
        documentsGrid.Name = "documentsGrid";
        documentsGrid.ReadOnly = true;
        documentsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        documentsGrid.Size = new System.Drawing.Size(1170, 537);
        documentsGrid.TabIndex = 1;
        documentsGrid.Text = "dataGridView1";
        documentsGrid.VirtualMode = true;
        // 
        // mainTabControl
        // 
        mainTabControl.AccessibleDescription = "";
        mainTabControl.AccessibleName = "";
        mainTabControl.Controls.Add(dataTabPage);
        mainTabControl.Controls.Add(statisticsTabPage);
        mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        mainTabControl.Location = new System.Drawing.Point(0, 110);
        mainTabControl.Name = "mainTabControl";
        mainTabControl.SelectedIndex = 0;
        mainTabControl.Size = new System.Drawing.Size(1184, 571);
        mainTabControl.TabIndex = 3;
        mainTabControl.Tag = "";
        // 
        // dataTabPage
        // 
        dataTabPage.Controls.Add(documentsGrid);
        dataTabPage.Location = new System.Drawing.Point(4, 24);
        dataTabPage.Name = "dataTabPage";
        dataTabPage.Padding = new System.Windows.Forms.Padding(3);
        dataTabPage.Size = new System.Drawing.Size(1176, 543);
        dataTabPage.TabIndex = 0;
        dataTabPage.Text = "Данные";
        dataTabPage.UseVisualStyleBackColor = true;
        // 
        // statisticsTabPage
        // 
        statisticsTabPage.Controls.Add(statisticsSplitContainer);
        statisticsTabPage.Controls.Add(statisticsTopPanel);
        statisticsTabPage.Location = new System.Drawing.Point(4, 24);
        statisticsTabPage.Name = "statisticsTabPage";
        statisticsTabPage.Padding = new System.Windows.Forms.Padding(3);
        statisticsTabPage.Size = new System.Drawing.Size(1176, 543);
        statisticsTabPage.TabIndex = 1;
        statisticsTabPage.Text = "Статистика";
        statisticsTabPage.UseVisualStyleBackColor = true;
        // 
        // statisticsSplitContainer
        // 
        statisticsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        statisticsSplitContainer.Location = new System.Drawing.Point(3, 103);
        statisticsSplitContainer.Name = "statisticsSplitContainer";
        // 
        // statisticsSplitContainer.Panel1
        // 
        statisticsSplitContainer.Panel1.Controls.Add(statisticsGrid);
        // 
        // statisticsSplitContainer.Panel2
        // 
        statisticsSplitContainer.Panel2.Controls.Add(statisticsChartPanel);
        statisticsSplitContainer.Size = new System.Drawing.Size(1170, 437);
        statisticsSplitContainer.SplitterDistance = 570;
        statisticsSplitContainer.TabIndex = 1;
        statisticsSplitContainer.Text = "splitContainer1";
        // 
        // statisticsGrid
        // 
        statisticsGrid.AllowUserToAddRows = false;
        statisticsGrid.AllowUserToDeleteRows = false;
        statisticsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        statisticsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
        statisticsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        statisticsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        statisticsGrid.Location = new System.Drawing.Point(0, 0);
        statisticsGrid.Name = "statisticsGrid";
        statisticsGrid.ReadOnly = true;
        statisticsGrid.RowHeadersVisible = false;
        statisticsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        statisticsGrid.Size = new System.Drawing.Size(570, 437);
        statisticsGrid.TabIndex = 0;
        statisticsGrid.Text = "dataGridView1";
        // 
        // statisticsChartPanel
        // 
        statisticsChartPanel.BackColor = System.Drawing.Color.White;
        statisticsChartPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        statisticsChartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        statisticsChartPanel.Location = new System.Drawing.Point(0, 0);
        statisticsChartPanel.Name = "statisticsChartPanel";
        statisticsChartPanel.Size = new System.Drawing.Size(596, 437);
        statisticsChartPanel.TabIndex = 0;
        // 
        // statisticsTopPanel
        // 
        statisticsTopPanel.Controls.Add(statisticsDetailsLabel);
        statisticsTopPanel.Controls.Add(statisticsModeComboBox);
        statisticsTopPanel.Controls.Add(statisticsModeLabel);
        statisticsTopPanel.Controls.Add(statisticsFilterLabel);
        statisticsTopPanel.Controls.Add(statisticsEmptyLabel);
        statisticsTopPanel.Controls.Add(statisticsUniqueLabel);
        statisticsTopPanel.Controls.Add(statisticsTotalLabel);
        statisticsTopPanel.Controls.Add(statisticsColumnComboBox);
        statisticsTopPanel.Controls.Add(statisticsColumnLabel);
        statisticsTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
        statisticsTopPanel.Location = new System.Drawing.Point(3, 3);
        statisticsTopPanel.Name = "statisticsTopPanel";
        statisticsTopPanel.Size = new System.Drawing.Size(1170, 100);
        statisticsTopPanel.TabIndex = 0;
        // 
        // statisticsDetailsLabel
        // 
        statisticsDetailsLabel.AutoSize = true;
        statisticsDetailsLabel.Location = new System.Drawing.Point(481, 40);
        statisticsDetailsLabel.Name = "statisticsDetailsLabel";
        statisticsDetailsLabel.Size = new System.Drawing.Size(89, 15);
        statisticsDetailsLabel.TabIndex = 8;
        statisticsDetailsLabel.Text = "Тип данных: —";
        // 
        // statisticsModeComboBox
        // 
        statisticsModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        statisticsModeComboBox.FormattingEnabled = true;
        statisticsModeComboBox.Location = new System.Drawing.Point(315, 37);
        statisticsModeComboBox.Name = "statisticsModeComboBox";
        statisticsModeComboBox.Size = new System.Drawing.Size(150, 23);
        statisticsModeComboBox.TabIndex = 7;
        // 
        // statisticsModeLabel
        // 
        statisticsModeLabel.AutoSize = true;
        statisticsModeLabel.Location = new System.Drawing.Point(214, 40);
        statisticsModeLabel.Name = "statisticsModeLabel";
        statisticsModeLabel.Size = new System.Drawing.Size(79, 15);
        statisticsModeLabel.TabIndex = 6;
        statisticsModeLabel.Text = "Вид графика:";
        // 
        // statisticsFilterLabel
        // 
        statisticsFilterLabel.AutoSize = true;
        statisticsFilterLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        statisticsFilterLabel.Location = new System.Drawing.Point(5, 40);
        statisticsFilterLabel.Name = "statisticsFilterLabel";
        statisticsFilterLabel.Size = new System.Drawing.Size(184, 15);
        statisticsFilterLabel.TabIndex = 5;
        statisticsFilterLabel.Text = "Активный фильтр: отсутствует";
        // 
        // statisticsEmptyLabel
        // 
        statisticsEmptyLabel.AutoSize = true;
        statisticsEmptyLabel.Location = new System.Drawing.Point(665, 10);
        statisticsEmptyLabel.Name = "statisticsEmptyLabel";
        statisticsEmptyLabel.Size = new System.Drawing.Size(114, 15);
        statisticsEmptyLabel.TabIndex = 4;
        statisticsEmptyLabel.Text = "Пустых значений: 0";
        // 
        // statisticsUniqueLabel
        // 
        statisticsUniqueLabel.AutoSize = true;
        statisticsUniqueLabel.Location = new System.Drawing.Point(508, 10);
        statisticsUniqueLabel.Name = "statisticsUniqueLabel";
        statisticsUniqueLabel.Size = new System.Drawing.Size(141, 15);
        statisticsUniqueLabel.TabIndex = 3;
        statisticsUniqueLabel.Text = "Уникальных значений: 0";
        // 
        // statisticsTotalLabel
        // 
        statisticsTotalLabel.AutoSize = true;
        statisticsTotalLabel.Location = new System.Drawing.Point(393, 10);
        statisticsTotalLabel.Name = "statisticsTotalLabel";
        statisticsTotalLabel.Size = new System.Drawing.Size(97, 15);
        statisticsTotalLabel.TabIndex = 2;
        statisticsTotalLabel.Text = "Всего записей: 0";
        // 
        // statisticsColumnComboBox
        // 
        statisticsColumnComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        statisticsColumnComboBox.FormattingEnabled = true;
        statisticsColumnComboBox.Location = new System.Drawing.Point(137, 7);
        statisticsColumnComboBox.Name = "statisticsColumnComboBox";
        statisticsColumnComboBox.Size = new System.Drawing.Size(250, 23);
        statisticsColumnComboBox.TabIndex = 1;
        // 
        // statisticsColumnLabel
        // 
        statisticsColumnLabel.AutoSize = true;
        statisticsColumnLabel.Location = new System.Drawing.Point(5, 10);
        statisticsColumnLabel.Name = "statisticsColumnLabel";
        statisticsColumnLabel.Size = new System.Drawing.Size(126, 15);
        statisticsColumnLabel.TabIndex = 0;
        statisticsColumnLabel.Text = "Колонка для анализа:";
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1184, 681);
        Controls.Add(mainTabControl);
        Controls.Add(topPanel);
        MinimumSize = new System.Drawing.Size(1000, 600);
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "Анализ технической документации";
        topPanel.ResumeLayout(false);
        topPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)documentsGrid).EndInit();
        mainTabControl.ResumeLayout(false);
        dataTabPage.ResumeLayout(false);
        statisticsTabPage.ResumeLayout(false);
        statisticsSplitContainer.Panel1.ResumeLayout(false);
        statisticsSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)statisticsSplitContainer).EndInit();
        statisticsSplitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)statisticsGrid).EndInit();
        statisticsTopPanel.ResumeLayout(false);
        statisticsTopPanel.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label statisticsDetailsLabel;

    private System.Windows.Forms.ComboBox statisticsModeComboBox;

    private System.Windows.Forms.Label statisticsModeLabel;

    private System.Windows.Forms.Label statisticsFilterLabel;

    private System.Windows.Forms.Panel statisticsChartPanel;

    private System.Windows.Forms.DataGridView statisticsGrid;

    private System.Windows.Forms.SplitContainer statisticsSplitContainer;

    private System.Windows.Forms.Label statisticsEmptyLabel;
    
    private System.Windows.Forms.Label statisticsTotalLabel;
    
    private System.Windows.Forms.Label statisticsUniqueLabel;

    private System.Windows.Forms.ComboBox statisticsColumnComboBox;

    private System.Windows.Forms.Label statisticsColumnLabel;

    private System.Windows.Forms.Panel statisticsTopPanel;

    private System.Windows.Forms.TabControl mainTabControl;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.TabPage statisticsTabPage;

    private System.Windows.Forms.ComboBox encodingComboBox;

    private System.Windows.Forms.Label encodingLabel;

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