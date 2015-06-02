using System;

namespace MITD.Presentation
{
    public interface IDialogView
    {
        string Title { get; set; }
        Object DialogContent { get; set; }
    }
}
