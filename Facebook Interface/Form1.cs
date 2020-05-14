using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Facebook_Interface
{
    public partial class Form1 : Form
    {
        MySqlConnection connection;
        MySqlCommand command;
        MySqlDataAdapter adapter;
        DataTable table;
        int userID;
        string firstName;
        string lastName;
        int nextID;

        public void CreateDataGrid()
        {
            connection = new MySqlConnection("Server = proj-mysql.uopnet.plymouth.ac.uk; Port = 3306; UID = ISAD157_HCobert; password = ISAD157_22227228; database = isad157_hcobert; SslMode = none");
            command = new MySqlCommand("SELECT UserID, FirstName, LastName FROM user;", connection);
            connection.Open();
            adapter = new MySqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);

            dataGridProfiles.DataSource = table;
            dataGridProfiles.Columns[0].Visible = false;
            dataGridProfiles.Width = dataGridProfiles.Columns.GetColumnsWidth(DataGridViewElementStates.None) - dataGridProfiles.Columns[0].Width + 18;
            //dataGridProfiles.AutoSize = true;
        }

        public void ClearSelectedData()
        {
            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            UserIDBox.Text = "";
            HometownBox.Text = "";
            GenderBox.Text = "";
            CurrentLocationBox.Text = "";
            dataGridProfiles.ClearSelection();
        }

        public Form1()
        {          
            InitializeComponent();
            CreateDataGrid();
        }

        private void dataGridProfiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridProfiles.SelectedRows.Count == 0)
            {
                return;
            }

            //grab userid, first name, last name
            userID = Convert.ToInt32(dataGridProfiles.SelectedRows[0].Cells[0].Value);            
            firstName = Convert.ToString(dataGridProfiles.SelectedRows[0].Cells[1].Value);
            lastName = Convert.ToString(dataGridProfiles.SelectedRows[0].Cells[2].Value);

            //grab hometown, gender, currentlocation
            command = new MySqlCommand("SELECT Hometown, Gender, CurrentLocation FROM user WHERE UserID = @UserID;", connection);
            command.Parameters.AddWithValue("@UserID",userID);
            adapter = new MySqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);

            //write into text boxes
            UserIDBox.Text = Convert.ToString(userID);
            FirstNameBox.Text = firstName;
            LastNameBox.Text = lastName;

            HometownBox.Text = table.Rows[0].Field<string>("Hometown");
            GenderBox.Text = table.Rows[0].Field<string>("Gender");
            CurrentLocationBox.Text = table.Rows[0].Field<string>("CurrentLocation");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //grab userid
            userID = Convert.ToInt32(dataGridProfiles.SelectedRows[0].Cells[0].Value);

            //save changes of user info to database
            command = new MySqlCommand("UPDATE user SET FirstName = @FirstName, LastName = @LastName, Hometown = @Hometown, Gender = @Gender, CurrentLocation = @CurrentLocation WHERE UserID = @UserID", connection);
            command.Parameters.AddWithValue("@FirstName", FirstNameBox.Text);
            command.Parameters.AddWithValue("@LastName", LastNameBox.Text);
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@Hometown", HometownBox.Text);
            command.Parameters.AddWithValue("@Gender", GenderBox.Text);
            command.Parameters.AddWithValue("@CurrentLocation", CurrentLocationBox.Text);
            command.ExecuteNonQuery();

            //refresh table
            CreateDataGrid();

            //Clear Selection
            ClearSelectedData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Clear Selection
            ClearSelectedData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //grab last id 
            command = new MySqlCommand("SELECT UserID FROM user ORDER BY UserID DESC", connection);
            adapter = new MySqlDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);
            nextID = Convert.ToInt32(table.Rows[0].Field<int>("UserID")) + 1;

            //create new record
            command = new MySqlCommand("INSERT INTO user (UserID, FirstName, LastName, Hometown, Gender, CurrentLocation) VALUES (@UserID, @FirstName, @LastName, @Hometown, @Gender, @CurrentLocation)", connection);
            command.Parameters.AddWithValue("@FirstName", FirstNameBox.Text);
            command.Parameters.AddWithValue("@LastName", LastNameBox.Text);
            command.Parameters.AddWithValue("@UserID", nextID);
            command.Parameters.AddWithValue("@Hometown", HometownBox.Text);
            command.Parameters.AddWithValue("@Gender", GenderBox.Text);
            command.Parameters.AddWithValue("@CurrentLocation", CurrentLocationBox.Text);
            command.ExecuteNonQuery();

            //refresh table
            CreateDataGrid();

            //Clear Selection
            ClearSelectedData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.Close();
        }
    }
}
