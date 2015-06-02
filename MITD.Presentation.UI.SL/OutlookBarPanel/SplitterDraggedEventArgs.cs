using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Presentation.UI.OutlookBarPanel;

namespace MITD.Presentation.UI
{
    public class SplitterDraggedEventArgs : EventArgs
    {
        private readonly double _dragDistance = 30;

        public SplitterDraggedEventArgs (double dragDistance)
	    {
            _dragDistance = dragDistance;
	    }

        public double DragDistance 
        { 
            get { return _dragDistance; }
        }
    }
}
