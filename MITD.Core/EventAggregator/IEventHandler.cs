namespace MITD.Core
{
	public interface IEventHandler<T>
	{
		void Handle(T eventData);
	}
}