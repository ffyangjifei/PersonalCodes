using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/**************************************************
* 
* 创建者：		yangjifei
* 创建时间：	2018/03/12 16:13
* 描述：		迭代器创建UI列表
*               T:UI组件  T2:迭代器参数
* 
**************************************************/
[System.Serializable]
public abstract class GridItemCreator<T, T2> : MonoBehaviour where T : GridItemCreatorCellBase<T2>
{
    [SerializeField]
    public T prefab;
    [SerializeField]
    public GameObject GridObj;
    public List<T> itemList = new List<T>();
    public Transform[] KeepLastSibling;
    private bool isRegistered = false;
    public void OnDestroy()
    {
        if (isRegistered)
        {
            UnRegister();
        }
    }
    public void CreateGrid(bool clearAll = true, bool sort = false)
    {
        if (!isRegistered)
        {
            isRegistered = true;
            Register();
        }
        if (prefab == null)
        {
            prefab = GridObj.transform.GetChild(0).GetComponent<T>();
        }
        prefab.gameObject.SetActive(false);
        while (clearAll && itemList.Count > 0)
        {
            GameObject.Destroy(itemList[0].gameObject);
            itemList.RemoveAt(0);
        }
        var ienu = createIenumerator();
        while (ienu.MoveNext())
        {
            var instantitate = NGUITools.AddChild(prefab.transform.parent.gameObject, prefab.gameObject);
            //var cell = GameObject.Instantiate(prefab.gameObject).GetComponent<T>();
            var cell = instantitate.GetComponent<T>();
            cell.gameObject.SetActive(true);
            //cell.transform.parent = prefab.transform.parent;
            cell.transform.localPosition = Vector3.zero;
            cell.transform.localScale = Vector3.one;
            itemList.Add(cell);
            createFunc(cell,ienu.Current);
        }
        OnGridItemUpdate(sort);
    }
    public T CreateSingleItem(T2 p,bool sort=false,bool rePosition=true)
    {
        if (prefab == null)
        {
            prefab = GridObj.transform.GetChild(0).GetComponent<T>();
            prefab.gameObject.SetActive(false);
        }

        var cell = GameObject.Instantiate(prefab.gameObject).GetComponent<T>();
        cell.gameObject.SetActive(true);
        cell.transform.parent = prefab.transform.parent;
        cell.transform.localPosition = Vector3.zero;
        cell.transform.localScale = Vector3.one;
        itemList.Add(cell);
        createFunc(cell, p);

        OnGridItemUpdate(sort,rePosition);
        return cell;
    }
    private void OnGridItemUpdate(bool sort,bool rePosition=true)
    {
        if (sort)
        {
            SortItems();
        }
        if (KeepLastSibling.Length>0)
        {
            foreach (var item in KeepLastSibling)
            {
                if (item!=null)
                {
                    item.transform.SetAsLastSibling();
                }
            }
        }
        if (rePosition)
        {
            CoroutineHelper.StartYieldWaitFramesCallback(1, () =>
            {
                GridObj.GetComponent<UIGrid>().Reposition();
            });

            CoroutineHelper.StartYieldWaitFramesCallback(2, () =>
            {
                var panel = GridObj.transform.parent.GetComponent<UIDraggablePanel>();
                if (panel != null && panel.enabled)
                {
                    panel.ResetPosition();
                }
            });
        }
    }

    private void SortItems()
    {
        itemList.Sort();
        foreach (var item in itemList)
        {
            item.transform.SetAsLastSibling();
        }
    }
    public void DeleteSingleItem(T2 p)
    {
        T toDelete = null;
        foreach (var item in itemList)
        {
            if (item.GetGridItemCreatorParam().Equals(p))
            {
                toDelete = item;
                break;
            }
        }
        if (toDelete!=null)
        {
            itemList.Remove(toDelete);
            GameObject.Destroy(toDelete.gameObject);

            CoroutineHelper.StartYieldWaitFramesCallback(1, () =>
            {
                GridObj.GetComponent<UIGrid>().Reposition();
            });

            CoroutineHelper.StartYieldWaitFramesCallback(2, () =>
            {
                GridObj.transform.parent.GetComponent<UIDraggablePanel>().ResetPosition();
            });

        }
    }
    public T ContainsItem(T2 p)
    {
        foreach (var item in itemList)
        {
            if (item.GetGridItemCreatorParam().Equals(p))
            {
                return item;
            }
        }
        return null;
    }


    /// <summary>
    /// 创建列表时用的迭代器
    /// </summary>
    protected abstract IEnumerator<T2> createIenumerator();
    /// <summary>
    /// 实例化后的回调
    /// </summary>
    /// <param name="cell">UI组件</param>
    /// <param name="t2">迭代器参数</param>
    protected virtual void createFunc(T cell, T2 t2)
    {
        cell.GridItemCellSetUp(t2);
    }

    protected virtual void Register()
    {

    }
    protected virtual void UnRegister()
    {

    }
}
