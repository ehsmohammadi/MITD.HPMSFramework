

namespace MITD.Domain.Model
{
    public interface IEntity<T>
    {
        bool SameIdentityAs(T other);
    }
}
