using System.Windows.Forms;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class PaletteBusy : UserControl
    {
        public PaletteBusy()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Sets the text displayed
        /// </summary>
        public string InfoText
        {
            get
            {
                if (label != null)
                {
                    return label.Text;
                }
                return null;
            }
            set
            {
                if (label != null)
                {
                    label.Text = value;
                }
            }
        }


    }
}
