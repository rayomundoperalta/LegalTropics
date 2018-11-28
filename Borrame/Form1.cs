using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Borrame
{
    // This form demonstrates using a BindingNavigator to display 
    // rows from a database query sequentially.
    public partial class Form1 : Form
    {
        // This is the BindingNavigator that allows the user
        // to navigate through the rows in a DataSet.
        BindingNavigator customersBindingNavigator = new BindingNavigator(true);

        // This is the BindingSource that provides data for
        // the Textbox control.
        BindingSource customersBindingSource = new BindingSource();

        // This is the TextBox control that displays the CompanyName
        // field from the DataSet.
        TextBox companyNameTextBox = new TextBox();

        public Form1()
        {
            // Set up the BindingSource component.
            this.customersBindingNavigator.BindingSource = this.customersBindingSource;
            this.customersBindingNavigator.Dock = DockStyle.Top;
            this.Controls.Add(this.customersBindingNavigator);

            // Set up the TextBox control for displaying company names.
            this.companyNameTextBox.Dock = DockStyle.Bottom;
            this.Controls.Add(this.companyNameTextBox);

            // Set up the form.
            this.Size = new Size(800, 200);
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            // Open a connection to the database.
            // Replace the value of connectString with a valid 
            // connection string to a Northwind database accessible 
            // to your system.
            
            string connectString =
                "Data Source=REPH-NITRO-5; Initial Catalog=AMLOTeam; Persist Security Info = True;user=sa;password=Samahil14200";

            using (SqlConnection connection = new SqlConnection(connectString))
            {

                SqlDataAdapter dataAdapter1 =
                    new SqlDataAdapter(new SqlCommand("Select * From Funcionarios", connection));
                DataSet ds = new DataSet("APF");
                ds.Tables.Add("Funcionarios");
                dataAdapter1.Fill(ds.Tables["Funcionarios"]);

                // Assign the DataSet as the DataSource for the BindingSource.
                this.customersBindingSource.DataSource = ds.Tables["Customers"];

                // Bind the CompanyName field to the TextBox control.
                this.companyNameTextBox.DataBindings.Add(
                    new Binding("Text",
                    this.customersBindingSource,
                    "PrimerNombre",
                    true));
            }
        }
    }
}



