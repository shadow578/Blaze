using System.Windows.Forms;

namespace BlazeSharp.UI
{
    public partial class LiveStatsUI : Form
    {
        public LiveStatsUI()
        {
            InitializeComponent();
        }

        public void UpdateStats(Keys vKey, char vChar, string cmdProg, bool isCapturing)
        {
            txtLastKeycode.Text = vKey.ToString();
            txtLastKeyChar.Text = vChar.ToString();
            txtCommand.Text = cmdProg;
            chkIsCapturing.Checked = isCapturing;
        }
    }
}
