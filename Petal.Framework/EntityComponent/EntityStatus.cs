using System;

namespace Petal.Framework.EntityComponent;

[Flags]
public enum EntityStatus
{
    Active = 0x1,
    Inactive = 0x2,
    PendingRemoval = 0x4
}