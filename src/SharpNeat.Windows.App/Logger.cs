// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Collections;
using System.ComponentModel;
using Redzen.Structures;

namespace SharpNeat.Windows.App;

/// <summary>
/// Logging control. The log4net subsystem is configured to pass log message through to this class'
/// static Log() method. Log messages are held in a circular buffer and are displayed in a referenced
/// GUI ListBox.
/// </summary>
internal sealed class Logger
{
    delegate void LogDelegate(string message);
    static readonly LogDelegate logDelegate = new(Log);

    static ListBox __lbxLog;
    static readonly BindingList<LogItem> __bindingList;

    static Logger()
    {
        __bindingList = new BindingList<LogItem>(
            new LogBuffer(500))
            {
                // Add an initial empty log item; otherwise ListBox throws an exception when the first item is added.
                new("")
            };
    }

    /// <summary>
    /// Assign the GUI ListBox to display log messages on.
    /// </summary>
    /// <param name="lbxLog"></param>
    public static void SetListBox(ListBox lbxLog)
    {
        __lbxLog = lbxLog;
        lbxLog.DataSource = __bindingList;
        lbxLog.ValueMember = "Message";
        lbxLog.DisplayMember = "Message";
    }

    /// <summary>
    /// Log a message.
    /// </summary>
    public static void Log(string message)
    {
        // Handle calls from threads other than the GUI thread (re-invoke the method on the GUI thread, but don't wait for completion).
        if(__lbxLog.InvokeRequired)
        {
            __lbxLog.BeginInvoke(logDelegate, new object[] { message });
            return;
        }
        __bindingList.Add(new LogItem(message));
        __lbxLog.SelectedIndex = __lbxLog.Items.Count-1;
    }

    /// <summary>
    /// Clear the circular history buffer of log messages.
    /// </summary>
    public static void Clear()
    {
        __bindingList.Clear();
    }

    #region Inner Classes

    // This weird wrapper around a string is necessary in order to use BindingList<LogItem>, because we can't use BindingList<string>.
    internal sealed class LogItem(string message)
    {
        public string Message { get; private set; } = message;
    }

    internal sealed class LogBuffer(int capacity) 
        : CircularBuffer<LogItem>(capacity), IList<LogItem>
    {
        #region IList<LogItem>

        int IList<LogItem>.IndexOf(LogItem item)
        {
            throw new NotImplementedException();
        }

        void IList<LogItem>.Insert(int index, LogItem item)
        {
            Enqueue(item);
        }

        void IList<LogItem>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        LogItem IList<LogItem>.this[int index]
        {
            get { return this[index]; }
            set { return; }
        }

        void ICollection<LogItem>.Add(LogItem item)
        {
            Enqueue(item);
        }

        void ICollection<LogItem>.Clear()
        {
            Clear();
        }

        bool ICollection<LogItem>.Contains(LogItem item)
        {
            throw new NotImplementedException();
        }

        void ICollection<LogItem>.CopyTo(LogItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<LogItem>.Count
        {
            get { return Length; }
        }

        bool ICollection<LogItem>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<LogItem>.Remove(LogItem item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<LogItem> IEnumerable<LogItem>.GetEnumerator()
        {
            int len = Length;
            for(int i=0; i < len; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int len = Length;
            for(int i=0; i < len; i++)
            {
                yield return this[i];
            }
        }

        #endregion
    }

    #endregion
}
