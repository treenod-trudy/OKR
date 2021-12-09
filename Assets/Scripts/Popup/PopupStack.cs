using System.Collections.Generic;

public class PopupStack
{
    #region StackData
    private struct Data
    {
        public Popup from;
        public Popup to;
    }
    #endregion
    
    #region private
    private readonly Stack<Data> _popupStackDatas = new Stack<Data>();
    #endregion
    
    #region properties
    public Popup CurrentPopup => Depth > 0 ? _popupStackDatas.Peek().to : null;
    public int Depth => _popupStackDatas.Count;
    #endregion

    #region method
    public void Push(Popup to)
    {
        //TODO: need memoryPool
        OnPush(new Data { from = CurrentPopup, to = to });
    }
    private void OnPush(Data data)
    {
        bool exist = data.to != null;
        if (!exist)
        {
            return;
        }

        Popup popup = data.to;
        popup.Show();
        
        switch (popup.Style)
        {
            case PopupStyle.Layout: break;
            case PopupStyle.Screen:
            {
                IEnumerator<Data> enumr = _popupStackDatas.GetEnumerator(); 
                while (enumr.MoveNext())
                {
                    Popup target = enumr.Current.to;
                    if (target != null)
                        target.Hide();
                }
                break;
            }
        }
        
        _popupStackDatas.Push(data);
    }
    
    public void Pop()
    {
        if (Depth > 0)
        {
            OnPop(_popupStackDatas.Pop());
        }
    }
    private void OnPop(Data data)
    {
        Popup popup = data.to;
        popup.Hide();
        
        switch (popup.Style)
        {
            case PopupStyle.Layout: break;
            case PopupStyle.Screen:
            {
                IEnumerator<Data> enumr = _popupStackDatas.GetEnumerator(); 
                while (enumr.MoveNext())
                {
                    Popup target = enumr.Current.to;
                    if (target != null)
                    {
                        target.Show();

                        bool last = target.Style == PopupStyle.Screen;
                        if (last)
                        {
                            return;
                        }
                    }
                }
                break;
            }
        }
    }
    
    public void ClearAll()
    {
        while (Depth > 0)
        {
            Pop();
        }
    }
    #endregion
}