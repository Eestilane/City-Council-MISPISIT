namespace FormApp
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonTest = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.buttonDeputies = new System.Windows.Forms.Button();
            this.buttonMeetings = new System.Windows.Forms.Button();
            this.buttonProjects = new System.Windows.Forms.Button();
            this.buttonVotes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonClearDataGrid = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAddProject = new System.Windows.Forms.Button();
            this.labelSQLORM = new System.Windows.Forms.Label();
            this.radioButtonSQL = new System.Windows.Forms.RadioButton();
            this.radioButtonORM = new System.Windows.Forms.RadioButton();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(12, 639);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(75, 30);
            this.buttonTest.TabIndex = 0;
            this.buttonTest.Text = "Тест";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(557, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(695, 621);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 28);
            this.textBox1.MaximumSize = new System.Drawing.Size(458, 226);
            this.textBox1.MinimumSize = new System.Drawing.Size(458, 40);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(458, 40);
            this.textBox1.TabIndex = 2;
            // 
            // buttonEnter
            // 
            this.buttonEnter.Location = new System.Drawing.Point(476, 28);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(75, 30);
            this.buttonEnter.TabIndex = 3;
            this.buttonEnter.Text = "Enter";
            this.buttonEnter.UseVisualStyleBackColor = true;
            this.buttonEnter.Click += new System.EventHandler(this.buttonEnter_Click);
            // 
            // buttonDeputies
            // 
            this.buttonDeputies.Location = new System.Drawing.Point(12, 567);
            this.buttonDeputies.Name = "buttonDeputies";
            this.buttonDeputies.Size = new System.Drawing.Size(226, 30);
            this.buttonDeputies.TabIndex = 4;
            this.buttonDeputies.Text = "Список депутатов";
            this.buttonDeputies.UseVisualStyleBackColor = true;
            this.buttonDeputies.Click += new System.EventHandler(this.buttonDeputies_Click);
            // 
            // buttonMeetings
            // 
            this.buttonMeetings.Location = new System.Drawing.Point(244, 566);
            this.buttonMeetings.Name = "buttonMeetings";
            this.buttonMeetings.Size = new System.Drawing.Size(226, 30);
            this.buttonMeetings.TabIndex = 5;
            this.buttonMeetings.Text = "Список собраний";
            this.buttonMeetings.UseVisualStyleBackColor = true;
            this.buttonMeetings.Click += new System.EventHandler(this.buttonMeetings_Click);
            // 
            // buttonProjects
            // 
            this.buttonProjects.Location = new System.Drawing.Point(12, 603);
            this.buttonProjects.Name = "buttonProjects";
            this.buttonProjects.Size = new System.Drawing.Size(226, 30);
            this.buttonProjects.TabIndex = 6;
            this.buttonProjects.Text = "Список проектов";
            this.buttonProjects.UseVisualStyleBackColor = true;
            this.buttonProjects.Click += new System.EventHandler(this.buttonProjects_Click);
            // 
            // buttonVotes
            // 
            this.buttonVotes.Location = new System.Drawing.Point(244, 602);
            this.buttonVotes.Name = "buttonVotes";
            this.buttonVotes.Size = new System.Drawing.Size(226, 30);
            this.buttonVotes.TabIndex = 7;
            this.buttonVotes.Text = "Список голосований";
            this.buttonVotes.UseVisualStyleBackColor = true;
            this.buttonVotes.Click += new System.EventHandler(this.buttonVotes_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "SQL Ввод";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 551);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "ORM Методы";
            // 
            // buttonClearDataGrid
            // 
            this.buttonClearDataGrid.Location = new System.Drawing.Point(557, 639);
            this.buttonClearDataGrid.Name = "buttonClearDataGrid";
            this.buttonClearDataGrid.Size = new System.Drawing.Size(695, 30);
            this.buttonClearDataGrid.TabIndex = 10;
            this.buttonClearDataGrid.Text = "Очистить сетку";
            this.buttonClearDataGrid.UseVisualStyleBackColor = true;
            this.buttonClearDataGrid.Click += new System.EventHandler(this.buttonClearDataGrid_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(241, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(307, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Используйте для VALUES N: VALUES  (N\'Иванов\', N\'Иван\');";
            // 
            // buttonAddProject
            // 
            this.buttonAddProject.Location = new System.Drawing.Point(12, 336);
            this.buttonAddProject.Name = "buttonAddProject";
            this.buttonAddProject.Size = new System.Drawing.Size(226, 30);
            this.buttonAddProject.TabIndex = 12;
            this.buttonAddProject.Text = "Добавить проект";
            this.buttonAddProject.UseVisualStyleBackColor = true;
            this.buttonAddProject.Click += new System.EventHandler(this.buttonAddProject_Click);
            // 
            // labelSQLORM
            // 
            this.labelSQLORM.AutoSize = true;
            this.labelSQLORM.Location = new System.Drawing.Point(10, 319);
            this.labelSQLORM.Name = "labelSQLORM";
            this.labelSQLORM.Size = new System.Drawing.Size(108, 13);
            this.labelSQLORM.TabIndex = 15;
            this.labelSQLORM.Text = "SQL и ORM Методы";
            // 
            // radioButtonSQL
            // 
            this.radioButtonSQL.AutoSize = true;
            this.radioButtonSQL.Location = new System.Drawing.Point(9, 135);
            this.radioButtonSQL.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSQL.Name = "radioButtonSQL";
            this.radioButtonSQL.Size = new System.Drawing.Size(122, 17);
            this.radioButtonSQL.TabIndex = 21;
            this.radioButtonSQL.TabStop = true;
            this.radioButtonSQL.Text = "Использовать SQL";
            this.radioButtonSQL.UseVisualStyleBackColor = true;
            // 
            // radioButtonORM
            // 
            this.radioButtonORM.AutoSize = true;
            this.radioButtonORM.Location = new System.Drawing.Point(9, 156);
            this.radioButtonORM.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonORM.Name = "radioButtonORM";
            this.radioButtonORM.Size = new System.Drawing.Size(126, 17);
            this.radioButtonORM.TabIndex = 22;
            this.radioButtonORM.TabStop = true;
            this.radioButtonORM.Text = "Использовать ORM";
            this.radioButtonORM.UseVisualStyleBackColor = true;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(12, 73);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(226, 30);
            this.buttonSearch.TabIndex = 23;
            this.buttonSearch.Text = "Поиск";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(10, 372);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(226, 30);
            this.buttonSave.TabIndex = 24;
            this.buttonSave.Text = "Сохранить изменения";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(10, 409);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(226, 30);
            this.buttonDelete.TabIndex = 25;
            this.buttonDelete.Text = "Удалить запись";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(10, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Выберете метод!";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.radioButtonORM);
            this.Controls.Add(this.radioButtonSQL);
            this.Controls.Add(this.labelSQLORM);
            this.Controls.Add(this.buttonAddProject);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonClearDataGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonVotes);
            this.Controls.Add(this.buttonProjects);
            this.Controls.Add(this.buttonMeetings);
            this.Controls.Add(this.buttonDeputies);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonTest);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Button buttonDeputies;
        private System.Windows.Forms.Button buttonMeetings;
        private System.Windows.Forms.Button buttonProjects;
        private System.Windows.Forms.Button buttonVotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonClearDataGrid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAddProject;
        private System.Windows.Forms.Label labelSQLORM;
        private System.Windows.Forms.RadioButton radioButtonSQL;
        private System.Windows.Forms.RadioButton radioButtonORM;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Label label4;
    }
}

