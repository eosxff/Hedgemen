namespace Petal.Framework.Persistence;

public interface ISerializableObject
{
	public SerializedData WriteObjectState();
	public void ReadObjectState(SerializedData data);
}