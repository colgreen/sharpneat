using System.Windows.Forms;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// Form for genome visualization. A generic form that supports all genome types by wrapping an AbstractGenomeView
    /// (the control does the actual visual rendering).
    /// </summary>
    public partial class GenomeForm : Form
    {
        readonly AbstractGenomeView _genomeViewControl;

        #region Constructor

        /// <summary>
        /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
        /// from the evolution algorithm and cleanly detach from it when this form closes.
        /// </summary>
        public GenomeForm(string title, AbstractGenomeView genomeViewControl)
        {
            InitializeComponent();
            this.Text = title;

            _genomeViewControl = genomeViewControl;
            genomeViewControl.Dock = DockStyle.Fill;
            this.Controls.Add(genomeViewControl);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh view.
        /// </summary>
        public void RefreshView(object genome)
        {
            if(this.InvokeRequired)
            {
                RefreshView(genome);
                return;
            }

            this.Invoke(new MethodInvoker(delegate()
            {
                if(this.IsDisposed) {
                    return;
                }
                _genomeViewControl.RefreshView(genome);
            }));
        }

        #endregion
    }
}
