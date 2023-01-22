using Microsoft.Xna.Framework;

namespace Petal.Scenery;

public abstract class Scene
{
	public Color BackgroundColor
	{
		get;
		set;
	} = Color.CornflowerBlue;

	protected Scene()
	{
		
	}

	public virtual void Initialize()
	{
		
	}

	public virtual void Update()
	{
		
	}

	public virtual void Draw()
	{
		
	}
	
	public virtual void Begin()
	{
		
	}

	public virtual void End()
	{
		
	}

	public virtual void Exit()
	{
		
	}
}