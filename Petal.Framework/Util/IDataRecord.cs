namespace Petal.Framework.Util;

public interface IDataRecord<T>
{
	public T Create();
	public void Read(T obj);
}