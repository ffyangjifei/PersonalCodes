using UnityEngine;
using System.Collections;

public class GridItemCreatorCellBase<T> : MonoBehaviour
{
    public virtual T GetGridItemCreatorParam()
    {
        return m_param;
    }
    protected T m_param;
    public virtual void GridItemCellSetUp(T p)
    {
        m_param = p;
    }
}
