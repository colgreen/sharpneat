using System;


namespace SharpNeatLib
{
	/// <summary>
	/// This class is intended to be added to the Items collection of a combo box.
	/// </summary>
	public class ListItem : IComparable
	{
		private string itemCode;
		private string itemDescription;
		private object data;

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ListItem() {}

		/// <summary>
		/// Create a new ListItem with the provided itemCode and itemDescription.
		/// </summary>
		/// <param name="itemCode"></param>
		/// <param name="itemDescription"></param>
		public ListItem(string itemCode, string itemDescription)
		{
			this.ItemCode = itemCode;
			this.ItemDescription = itemDescription;
		}

		/// <summary>
		/// Create a new ListItem with the provided itemCode, itemDescription and data.
		/// </summary>
		/// <param name="itemCode"></param>
		/// <param name="itemDescription"></param>
		/// <param name="data"></param>
		public ListItem(string itemCode, string itemDescription, object data)
		{
			this.ItemCode = itemCode;
			this.ItemDescription = itemDescription;
			this.data = data;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets/Sets the ItemCode.
		/// </summary>
		public string ItemCode
		{
			get
			{
				return itemCode;
			}
			set
			{
				itemCode = (value==null ? "" : value);
			}
		}

		/// <summary>
		/// Gets/Sets the ItemDescription.
		/// </summary>
		public string ItemDescription
		{
			get
			{
				return itemDescription;
			}
			set
			{
				itemDescription = (value==null ? "" : value);
			}
		}
	
		/// <summary>
		/// Gets/Sets the Data. Data is decalred as Object and is provided so that additional data can be attached to a ListItem object.
		/// </summary>
		public object Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		#endregion


		#region IComparable

		/// <summary>
		/// Compare ListItems based on the itemCode.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			return itemCode.CompareTo(((ListItem)obj).ItemCode);
		}

		#endregion


		/// <summary>
		/// Returns the item's description.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return itemDescription;
		}
	}
}
