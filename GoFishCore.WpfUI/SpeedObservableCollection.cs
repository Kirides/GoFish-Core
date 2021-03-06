﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace GoFishCore.WpfUI
{
    public class SpeedObservableCollection<T> : ObservableCollection<T>
    {
        public SpeedObservableCollection()
        {
            _suspendCollectionChangeNotification = false;
        }

        bool _suspendCollectionChangeNotification;

        public void RaiseCollectionChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suspendCollectionChangeNotification)
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() => OnCollectionChanged(e)), System.Windows.Threading.DispatcherPriority.DataBind);
                    return;
                }
                base.OnCollectionChanged(e);
            }
        }

        public void SuspendCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = true;
        }

        public void ResumeCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = false;
        }

        public void AddRange(IEnumerable<T> items)
        {
            bool shouldResume = !_suspendCollectionChangeNotification;
            SuspendCollectionChangeNotification();
            try
            {
                foreach (var i in items)
                    base.InsertItem(Count, i);
            }
            finally
            {
                if (shouldResume)
                {
                    ResumeCollectionChangeNotification();
                }
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(arg);
            }
        }
    }
}
