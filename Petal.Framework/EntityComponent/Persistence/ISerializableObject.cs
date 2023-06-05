namespace Petal.Framework.EntityComponent.Persistence;

public interface ISerializableObject
{
	public SerializedRecord WriteObjectState();
	public void ReadObjectState(SerializedRecord record);
}