using UnityEngine;

public class Std_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_StashInventory; 

    private bool _isUIOpened = false;

    private void OnEnable()
    {
        _isUIOpened = false;
    }

    private void OnClick_Button_StashInventory()
    {
        _isUIOpened = !_isUIOpened;

        if (_isUIOpened)
        {
            DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.Std_StashInventoryUI);
        }
        else
        {
            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.Std_StashInventoryUI);
        }
    }
}