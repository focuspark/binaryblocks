using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinaryBlocks.Viewer
{
    public partial class TextForm : Form
    {
        public TextForm(Action<Stream> callback)
        {
            if(callback==null)
                throw new ArgumentNullException("callback");

            InitializeComponent();

            _callback = callback;
        }

        private Action<Stream> _callback;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.base64Textbox.Focus();
        }

        private void Decode_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(this.base64Textbox.Text);
                MemoryStream stream = new MemoryStream(bytes);
                _callback(stream);
                this.Owner.Focus();
                this.Close();
            }
            catch
            {
                MessageBox.Show("Invalid or corrupt Base64 detected. Unable to decode.", "Base64 Decode Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
