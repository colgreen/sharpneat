/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */

// Disable missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeatGUI
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
