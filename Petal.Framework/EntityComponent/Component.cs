using System;

namespace Petal.Framework.EntityComponent;

public abstract class Component
{
    protected Component()
    {
        
    }
    
    public abstract ComponentInfoQuery QueryComponentInfo();
}

public struct ComponentInfoQuery
{
    public NamespacedString ContentIdentifier
    {
        get;
        init;
    }

    public Type ComponentType
    {
        get;
        init;
    }
}