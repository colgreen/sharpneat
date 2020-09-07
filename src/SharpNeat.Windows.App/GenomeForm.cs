using System.Windows.Forms;

namespace SharpNeat.Windows.App
{
    /// <summary>
    /// Form for genome visualization. A generic form that supports all genome types by wrapping an AbstractGenomeView
    /// (the control does the actual visual rendering).
    /// </summary>
    public partial class GenomeForm : Form
    {




        #region Constructor

        /// <summary>
        /// Construct with the provided form title, genome view/renderer and evolution algorithm. We listen to update events
        /// from the evolution algorithm and cleanly detach from it when this form closes.
        /// </summary>
        public GenomeForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the genome to render.
        /// </summary>
        //public object Genome 
        //{ 
        //    get => _genomeViewControl.Genome;
        //    set
        //    {
        //        // Initial check.
        //        if(this.IsDisposed) {
        //            return;
        //        }

        //        if(!this.InvokeRequired)
        //        {
        //            _genomeViewControl.Genome = value;
        //            return;
        //        }

        //        this.Invoke(new MethodInvoker(delegate()
        //        {
        //            // Secondary check; the form could have been disposed after the first test of IsDisposed, and the call to Invoke().
        //            if(this.IsDisposed) {
        //                return;
        //            }
        //            _genomeViewControl.Genome = value;
        //        }));
        //    }
        //}

        #endregion
    }
}
