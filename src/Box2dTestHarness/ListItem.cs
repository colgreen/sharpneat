/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */

// Disable missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeat.Box2dTestHarness
{
    public class ListItem 
    {
        string _itemCode;
        string _itemDescription;
        object _data;

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
            _data = data;
        }

        #endregion

        #region Properties

        public string ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value??string.Empty; }
        }

        public string ItemDescription
        {
            get { return _itemDescription; }
            set { _itemDescription = value??string.Empty; }
        }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return _itemDescription;
        }

        #endregion
    }
}
