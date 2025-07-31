using System;

public interface ILifecycleNotifier
{
    event Action OnDone;
}
