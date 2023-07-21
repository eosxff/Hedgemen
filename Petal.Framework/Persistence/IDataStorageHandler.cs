namespace Petal.Framework.Persistence;

public interface IDataStorageHandler
{
	public DataStorage WriteStorage();
	public void ReadStorage(DataStorage storage);
}