using UnityEngine;

public class Std_RobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_GameStart;
    [SerializeField] private DaniTechUIButton Button_GameQuit;

    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_Button_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_Button_GameQuit);

    }

    public void OnClick_Button_GameStart()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.StdRobbyUI);
        DaniTechUIManager.Instance.OpenUI(DaniTechUIRootType.MainUI, DaniTechUIType.StdMainUI);
    }

    public void OnClick_Button_GameQuit()
    {
        DaniTechGameManager.Inst.SaveAndEndGame();
    }

}
