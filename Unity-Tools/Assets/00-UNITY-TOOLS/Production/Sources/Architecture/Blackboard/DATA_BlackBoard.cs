using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  A class allowing to store an array of variables by entities. 
///  This allows information to be transmitted through multiple components by reducing their coupling.
///  Useful for purpose such as: 
///  - Finite State machine: to transit information through state without referencing different components or object.
///  - Indepedant monobehaviours: you can design very small behaviour without needed interaction with other, you just needed a blackboard holding this data. (i.e. get the direction; know if a character is landing, dead...)
/// </summary>
[CreateAssetMenu(menuName = "Framework Data/BlackBoard", order = 0)]
public class DATA_BlackBoard : ScriptableObject_AdvPostProcessing//, IClonable
{
    #region CLASS
    [System.Serializable]
    private class ParametersBoard
    {
        [SerializeField]
        private Dictionary<string, InternalBlackboardParameter> ParametersMap;

        public void Build(List<InternalBlackboardParameter> bbp)
        {
            if(ParametersMap != null)
            {
                ParametersMap.Clear();
            }
            else
            {
                ParametersMap = new Dictionary<string, InternalBlackboardParameter>(bbp.Count);
            }

            for (int i = 0, c = bbp.Count; i < c; ++i)
            {
                InternalBlackboardParameter cur = bbp[i].Clone() as InternalBlackboardParameter;
                cur.Init();
                ParametersMap.Add(cur.Name, cur);
            }
        }

        public bool SetReference<T, TVariable>(string entryName, ref TVariable var)
            where TVariable : DataVariable<T>
        {
            if (ParametersMap.ContainsKey(entryName))
            {
                InternalBlackboardParameter<TVariable> b = ParametersMap[entryName] as InternalBlackboardParameter<TVariable>;
                b.Value = var;
                return b != null ? b.Value : null;
            }

            return false;
        }

        public TVariable GetVariable<T, TVariable>(string entryName)
            where TVariable : DataVariable<T>
        {
            if (entryName != null && entryName != "" && ParametersMap.ContainsKey(entryName))
            {
                InternalBlackboardParameter<TVariable> b = ParametersMap[entryName] as InternalBlackboardParameter<TVariable>;
                return b != null ? b.Value : null;
            }

            return null;
        }

        public string Scan()
        {
            string Ah = "";
            foreach(var v in ParametersMap.Keys)
            {
                Ah += "- \"" + v + "\"" + ParametersMap[v] + "\n";
            }
            return Ah;
        }
    }
    #endregion


    #region PROPERTIES
    [HideInInspector]
    public List<InternalBlackboardParameter> BlackboardParameters = new List<InternalBlackboardParameter>();

    // The template blackboard.
    private ParametersBoard ParametersMap = null;

    // The map holding blackboard per object.
    private Dictionary<GameObject, ParametersBoard> m_EntitiesMap = null;
    private Dictionary<GameObject, ParametersBoard> EntitiesMap
    {
        get
        {
            if(m_EntitiesMap == null)
            {
                if (ParametersMap == null)
                {
                    ParametersMap = new ParametersBoard();
                }
                ParametersMap.Build(BlackboardParameters);
                m_EntitiesMap = new Dictionary<GameObject, ParametersBoard>(3);
            }
            return m_EntitiesMap;
        }
    }
    #endregion


    #region CONSTRUCTOR
    public void Awake()
    {
        if(ParametersMap == null)
        {
            ParametersMap = new ParametersBoard();
        }
        ParametersMap.Build(BlackboardParameters);
    }
    #endregion


    #region PUBLIC METHODS - TEMPLATE
    public bool SetReference<T, TVariable>(BlackboardTargetParameter target, ref TVariable var)
        where TVariable : DataVariable<T>
    {
        if (EntitiesMap.ContainsKey(target.Target))
        {
            return m_EntitiesMap[target.Target].SetReference<T, TVariable>(target.EntryName, ref var);
        }
        else
        {
            m_EntitiesMap.Add(target.Target, ParametersMap);
            target.Initialize(this);
            return SetReference<T, TVariable>(target, ref var);
        }
    }

    public TVariable GetVariable<T, TVariable>(BlackboardTargetParameter target)
        where TVariable : DataVariable<T>
    {
        if (EntitiesMap.ContainsKey(target.Target))
        {
            if (target.EntryName != null && target.EntryName == "")
            {
                return null;
            }
            return m_EntitiesMap[target.Target].GetVariable<T, TVariable>(target.EntryName);
        }
        else
        {
            m_EntitiesMap.Add(target.Target, ParametersMap);
            target.Initialize(this);
            return GetVariable<T, TVariable>(target);
        }
    }

    public void SetValue<T>(BlackboardTargetParameter target, T var)
    {
        if (EntitiesMap.ContainsKey(target.Target))
        {
            DataVariable<T> val = GetVariable<T, DataVariable<T>>(target);
            if (val)
            {
                val.Value = var;
            }
        }
        else
        {
            m_EntitiesMap.Add(target.Target, ParametersMap);
            target.Initialize(this);
            SetValue<T>(target, var);
        }
    }

    public bool SetReference<T, TVariable>(GameObject target, string entryName, ref TVariable var)
        where TVariable : DataVariable<T>
    {
        if (EntitiesMap.ContainsKey(target))
        {
            return m_EntitiesMap[target].SetReference<T, TVariable>(entryName, ref var);
        }
        else
        {
            m_EntitiesMap.Add(target, ParametersMap);
            return SetReference<T, TVariable>(target, entryName, ref var);
        }
    }

    public TVariable GetVariable<T, TVariable>(GameObject target, string entryName)
        where TVariable : DataVariable<T>
    {
        if (EntitiesMap.ContainsKey(target))
        {
            if (entryName != null && entryName == "")
            {
                return null;
            }
            return m_EntitiesMap[target].GetVariable<T, TVariable>(entryName);
        }
        else
        {
            m_EntitiesMap.Add(target, ParametersMap);
            return GetVariable<T, TVariable>(target, entryName);
        }
    }

    public void SetValue<T, TVariable>(GameObject target, string entryName, T var)
        where TVariable : DataVariable<T>
    {
        if (EntitiesMap.ContainsKey(target))
        {
            TVariable val = GetVariable<T, TVariable>(target, entryName);
            if (val)
            {
                val.Value = var;
            }
        }
        else
        {
            m_EntitiesMap.Add(target, ParametersMap);
            SetValue<T, TVariable>(target, entryName, var);
        }
    }
    #endregion



    #region PUBLIC METHODS
    public void ScanParameters(GameObject target)
    {
        if(EntitiesMap.ContainsKey(target))
        {
            Debug.Log(this + "[" + target +"] params:\n" + m_EntitiesMap[target].Scan());
        }
        else
        {
            Debug.LogWarning(this + " doesn't contain the key '" + target + "'!");
        }
    }

    // BOOLEAN
    public BoolVariable GetBooleanVariable(BlackboardTargetParameter target)
    {
        return GetVariable<bool, BoolVariable>(target);
    }

    public void SetBooleanValue(BlackboardTargetParameter target, bool value)
    {
        SetValue(target, value);
    }

    public bool SetBooleanReference(BlackboardTargetParameter target, ref BoolVariable var)
    {
        return SetReference<bool, BoolVariable>(target, ref var);
    }


    // INTEGER
    public IntVariable GetIntegerVariable(BlackboardTargetParameter target)
    {
        return GetVariable<int, IntVariable>(target);
    }

    public void SetIntegerValue(BlackboardTargetParameter target, int value)
    {
        SetValue(target, value);
    }

    public bool SetIntegerReference(BlackboardTargetParameter target, ref IntVariable var)
    {
        return SetReference<int, IntVariable>(target, ref var);
    }


    // FLOAT
    public FloatVariable GetFloatVariable(BlackboardTargetParameter target)
    {
        return GetVariable<float, FloatVariable>(target);
    }

    public void SetFloatValue(BlackboardTargetParameter target, float value)
    {
        SetValue(target, value);
    }

    public bool SetFloatReference(BlackboardTargetParameter target, ref FloatVariable var)
    {
        return SetReference<float, FloatVariable>(target, ref var);
    }


    // VECTOR2
    public Vector2Variable GetVector2Variable(BlackboardTargetParameter target)
    {
        return GetVariable<Vector2, Vector2Variable>(target);
    }

    public void SetVector2Value(BlackboardTargetParameter target, Vector2 value)
    {
        SetValue(target, value);
    }

    public bool SetVector2Reference(BlackboardTargetParameter target, ref Vector2Variable var)
    {
        return SetReference<Vector2, Vector2Variable>(target, ref var);
    }


    // VECTOR3
    public Vector3Variable GetVector3Variable(BlackboardTargetParameter target)
    {
        return GetVariable<Vector3, Vector3Variable>(target);
    }

    public void SetVector3Value(BlackboardTargetParameter target, Vector3 value)
    {
        SetValue(target, value);
    }

    public bool SetVector3Reference(BlackboardTargetParameter target, ref Vector3Variable var)
    {
        return SetReference<Vector3, Vector3Variable>(target, ref var);
    }


    // GAMEOBJECT
    public GameObjectVariable GetGameObjectVariable(BlackboardTargetParameter target)
    {
        return GetVariable<GameObject, GameObjectVariable>(target);
    }

    public void SetGameObjectValue(BlackboardTargetParameter target, GameObject value)
    {
        SetValue(target, value);
    }

    public bool SetGameObjectReference(BlackboardTargetParameter target, ref GameObjectVariable var)
    {
        return SetReference<GameObject, GameObjectVariable>(target, ref var);
    }
    #endregion
}
#region PRIVATE METHODS
/*public static DATA_BlackBoard CreateInstance(DATA_BlackBoard[] templates)
{
    if (templates != null && templates.Length > 0)
    {

        DATA_BlackBoard obj = CreateInstance(templates[0].GetType()) as DATA_BlackBoard;

        int totalParams = 0;
        for (int i = 0; i < templates.Length; ++i)
        {
            totalParams += templates[i].ParametersMap.Count;
        }

        obj.ParametersMap = new Dictionary<string, BlackboardParameter>(totalParams);

        for (int i = 0, j = 0; i < templates.Length; ++i)
        {
            List<BlackboardParameter> bp = templates[j].BlackboardParameters;
            for (j = 0; j < bp.Count; j++)
            {
                BlackboardParameter cur = bp[i].Clone() as BlackboardParameter;
                cur.Init();
                obj.ParametersMap.Add(cur.Name, cur);

            }
        }

        return obj;
    }

    return null;
}
*/
//public virtual IClonable Clone()
//{
//    DATA_BlackBoard obj = CreateInstance(GetType()) as DATA_BlackBoard;

//    obj.ParametersMap = new Dictionary<string, BlackboardParameter>(BlackboardParameters.Count);

//    for (int i = 0, c = BlackboardParameters.Count; i < c; ++i)
//    {
//        BlackboardParameter cur = BlackboardParameters[i].Clone() as BlackboardParameter;
//        cur.Init();
//        obj.ParametersMap.Add(cur.Name, cur);
//    }

//    return obj;
//}
//public bool SetReference<T, TVariable>(string entryName, ref TVariable var)
//    where TVariable : DataVariable<T>
//{
//    if (ParametersMap.ContainsKey(entryName))
//    {
//        BlackboardParameter<TVariable> b = ParametersMap[entryName] as BlackboardParameter<TVariable>;
//        b.Value = var;
//        return b != null ? b.Value : null;
//    }

//    return false;
//}

//public TVariable GetVariable<T, TVariable>(string entryName)
//    where TVariable : DataVariable<T>
//{
//    if (ParametersMap.ContainsKey(entryName))
//    {
//        BlackboardParameter<TVariable> b = ParametersMap[entryName] as BlackboardParameter<TVariable>;
//        return b != null ? b.Value : null;
//    }

//    return null;
//}
#endregion
/*public void SetBooleanReference(string entryName, ref BoolVariable var)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Boolean b = ParametersMap[entryName] as BlackboardParameter_Boolean;
            b.Value = var;
            //return b != null ? b.Value : null;
        }
    }

    public BoolVariable GetBoolVariable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Boolean b = ParametersMap[entryName] as BlackboardParameter_Boolean;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public FloatVariable GetFloatVariable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Float b = ParametersMap[entryName] as BlackboardParameter_Float;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public void SetFloatReference(string entryName, ref FloatVariable var)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Float b = ParametersMap[entryName] as BlackboardParameter_Float;
            b.Value = var;
            //return b != null ? b.Value : null;
        }
    }

    public IntVariable GetIntVariable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Int b = ParametersMap[entryName] as BlackboardParameter_Int;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public StringVariable GetStringVariable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_String b = ParametersMap[entryName] as BlackboardParameter_String;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public Vector2Variable GetVector2Variable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Vector2 b = ParametersMap[entryName] as BlackboardParameter_Vector2;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public Vector3Variable GetVector3Variable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_Vector3 b = ParametersMap[entryName] as BlackboardParameter_Vector3;
            return b != null ? b.Value : null;
        }

        return null;
    }
    public GameObjectVariable GetGameObjectVariable(string entryName)
    {
        if (ParametersMap.ContainsKey(entryName))
        {
            BlackboardParameter_GameObject b = ParametersMap[entryName] as BlackboardParameter_GameObject;
            return b != null ? b.Value : null;
        }

        return null;
    }*/


