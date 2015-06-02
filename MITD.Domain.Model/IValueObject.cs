namespace MITD.Domain.Model
{
    public interface IValueObject<T>
    {
        bool SameValueAs(T other);
    }
}
