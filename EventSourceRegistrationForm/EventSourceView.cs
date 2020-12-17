using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ssepan.Utility;
namespace EventSourceRegistrationForm
{
    public partial class EventSourceView : Form
    {
        public EventSourceView()
        {
            InitializeComponent();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                txtSource.Text = txtSource.Text.Trim();
                if ((txtSource.Text == null) || (txtSource.Text == String.Empty))
                {
                    MessageBox.Show("Enter a source.");
                }
                else
                {
                    if (EventLog.SourceExists(txtSource.Text))
                    {
                        MessageBox.Show("Source already exists.");
                    }
                    else
                    {
                        EventLog.CreateEventSource(txtSource.Text, "Application");
                        MessageBox.Show("Source added.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(
                    ex,
                    System.Reflection.MethodBase.GetCurrentMethod(),
                    System.Diagnostics.EventLogEntryType.Error);
                    
                throw;
            }
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                txtSource.Text = txtSource.Text.Trim();
                if ((txtSource.Text == null) || (txtSource.Text == String.Empty))
                {
                    MessageBox.Show("Enter a source.");
                }
                else
                { 
                    if (!EventLog.SourceExists(txtSource.Text))
                    {
                        MessageBox.Show("Source does not exist.");
                    }
                    else
                    {
                        EventLog.DeleteEventSource(txtSource.Text, "Application");//this crashes with a path not found...
                        MessageBox.Show("Source deleted.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(
                    ex,
                    System.Reflection.MethodBase.GetCurrentMethod(),
                    System.Diagnostics.EventLogEntryType.Error);
                    
                throw;
            }
        }
    }
}
