using UnityEngine;

public class Std_RobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_GameStart;
    [SerializeField] private DaniTechUIButton Button_GameQuit;
    [SerializeField] private DaniTechUIButton Button_GameBook;

    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_Button_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_Button_GameQuit);
        Button_GameBook.BindOnClickButtonEvent(OnClick_Button_GameBook);

    }

    public void OnClick_Button_GameStart()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.Std_RobbyUI);
        DaniTechUIManager.Instance.OpenUI(DaniTechUIRootType.MainUI, DaniTechUIType.Std_MainUI);
    }

    public void OnClick_Button_GameQuit()
    {
        DaniTechGameManager.Inst.SaveAndEndGame();
    }

    public void OnClick_Button_GameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.Std_GameBookUI);
    }

}
