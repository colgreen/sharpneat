
namespace SharpNeat.Windows.App
{
    public class ListItem 
    {
        string _itemCode;
        string _itemDescription;

        #region Constructors

        public ListItem(string itemCode, string itemDescription)
        {
            _itemCode = itemCode;
            _itemDescription = itemDescription;
        }

        public ListItem(string itemCode, string itemDescription, object data)
        {
            _itemCode = itemCode;
            _itemDescription = itemDescription;
            Data = data;
        }

        #endregion

        #region Properties

        public string ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value ?? string.Empty; }
        }

        public string ItemDescription
        {
            get { return _itemDescription; }
            set { _itemDescription = value ?? string.Empty; }
        }

        public object Data { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return _itemDescription;
        }

        #endregion
    }
}
