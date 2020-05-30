
using UnityEngine;


public class InternalBlackboardParameter : ScriptableObject, IClonable
{
    public StringReference Name = new StringReference();
    public string Description = "Enter a description...";
    [HideInInspector]
    public bool MustBeShared;

    public virtual IClonable Clone()
    {
        InternalBlackboardParameter obj = CreateInstance(GetType()) as InternalBlackboardParameter;
        obj.Name = Name;
        obj.Description = Description;
        obj.MustBeShared = MustBeShared;
        return obj;
    }

    public virtual void Init()
    { }

    public virtual void Close()
    { }
}

public class InternalBlackboardParameter<T> : InternalBlackboardParameter
    where T : ScriptableObject
{
    public T Value;

    public override string ToString()
    {
        return "{" + Value + "}";
    }

    public override IClonable Clone()
    {
        InternalBlackboardParameter<T> obj = base.Clone() as InternalBlackboardParameter<T>;
        if (obj)
        {
            obj.Value = Value;
            return obj;
        }
        return null;
    }

    public override void Init()
    {
        if(!MustBeShared
            || (MustBeShared && Value == null))
        {
            Value = ScriptableObject.CreateInstance<T>();
        }
    }

    public override void Close()
    {
        if(!MustBeShared && Value != null)
        {
            Value = null;
        }
    }
}


