using System;

namespace MITD.Core
{
	public class EventHandlerOptions
	{
		internal Action<Exception> ErrorHandler { get; private set; }
		internal WeakReference EventHandler { get; private set; }
		
		public EventHandlerOptions(object eventHandler)
		{
			EventHandler = new WeakReference(eventHandler);
		}

		public void WithErrorHandler(Action<Exception> errorHandler)
		{
			ErrorHandler = errorHandler;
		}
	}
}