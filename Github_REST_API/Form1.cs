using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace Github_REST_API
{
    public partial class Form1 : Form
    {

        private List<string[]> foo = new List<string[]>();
        public Form1()
        {
            InitializeComponent();
            Search_Type_cmbo.Items.Add("Users");
            Search_Type_cmbo.Items.Add("Commits");
            Search_Type_cmbo.SelectedIndex = 0;
            Search_cmbo.Items.Add("followers:>1000");
            Search_cmbo.Items.Add("language:JavaScript+created%3A2011-01-01..2011-02-01");
            Search_cmbo.Items.Add("location:usa");
            Search_cmbo.Items.Add("location:russia");
            Search_cmbo.SelectedIndex = 0;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void Srch_btn_ClickAsync(object sender, EventArgs e)
        {

            // dataGridView1.Rows.Clear();
            // dataGridView1.Refresh();
            int index = 0;
            const string username = "Nahel88";
            var client = new GitHubClient(new ProductHeaderValue(username));
            var basicAuth = new Credentials("f0701a76d2520b4be4373af02c3d88a217000928");
            client.Credentials = basicAuth;
            string start_date = dateTimePicker1.Value.Date.ToString("yyyy-MM");
            string end_date = dateTimePicker2.Value.Date.ToString("yyyy-MM");
            string search_date = dateTimePicker1.Value.Date.AddMonths(1).ToString("yyyy-MM");
            List<string[]> row = new List<string[]>();

            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "#";
            dataGridView1.Columns[1].Name = "USER NAME";
            dataGridView1.Columns[2].Name = "ID";
            dataGridView1.Columns[3].Name = "URL";

            if (dateTimePicker1.Value < dateTimePicker2.Value)
            {
                if (Search_Type_cmbo.SelectedIndex == 0)
                {
                    Srch_btn.Text = "Wait...";
                    Srch_btn.Enabled = false;

                    for (DateTime f = dateTimePicker1.Value; f <= dateTimePicker2.Value; f = f.AddMonths(1))
                    {

                       // var request = new SearchUsersRequest(Search_cmbo.SelectedItem.ToString() + "+created%3A" + start_date + "-01" + ".." + f.ToString() + "-01");

                       // var result = await githubClient.Search.SearchUsers(request);
                        var request = new SearchUsersRequest(Search_cmbo.SelectedItem.ToString() + "+created%3A" + start_date +"-01"+ ".." + f.ToString() + "-01");
                        request.PerPage = 100;

                        var counter = client.Search.SearchUsers(request).GetAwaiter().GetResult();
                       // var counter = await client.Search.SearchUsers(request);
                        label6.Text = counter.TotalCount.ToString();
                        foreach (var item in counter.Items)
                        {

                            // this.dataGridView2.Rows.Add(item.Login.ToString());
                            string[] rows = new string[] { index.ToString(), item.Login.ToString(), item.Id.ToString(), item.HtmlUrl.ToString() };
                            row.Add(rows);

                        


                        }
                        search_date = f.ToString();
                        Thread.Sleep(2000);
                    }
                    //  IEnumerable<string[]> distinctvals = row;
                    
                    foo = row;
                    // var distinctList = foo.Distinct(new Comparer()).ToList();
                    //  var distinctArraylist = row.ToArray().Distinct();
                    //  var distinctArrayList = new ArrayList((ICollection)row.Cast<string[]>().Distinct().ToArray());

                  //  DataTable table = ConvertListToDataTable(row);


                    DataRow[] dtrows = new DataRow[row.Count];


                    int currentRow = 0;
                    foreach (string[] curObj in row)
                    {
                        this.dataGridView1.Rows.Add(curObj);
                       dataGridView1.Rows[currentRow].Cells["#"].Value = currentRow;              
                        currentRow++;
                    }
                 //   DataTable dt =ConvertListToDataTable(row);
                    
                   
                }
          
                var requesta = new SearchUsersRequest(Search_cmbo.SelectedItem.ToString() + "+created%3A" + start_date + "-01"+".." + end_date + "-01");
                var countera = client.Search.SearchUsers(requesta).GetAwaiter().GetResult();
                label5.Text = countera.TotalCount.ToString();
                
                Srch_btn.Text = "Search";
                Srch_btn.Enabled = true;
                DataTable table = ConvertListToDataTable(row);
                DataTable UniqueRecords = RemoveDuplicateRows(table, "ID");
                int rr = 0;
                foreach (DataRow item in UniqueRecords.Rows)
                {
                
                for (int i = 0; i < UniqueRecords.Rows.Count; i++)
                {
                        item["#"] = rr.ToString();
                }
                    rr++;
                }

                dataGridView2.DataSource = UniqueRecords;
            }
            else
            {
                MessageBox.Show("Start date can not be after End date");
            }

            
        }

        public static DataTable ToDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }

        public DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                ArrayList UniqueRecords = new ArrayList();
                ArrayList DuplicateRecords = new ArrayList();
                foreach (DataRow dRow in table.Rows)
                {
                    if (UniqueRecords.Contains(dRow[DistinctColumn]))
                        DuplicateRecords.Add(dRow);
                    else
                        UniqueRecords.Add(dRow[DistinctColumn]);
                }

                foreach (DataRow dRow in DuplicateRecords)
                {
                    table.Rows.Remove(dRow);
                }

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            table.Columns.Add("#"); table.Columns.Add("USER NAME"); table.Columns.Add("ID"); table.Columns.Add("URL");
            //table.Rows.Add("", "", "", "");
            // Add rows.
            int tindx = 0;
          foreach (var array in list)
            {
                /* for (int i = 1; i <= list.Count; i++)
                 {
                     DataRow dr = table.NewRow();

                     dr ["#"] = array[0];
                     dr["USER NAME"] = array[1];
                     dr["ID"] = array[2];
                     dr["URL"] = array[3];
                     table.Rows.Add(dr);
                 }*/
              
                    DataRow
                        row = table.NewRow();
                row["#"] = tindx;
                row["USER NAME"] = array[1];
                    row["ID"] = array[2];
                    row["URL"] = array[3];
                    table.Rows.Add(row);

                tindx++;
            }

            return table;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable table = ConvertListToDataTable(foo);
            DataTable dt = new DataTable();
         //   dt.Columns.Add("#"); dt.Columns.Add("USER NAME"); dt.Columns.Add("ID"); dt.Columns.Add("URL");
            dataGridView1.DataSource = table;
        //    dt = dataGridView1.DataSource as DataTable;
         //   DataTable UniqueRecords = RemoveDuplicateRowss(dt, "ID");
//dataGridView1.DataSource = UniqueRecords;
        }

        public DataTable RemoveDuplicateRowss(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }
    }

}