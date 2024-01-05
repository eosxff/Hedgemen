namespace Petal.Framework.Persistence;

public interface IPersistent
{
	public PersistentData WriteData();
	public void ReadData(PersistentData data);
}
