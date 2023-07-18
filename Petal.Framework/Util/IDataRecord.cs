namespace Petal.Framework.Util;

public interface IDataRecord<T>
{
	public void Read(T obj);
}
