using System.Windows.Forms;

namespace SharpNeat.Windows.App.Forms
{

    // TODO: Implement.

    /// <summary>
    /// Generic form for task visualization. 
    /// </summary>
    public partial class TaskForm : Form
    {
        //readonly AbstractDomainView _domainViewControl;

        #region Constructor

        /// <summary>
        /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
        /// from the evolution algorithm and cleanly detach from it when this form closes.
        /// </summary>
        public TaskForm(string title)
        {
            InitializeComponent();
            this.Text = title;

            //_domainViewControl = domainViewControl;
            //domainViewControl.Dock = DockStyle.Fill;
            //this.Controls.Add(domainViewControl);
            //this.Size = domainViewControl.WindowSize;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh view.
        /// </summary>
        public void RefreshView(object genome)
        {
            //if(this.InvokeRequired)
            //{
            //    // Note. Must use Invoke(). BeginInvoke() will execute asynchronously and the evolution algorithm therefore 
            //    // may have moved on and will be in an intermediate and indeterminate (between generations) state.
            //    this.Invoke(new MethodInvoker(delegate()
            //    {
            //        RefreshView(genome);
            //    }));
            //    return;
            //}

            //if(this.IsDisposed) {
            //    return;
            //}

            //_domainViewControl.RefreshView(genome);
        }

        #endregion
    }
}
