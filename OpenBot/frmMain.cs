using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenBot
{
    public partial class frmMain : Form
    {
        private OpenBot _bot;
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            _bot = new OpenBot();
        }

        private void populateList()
        {
            lstActiveHandlers.Items.Clear();
            lstActiveHandlers.BeginUpdate();

            foreach (var i in _bot.PluginManager.PluginAssemblies)
            {
                foreach (var u in i.RunningChatHandlersWithNames)
                {
                    lstActiveHandlers.Items.Add(u.Key);
                }
            }

            lstActiveHandlers.EndUpdate();
        }

        private void cmsFactories_Opening(object sender, CancelEventArgs e)
        {
            cmsFactories.Items.Clear();

            if(lstActiveHandlers.SelectedIndex >= 0)
            {
                cmsFactories.Items.Add("Preferences", null, (o, p) =>
                {
                    foreach(var i in _bot.PluginManager.PluginAssemblies)
                    {
                        foreach (var u in i.RunningChatHandlersWithNames)
                        {
                            if (u.Key == (string)lstActiveHandlers.SelectedItem)
                                u.Value.ShowPreferences();
                        }
                    }
                });
            }

            foreach(var i in _bot.PluginManager.PluginAssemblies)
            {
                foreach(var u in i.Plugins)
                {
                    foreach(var f in u.ChatHandlerFactories)
                    {
                        cmsFactories.Items.Add(f.Name, null, (o, p) => {
                            if(!i.CreateChatHandler(f.Name, f))
                            {
                                MessageBox.Show("Unable to create \"" + f.Name + "\"", "OpenBot", MessageBoxButtons.OK);
                            }
                            populateList();
                        });
                    }
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _bot.Connect();
            _bot.DebugOutput = true;

            _bot.Login(txtUsername.Text, txtOauth.Text);
            _bot.JoinChannel(txtChannel.Text.ToLower());

            _bot.InitializePluginManager();
            _bot.BeginListen();
        }
    }
}
