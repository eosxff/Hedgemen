namespace Petal.Framework.Scenery;

public class NodeSelection
{
	public Node? Target;
	public Node? PreviousTarget;

	public void Update()
	{
		PreviousTarget = Target;
		Target = null;
	}
	
	public void Reset()
	{
		Target = null;
	}
}