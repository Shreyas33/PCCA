using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoreLinq;

namespace FizzBuzzApp
{
    public partial class Form1 : Form
    {
        public static int totalRecords { get; set; }
        private const int pageSize = 100000;
        int fromVal = 0;
        int toVal = 0;
        List<keyvalue> kvpairlist = new List<keyvalue>();
        public Form1()
        {
            InitializeComponent();
        }
        public void button1_Click(object sender, EventArgs e)
        {
            if(rangevalid())
            { 
            Refreshform();
            fromVal = Convert.ToInt32(RangeFrom.Text.Trim());
            toVal = Convert.ToInt32(RangeTo.Text.Trim());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Index" });
            bindingNavigator1.BindingSource = bindingSource1;
            bindingSource1.CurrentChanged += new System.EventHandler(bindingSource1_CurrentChanged);
            bindingSource1.DataSource = new PageOffsetList(pageSize, toVal);
            ClearTextBoxes();
            }
        }

        private bool rangevalid()
        {
           
            int parsedValue;

            if (RangeFrom.Text.Trim().Length == 0 || RangeTo.Text.Trim().Length == 0)
            {
                MessageBox.Show("Both 'to' and 'from' are required field");
                return false;
            }
            if ((!int.TryParse(RangeFrom.Text, out parsedValue) || (!int.TryParse(RangeTo.Text, out parsedValue))))
            {
                MessageBox.Show("Both 'to' and 'from' should be numberic.");
                return false;
            }
            if (Convert.ToInt32(RangeTo.Text.Trim()) <= 0)
            {
                MessageBox.Show("'To' field can not be 0 or negative.");
                return false;
            }
            if (Convert.ToInt32(RangeTo.Text.Trim()) <= Convert.ToInt32(RangeFrom.Text.Trim()))
            {
                MessageBox.Show("'To' field shot be greater than 'from'");
                return false;
            }
            return true;
        }

        public void Refreshform()
        {
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.DataSource = "";
                fromVal = 0;
                toVal = 0;
                
            }

        }

        private void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;
            
            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }
        private void keyValBtn_Click(object sender, EventArgs e)
        {
            if (kvisvalid())
            {
                keyvalue kvpair = new keyvalue();
                kvpair.key = Convert.ToInt32(keytext.Text);
                kvpair.value = valuetext.Text;
                kvpairlist.Add(kvpair);
                keytext.Text = String.Empty;
                valuetext.Text = String.Empty;
                listBox1.Items.Add(string.Format("{0} => {1}", kvpair.key, kvpair.value));//kvpairlist.ToList();
            }
        }

        private bool kvisvalid()
        {
            int parsedValue;
            
            if (keytext.Text.Length ==0 || valuetext.Text.Length==0)
            {
                MessageBox.Show("Both Key and value are required field");
                return false;
            }
            if (!int.TryParse(keytext.Text, out parsedValue))
            {
                MessageBox.Show("Key is a number only field");
                return false;
            }
            return true;
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

            //get next list of records using the "Current" offset 
            int offset = (int)bindingSource1.Current;
            var records = new List<Record>();

            //adding records
            for (int i = fromVal; i < offset + pageSize && i <= toVal; i++)
                records.Add(new Record { Index = CheckCondition(i) });
            dataGridView1.DataSource = records;

            //  scroll logic
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }
        
        //Fizz Buzz Logoc
        private string CheckCondition(int i)
        {
            string var = "";
            foreach (var kp in kvpairlist)
            {
                if (i % kp.key == 0)
                    var += kp.value + " ";
            }
            if (var.Length == 0)
                var = i.ToString();
            return var;
        }

        class Record
        {
            public String Index { get; set; }
        }
        class PageOffsetList : System.ComponentModel.IListSource
        {
            public bool ContainsListCollection { get; protected set; }
            private int TotalRecords = 0;
            private int pageSize = 0;
            public PageOffsetList(int pageSize, int total)
            {
                this.TotalRecords = total;
                this.pageSize = pageSize;
            }
            public System.Collections.IList GetList()
            {
                // Return a list of page offsets based on "totalRecords" and "pageSize"
                var pageOffsets = new List<int>();
                for (int offset = 1; offset < TotalRecords; offset += pageSize)
                    pageOffsets.Add(offset);
                return pageOffsets;
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            kvpairlist.Clear();
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }
        }
    }
    class keyvalue
    {
        public int key { get; set; }
        public string value { get; set; }
    }
}
